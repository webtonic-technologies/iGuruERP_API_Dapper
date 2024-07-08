using Dapper;
using System.Data;
using System.Data.Common;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Repository.Interfaces;

namespace Transport_API.Repository.Implementations
{
    public class RoutePlanRepository : IRoutePlanRepository
    {
        private readonly IDbConnection _dbConnection;
        public RoutePlanRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<ServiceResponse<string>> AddUpdateRoutePlan(RoutePlanRequestDTO routePlan)
        {
            string sql;
            if (routePlan.RoutePlanID == 0)
            {
                sql = @"INSERT INTO tblRoutePlan (RouteName, VehicleID, InstituteID, IsActive) 
                VALUES (@RouteName, @VehicleID, @InstituteID, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int);";

                routePlan.RoutePlanID = await _dbConnection.QuerySingleAsync<int>(sql, routePlan);
            }
            else
            {
                sql = @"UPDATE tblRoutePlan SET RouteName = @RouteName, VehicleID = @VehicleID, InstituteID = @InstituteID, IsActive = @IsActive 
                WHERE RoutePlanID = @RoutePlanID";
                await _dbConnection.ExecuteAsync(sql, routePlan);
            }

            if (routePlan.RoutePlanID > 0)
            {
                bool stopsHandled = await HandleRouteStops(routePlan.RoutePlanID, routePlan.RouteStops);

                if (stopsHandled)
                {
                    return new ServiceResponse<string>(true, "Operation Successful", "Route plan added/updated successfully", StatusCodes.Status200OK);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation Failed", "Error handling route stops", StatusCodes.Status400BadRequest);
                }
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating route plan", StatusCodes.Status400BadRequest);
            }
        }
        public async Task<ServiceResponse<IEnumerable<RoutePlanResponseDTO>>> GetAllRoutePlans(GetAllRoutePlanRequest request)
        {
            try
            {
                string countSql = @"SELECT COUNT(*) FROM tblRoutePlan WHERE IsActive = 1";
                if (request.VehicleId > 0)
                {
                    countSql += " AND VehicleID = @VehicleId";
                }

                int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql, new { VehicleId = request.VehicleId });

                string sql = @"SELECT rp.RoutePlanID, rp.RouteName, rp.VehicleID, v.VehicleNumber as VehicleName , rp.InstituteID, rp.IsActive 
                       FROM tblRoutePlan rp
                       JOIN tblVehicleMaster v ON rp.VehicleID = v.VehicleID
                       WHERE rp.IsActive = 1";

                if (request.VehicleId > 0)
                {
                    sql += " AND rp.VehicleID = @VehicleId";
                }

                sql += @" ORDER BY rp.RoutePlanID 
                  OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var routePlans = await _dbConnection.QueryAsync<RoutePlanResponseDTO>(sql, new
                {
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize,
                    VehicleId = request.VehicleId
                });

                foreach (var routePlan in routePlans)
                {
                    routePlan.RouteStops = await GetRouteStopsByRoutePlanID(routePlan.RoutePlanID);
                }

                if (routePlans.Any())
                {
                    return new ServiceResponse<IEnumerable<RoutePlanResponseDTO>>(true, "Records Found", routePlans, StatusCodes.Status200OK, totalCount);
                }
                else
                {
                    return new ServiceResponse<IEnumerable<RoutePlanResponseDTO>>(false, "No Records Found", [], StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<RoutePlanResponseDTO>>(false, ex.Message, [], StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<ServiceResponse<RoutePlanResponseDTO>> GetRoutePlanById(int RoutePlanID)
        {
            try
            {
                string sql = @"SELECT rp.RoutePlanID, rp.RouteName, rp.VehicleID,  v.VehicleNumber as VehicleName, rp.InstituteID, rp.IsActive
                       FROM tblRoutePlan rp
                       JOIN tblVehicleMaster v ON rp.VehicleID = v.VehicleID
                       WHERE rp.RoutePlanID = @RoutePlanID AND rp.IsActive = 1";

                var routePlan = await _dbConnection.QueryFirstOrDefaultAsync<RoutePlanResponseDTO>(sql, new { RoutePlanID });

                if (routePlan != null)
                {
                    routePlan.RouteStops = await GetRouteStopsByRoutePlanID(routePlan.RoutePlanID);
                    return new ServiceResponse<RoutePlanResponseDTO>(true, "Record Found", routePlan, StatusCodes.Status200OK);
                }
                else
                {
                    return new ServiceResponse<RoutePlanResponseDTO>(false, "Record Not Found", new RoutePlanResponseDTO(), StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<RoutePlanResponseDTO>(false, ex.Message, new RoutePlanResponseDTO(), StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<ServiceResponse<bool>> UpdateRoutePlanStatus(int RoutePlanID)
        {
            string sql = @"UPDATE tblRoutePlan SET IsActive = @IsActive WHERE RoutePlanID = @RoutePlanID";
            var result = await _dbConnection.ExecuteAsync(sql, new { IsActive = false, RoutePlanID = RoutePlanID });

            if (result > 0)
            {
                return new ServiceResponse<bool>(true, "Status Updated Successfully", true, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<bool>(false, "Status Update Failed", false, StatusCodes.Status400BadRequest);
            }
        }
        private async Task<bool> HandleRouteStops(int routePlanID, List<RouteStop>? routeStops)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            if (routeStops == null || !routeStops.Any()) return true;

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    string deleteSql = @"DELETE FROM tblRouteStopMaster WHERE RoutePlanID = @RoutePlanID";
                    await _dbConnection.ExecuteAsync(deleteSql, new { RoutePlanID = routePlanID }, transaction);

                    string insertSql = @"INSERT INTO tblRouteStopMaster (RoutePlanID, StopName, PickUpTime, DropTime, FeeAmount) 
                                 VALUES (@RoutePlanID, @StopName, @PickUpTime, @DropTime, @FeeAmount)";

                    foreach (var stop in routeStops)
                    {
                        stop.RoutePlanID = routePlanID;
                        await _dbConnection.ExecuteAsync(insertSql, stop, transaction);
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
                finally
                {
                    _dbConnection.Close();
                }
            }
        }
        private async Task<List<RouteStopResponse>> GetRouteStopsByRoutePlanID(int routePlanID)
        {
            string sql = @"SELECT StopID, RoutePlanID, StopName, PickUpTime, DropTime, FeeAmount
                   FROM tblRouteStopMaster
                   WHERE RoutePlanID = @RoutePlanID";

            return (await _dbConnection.QueryAsync<RouteStopResponse>(sql, new { RoutePlanID = routePlanID })).ToList();
        }
    }
}
