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
                // Count total records for pagination
                string countSql = @"SELECT COUNT(*) FROM tblRoutePlan WHERE IsActive = 1 AND InstituteID = @InstituteID";
                int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql, new { InstituteID = request.InstituteId });

                // Validate if there are any records
                if (totalCount == 0)
                {
                    return new ServiceResponse<IEnumerable<RoutePlanResponseDTO>>(false, $"No route plans found for InstituteID: {request.InstituteId}", new List<RoutePlanResponseDTO>(), StatusCodes.Status204NoContent);
                }

                // Main SQL to retrieve paginated route plans, including the count of routeStops
                string sql = @"
        SELECT 
            rp.RoutePlanID, 
            rp.RouteName, 
            rp.VehicleID, 
            v.VehicleNumber, 
            (SELECT COUNT(*) FROM tblRouteStopMaster rs WHERE rs.RoutePlanID = rp.RoutePlanID) AS NoOfStops,
            (SELECT MIN(PickUpTime) FROM tblRouteStopMaster WHERE RoutePlanID = rp.RoutePlanID) AS PickUpTime,
            (SELECT MAX(DropTime) FROM tblRouteStopMaster WHERE RoutePlanID = rp.RoutePlanID) AS DropTime,
            ISNULL(CONCAT(e.First_Name, ' ', e.Last_Name), '') AS DriverName
        FROM 
            tblRoutePlan rp
            JOIN tblVehicleMaster v ON rp.VehicleID = v.VehicleID
            LEFT JOIN tbl_EmployeeProfileMaster e ON v.AssignDriverID = e.Employee_id
        WHERE 
            rp.IsActive = 1 
            AND rp.InstituteID = @InstituteID
        ORDER BY 
            rp.RoutePlanID 
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var routePlans = (await _dbConnection.QueryAsync<RoutePlanResponseDTO>(sql, new
                {
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize,
                    InstituteID = request.InstituteId
                })).ToList();

                // Check if the result set is empty
                if (routePlans == null || !routePlans.Any())
                {
                    return new ServiceResponse<IEnumerable<RoutePlanResponseDTO>>(false, $"No route plans found for InstituteID: {request.InstituteId}", new List<RoutePlanResponseDTO>(), StatusCodes.Status204NoContent);
                }

                // Return the retrieved route plans with routeStops count
                return new ServiceResponse<IEnumerable<RoutePlanResponseDTO>>(true, "Records Found", routePlans, StatusCodes.Status200OK, totalCount);
            }
            catch (Exception ex)
            {
                // Log the exception if needed and return the error response
                return new ServiceResponse<IEnumerable<RoutePlanResponseDTO>>(false, ex.Message, new List<RoutePlanResponseDTO>(), StatusCodes.Status500InternalServerError);
            }
        }


        public async Task<ServiceResponse<RoutePlanResponseDTO>> GetRoutePlanById(int routePlanID)
        {
            try
            {
                // SQL to retrieve the route plan by ID
                string sql = @"
            SELECT 
                rp.RoutePlanID, 
                rp.RouteName, 
                rp.VehicleID, 
                v.VehicleNumber, 
                (SELECT COUNT(*) FROM tblRouteStopMaster rs WHERE rs.RoutePlanID = rp.RoutePlanID) AS NoOfStops,
                (SELECT MIN(PickUpTime) FROM tblRouteStopMaster WHERE RoutePlanID = rp.RoutePlanID) AS PickUpTime,
                (SELECT MAX(DropTime) FROM tblRouteStopMaster WHERE RoutePlanID = rp.RoutePlanID) AS DropTime,
                ISNULL(CONCAT(e.First_Name, ' ', e.Last_Name), '') AS DriverName
            FROM 
                tblRoutePlan rp
                JOIN tblVehicleMaster v ON rp.VehicleID = v.VehicleID
                LEFT JOIN tbl_EmployeeProfileMaster e ON v.AssignDriverID = e.Employee_id
            WHERE 
                rp.RoutePlanID = @RoutePlanID 
                AND rp.IsActive = 1";

                // Execute the query to get the route plan
                var routePlan = await _dbConnection.QueryFirstOrDefaultAsync<RoutePlanResponseDTO>(sql, new { RoutePlanID = routePlanID });

                // Check if the route plan exists
                if (routePlan != null)
                {
                    // Load stops for this route plan
                    routePlan.RouteStops = await GetRouteStopsByRoutePlanID(routePlan.RoutePlanID);
                    return new ServiceResponse<RoutePlanResponseDTO>(true, "Record Found", routePlan, StatusCodes.Status200OK);
                }
                else
                {
                    // Provide a more informative response
                    return new ServiceResponse<RoutePlanResponseDTO>(false, $"No route plan found for RoutePlanID: {routePlanID}", null, StatusCodes.Status200OK);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<RoutePlanResponseDTO>(false, ex.Message, new RoutePlanResponseDTO(), StatusCodes.Status500InternalServerError);
            }
        }


        private async Task<List<RouteStopResponse>> GetRouteStopsByRoutePlanID(int routePlanID)
        {
            string sql = @"
        SELECT 
            StopID, 
            RoutePlanID, 
            StopName, 
            PickUpTime, 
            DropTime, 
            FeeAmount
        FROM 
            tblRouteStopMaster
        WHERE 
            RoutePlanID = @RoutePlanID";

            return (await _dbConnection.QueryAsync<RouteStopResponse>(sql, new { RoutePlanID = routePlanID })).ToList();
        }



        //public async Task<ServiceResponse<RoutePlanResponseDTO>> GetRoutePlanById(int RoutePlanID)
        //{
        //    try
        //    {
        //        string sql = @"
        //    SELECT 
        //        rp.RoutePlanID, 
        //        rp.RouteName, 
        //        rp.VehicleID, 
        //        v.VehicleNumber as VehicleName, 
        //        rp.InstituteID, 
        //        rp.IsActive,
        //        ISNULL(CONCAT(e.First_Name, ' ', e.Last_Name), '') AS EmployeeName,
        //        (SELECT COUNT(*) FROM tblRouteStopMaster rs WHERE rs.RoutePlanID = rp.RoutePlanID) AS NoOfStops
        //    FROM 
        //        tblRoutePlan rp
        //        JOIN tblVehicleMaster v ON rp.VehicleID = v.VehicleID
        //        LEFT JOIN tbl_EmployeeProfileMaster e ON v.AssignDriverID = e.Employee_id
        //    WHERE 
        //        rp.RoutePlanID = @RoutePlanID 
        //        AND rp.IsActive = 1";

        //        var routePlan = await _dbConnection.QueryFirstOrDefaultAsync<RoutePlanResponseDTO>(sql, new { RoutePlanID });

        //        if (routePlan != null)
        //        {
        //            routePlan.RouteStops = await GetRouteStopsByRoutePlanID(routePlan.RoutePlanID);
        //            return new ServiceResponse<RoutePlanResponseDTO>(true, "Record Found", routePlan, StatusCodes.Status200OK);
        //        }
        //        else
        //        {
        //            return new ServiceResponse<RoutePlanResponseDTO>(false, "Record Not Found", new RoutePlanResponseDTO(), StatusCodes.Status204NoContent);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<RoutePlanResponseDTO>(false, ex.Message, new RoutePlanResponseDTO(), StatusCodes.Status500InternalServerError);
        //    }
        //}

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
                    // Delete existing stops and associated payments
                    string deleteStopsSql = @"DELETE FROM tblRouteStopMaster WHERE RoutePlanID = @RoutePlanID";
                    await _dbConnection.ExecuteAsync(deleteStopsSql, new { RoutePlanID = routePlanID }, transaction);

                    // Insert each stop and retrieve its StopID
                    string insertStopSql = @"INSERT INTO tblRouteStopMaster (RoutePlanID, StopName, PickUpTime, DropTime, FeeAmount) 
                                     VALUES (@RoutePlanID, @StopName, @PickUpTime, @DropTime, @FeeAmount);
                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    foreach (var stop in routeStops)
                    {
                        stop.RoutePlanID = routePlanID;
                        // Insert stop and get the generated StopID
                        stop.StopID = await _dbConnection.QuerySingleAsync<int>(insertStopSql, stop, transaction);

                        // Handle Single Payments
                        if (stop.SinglePayment != null)
                        {
                            foreach (var singlePayment in stop.SinglePayment)
                            {
                                singlePayment.RoutePlanID = routePlanID;
                                singlePayment.StopID = stop.StopID; // Use the newly inserted StopID

                                string insertSinglePaymentSql = @"INSERT INTO tblRouteSingleFeesPayment (RoutePlanID, StopID, FeesAmount)
                                                          VALUES (@RoutePlanID, @StopID, @FeesAmount)";
                                await _dbConnection.ExecuteAsync(insertSinglePaymentSql, singlePayment, transaction);
                            }
                        }

                        // Handle Term Payments
                        if (stop.TermPayment != null)
                        {
                            foreach (var termPayment in stop.TermPayment)
                            {
                                termPayment.RoutePlanID = routePlanID;
                                termPayment.StopID = stop.StopID; // Use the newly inserted StopID

                                string insertTermPaymentSql = @"INSERT INTO tblRouteTermFeesPayment (RoutePlanID, StopID, TermName, FeesAmount, DueDate)
                                                        VALUES (@RoutePlanID, @StopID, @TermName, @FeesAmount, @DueDate)";
                                await _dbConnection.ExecuteAsync(insertTermPaymentSql, termPayment, transaction);
                            }
                        }

                        // Handle Monthly Payments
                        if (stop.MonthlyPayment != null)
                        {
                            foreach (var monthlyPayment in stop.MonthlyPayment)
                            {
                                monthlyPayment.RoutePlanID = routePlanID;
                                monthlyPayment.StopID = stop.StopID; // Use the newly inserted StopID

                                string insertMonthlyPaymentSql = @"INSERT INTO tblRouteMonthlyFeesPayment (RoutePlanID, StopID, MonthName, FeesAmount)
                                                           VALUES (@RoutePlanID, @StopID, @MonthName, @FeesAmount)";
                                await _dbConnection.ExecuteAsync(insertMonthlyPaymentSql, monthlyPayment, transaction);
                            }
                        }
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


        //private async Task<List<RouteStopResponse>> GetRouteStopsByRoutePlanID(int routePlanID)
        //{
        //    string sql = @"SELECT StopID, RoutePlanID, StopName, PickUpTime, DropTime, FeeAmount
        //           FROM tblRouteStopMaster
        //           WHERE RoutePlanID = @RoutePlanID";

        //    return (await _dbConnection.QueryAsync<RouteStopResponse>(sql, new { RoutePlanID = routePlanID })).ToList();
        //}
    }
}
