using Dapper;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
using Transport_API.Repository.Interfaces;
using System.Data;

namespace Transport_API.Repository.Implementations
{
    public class RouteMappingRepository : IRouteMappingRepository
    {
        private readonly IDbConnection _dbConnection;

        public RouteMappingRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<string>> AddUpdateRouteMapping(RouteMapping routeMapping)
        {
            string sql;
            if (routeMapping.AssignRouteID == 0)
            {
                sql = @"INSERT INTO tblAssignRoute (RoutePlanID, VehicleID, DriverID, TransportStaffID) 
                        VALUES (@RoutePlanID, @VehicleID, @DriverID, @TransportStaffID)";
            }
            else
            {
                sql = @"UPDATE tblAssignRoute SET RoutePlanID = @RoutePlanID, VehicleID = @VehicleID, DriverID = @DriverID, TransportStaffID = @TransportStaffID
                        WHERE AssignRouteID = @AssignRouteID";
            }

            var result = await _dbConnection.ExecuteAsync(sql, routeMapping);
            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Operation Successful", "Route mapping added/updated successfully", StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating route mapping", StatusCodes.Status400BadRequest);
            }
        }

        public async Task<ServiceResponse<IEnumerable<RouteMapping>>> GetAllRouteMappings(GetAllRouteMappingRequest request)
        {
            string countSql = @"SELECT COUNT(*) FROM tblAssignRoute";
            int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql);

            string sql = @"SELECT * FROM tblAssignRoute ORDER BY AssignRouteID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var routeMappings = await _dbConnection.QueryAsync<RouteMapping>(sql, new { Offset = (request.PageNumber - 1) * request.PageSize, PageSize = request.PageSize });

            if (routeMappings.Any())
            {
                return new ServiceResponse<IEnumerable<RouteMapping>>(true, "Records Found", routeMappings, StatusCodes.Status200OK, totalCount);
            }
            else
            {
                return new ServiceResponse<IEnumerable<RouteMapping>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<RouteMapping>> GetRouteMappingById(int AssignRouteID)
        {
            string sql = @"SELECT * FROM tblAssignRoute WHERE AssignRouteID = @AssignRouteID";
            var routeMapping = await _dbConnection.QueryFirstOrDefaultAsync<RouteMapping>(sql, new { AssignRouteID = AssignRouteID });

            if (routeMapping != null)
            {
                return new ServiceResponse<RouteMapping>(true, "Record Found", routeMapping, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<RouteMapping>(false, "Record Not Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateRouteMappingStatus(int AssignRouteID)
        {
            string sql = @"UPDATE tblAssignRoute SET IsActive = ~IsActive WHERE AssignRouteID = @AssignRouteID";
            var result = await _dbConnection.ExecuteAsync(sql, new { AssignRouteID = AssignRouteID });

            if (result > 0)
            {
                return new ServiceResponse<bool>(true, "Status Updated Successfully", true, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<bool>(false, "Status Update Failed", false, StatusCodes.Status400BadRequest);
            }
        }
    }
}
