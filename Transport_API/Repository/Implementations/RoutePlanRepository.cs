using Dapper;
using OfficeOpenXml;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.Responses;
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

            // Convert DueDate to DateTime for each TermPaymentDTO item
            foreach (var termPayment in routePlan.RouteStops.SelectMany(rs => rs.TermPayment))
            {
                if (DateTime.TryParseExact(termPayment.DueDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate))
                {
                    termPayment.DueDate = dueDate.ToString("yyyy-MM-dd"); // Save it in the standard SQL format
                }
                else
                {
                    return new ServiceResponse<string>(false, "Invalid Date Format", "Due date must be in DD-MM-YYYY format", StatusCodes.Status400BadRequest);
                }
            }

            // If RoutePlanID is 0, Insert a new route plan, else Update the existing route plan
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

        //public async Task<ServiceResponse<string>> AddUpdateRoutePlan(RoutePlanRequestDTO routePlan)
        //{
        //    string sql;
        //    if (routePlan.RoutePlanID == 0)
        //    {
        //        sql = @"INSERT INTO tblRoutePlan (RouteName, VehicleID, InstituteID, IsActive) 
        //VALUES (@RouteName, @VehicleID, @InstituteID, @IsActive);
        //SELECT CAST(SCOPE_IDENTITY() as int);";

        //        routePlan.RoutePlanID = await _dbConnection.QuerySingleAsync<int>(sql, routePlan);
        //    }
        //    else
        //    {
        //        sql = @"UPDATE tblRoutePlan SET RouteName = @RouteName, VehicleID = @VehicleID, InstituteID = @InstituteID, IsActive = @IsActive 
        //WHERE RoutePlanID = @RoutePlanID";
        //        await _dbConnection.ExecuteAsync(sql, routePlan);
        //    }

        //    if (routePlan.RoutePlanID > 0)
        //    {
        //        bool stopsHandled = await HandleRouteStops(routePlan.RoutePlanID, routePlan.RouteStops);

        //        if (stopsHandled)
        //        {
        //            return new ServiceResponse<string>(true, "Operation Successful", "Route plan added/updated successfully", StatusCodes.Status200OK);
        //        }
        //        else
        //        {
        //            return new ServiceResponse<string>(false, "Operation Failed", "Error handling route stops", StatusCodes.Status400BadRequest);
        //        }
        //    }
        //    else
        //    {
        //        return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating route plan", StatusCodes.Status400BadRequest);
        //    }
        //}


        public async Task<ServiceResponse<IEnumerable<RoutePlanResponseDTO>>> GetAllRoutePlans(GetAllRoutePlanRequest request)
        {
            try
            {
                // Count total records for pagination, searching by VehicleNumber and RouteName
                string countSql = @"
                SELECT COUNT(*) 
                FROM tblRoutePlan rp
                JOIN tblVehicleMaster v ON rp.VehicleID = v.VehicleID
                WHERE rp.IsActive = 1 
                AND rp.InstituteID = @InstituteID
                AND (v.VehicleNumber LIKE @SearchTerm OR rp.RouteName LIKE @SearchTerm)";

                int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql, new
                {
                    InstituteID = request.InstituteId,
                    SearchTerm = $"%{request.SearchTerm}%"
                });

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
                    AND (v.VehicleNumber LIKE @SearchTerm OR rp.RouteName LIKE @SearchTerm)
                ORDER BY 
                    rp.RoutePlanID 
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var routePlans = (await _dbConnection.QueryAsync<RoutePlanResponseDTO>(sql, new
                {
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize,
                    InstituteID = request.InstituteId,
                    SearchTerm = $"%{request.SearchTerm}%"
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

        public async Task<ServiceResponse<IEnumerable<RoutePlanResponseDTOExport>>> FetchRoutePlansForExport(GetAllRoutePlanExportRequest request)
        {
            var query = @"
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
                AND (v.VehicleNumber LIKE @SearchTerm OR rp.RouteName LIKE @SearchTerm)
            ORDER BY 
                rp.RoutePlanID";

            var routePlans = await _dbConnection.QueryAsync<RoutePlanResponseDTOExport>(query, new
            {
                InstituteID = request.InstituteId,
                SearchTerm = $"%{request.SearchTerm}%"
            });

            return new ServiceResponse<IEnumerable<RoutePlanResponseDTOExport>>(true, "Route plans fetched", routePlans, 200);
        }



        //public async Task<ServiceResponse<IEnumerable<RoutePlanResponseDTO>>> GetAllRoutePlans(GetAllRoutePlanRequest request)
        //{
        //    try
        //    {
        //        // Count total records for pagination
        //        string countSql = @"SELECT COUNT(*) FROM tblRoutePlan WHERE IsActive = 1 AND InstituteID = @InstituteID";
        //        int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql, new { InstituteID = request.InstituteId });

        //        // Validate if there are any records
        //        if (totalCount == 0)
        //        {
        //            return new ServiceResponse<IEnumerable<RoutePlanResponseDTO>>(false, $"No route plans found for InstituteID: {request.InstituteId}", new List<RoutePlanResponseDTO>(), StatusCodes.Status204NoContent);
        //        }

        //        // Main SQL to retrieve paginated route plans, including the count of routeStops
        //        string sql = @"
        //        SELECT 
        //            rp.RoutePlanID, 
        //            rp.RouteName, 
        //            rp.VehicleID, 
        //            v.VehicleNumber, 
        //            (SELECT COUNT(*) FROM tblRouteStopMaster rs WHERE rs.RoutePlanID = rp.RoutePlanID) AS NoOfStops,
        //            (SELECT MIN(PickUpTime) FROM tblRouteStopMaster WHERE RoutePlanID = rp.RoutePlanID) AS PickUpTime,
        //            (SELECT MAX(DropTime) FROM tblRouteStopMaster WHERE RoutePlanID = rp.RoutePlanID) AS DropTime,
        //            ISNULL(CONCAT(e.First_Name, ' ', e.Last_Name), '') AS DriverName
        //        FROM 
        //            tblRoutePlan rp
        //            JOIN tblVehicleMaster v ON rp.VehicleID = v.VehicleID
        //            LEFT JOIN tbl_EmployeeProfileMaster e ON v.AssignDriverID = e.Employee_id
        //        WHERE 
        //            rp.IsActive = 1 
        //            AND rp.InstituteID = @InstituteID
        //        ORDER BY 
        //            rp.RoutePlanID 
        //        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        //        var routePlans = (await _dbConnection.QueryAsync<RoutePlanResponseDTO>(sql, new
        //        {
        //            Offset = (request.PageNumber - 1) * request.PageSize,
        //            PageSize = request.PageSize,
        //            InstituteID = request.InstituteId
        //        })).ToList();

        //        // Check if the result set is empty
        //        if (routePlans == null || !routePlans.Any())
        //        {
        //            return new ServiceResponse<IEnumerable<RoutePlanResponseDTO>>(false, $"No route plans found for InstituteID: {request.InstituteId}", new List<RoutePlanResponseDTO>(), StatusCodes.Status204NoContent);
        //        }

        //        // Return the retrieved route plans with routeStops count
        //        return new ServiceResponse<IEnumerable<RoutePlanResponseDTO>>(true, "Records Found", routePlans, StatusCodes.Status200OK, totalCount);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception if needed and return the error response
        //        return new ServiceResponse<IEnumerable<RoutePlanResponseDTO>>(false, ex.Message, new List<RoutePlanResponseDTO>(), StatusCodes.Status500InternalServerError);
        //    }
        //}


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
                    // Return the response without `totalCount`
                    return new ServiceResponse<RoutePlanResponseDTO>(true, "Record Found", routePlan, StatusCodes.Status200OK);
                }
                else
                {
                    // Provide a response when no record is found
                    return new ServiceResponse<RoutePlanResponseDTO>(false, $"No route plan found for RoutePlanID: {routePlanID}", null, StatusCodes.Status200OK);
                }
            }
            catch (Exception ex)
            {
                // Return a failed response in case of an error
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
                    // Fetch existing stops for this RoutePlanID
                    string fetchExistingStopsSql = @"SELECT StopID FROM tblRouteStopMaster WHERE RoutePlanID = @RoutePlanID";
                    var existingStopIDs = (await _dbConnection.QueryAsync<int>(fetchExistingStopsSql, new { RoutePlanID = routePlanID }, transaction)).ToList();

                    // Iterate through new route stops from request
                    foreach (var stop in routeStops)
                    {
                        stop.RoutePlanID = routePlanID;

                        if (stop.StopID == 0)
                        {
                            // New stop, insert and get StopID
                            string insertStopSql = @"INSERT INTO tblRouteStopMaster (RoutePlanID, StopName, PickUpTime, DropTime, FeeAmount) 
                                             VALUES (@RoutePlanID, @StopName, @PickUpTime, @DropTime, @FeeAmount);
                                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
                            stop.StopID = await _dbConnection.QuerySingleAsync<int>(insertStopSql, stop, transaction);
                        }
                        else
                        {
                            // Existing stop, update it
                            string updateStopSql = @"UPDATE tblRouteStopMaster 
                                             SET StopName = @StopName, PickUpTime = @PickUpTime, DropTime = @DropTime, FeeAmount = @FeeAmount 
                                             WHERE StopID = @StopID";
                            await _dbConnection.ExecuteAsync(updateStopSql, stop, transaction);

                            // Remove the updated stop from the existing list (so we can delete the remaining ones later)
                            existingStopIDs.Remove(stop.StopID);
                        }

                        // Call the method to delete any existing payments before adding new ones
                        await DeleteExistingPayments(routePlanID, stop.StopID, transaction);

                        // Handle Single Payments
                        if (stop.SinglePayment != null)
                        {
                            foreach (var singlePayment in stop.SinglePayment)
                            {
                                singlePayment.RoutePlanID = routePlanID;
                                singlePayment.StopID = stop.StopID;

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
                                termPayment.StopID = stop.StopID;

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
                                monthlyPayment.StopID = stop.StopID;

                                string insertMonthlyPaymentSql = @"INSERT INTO tblRouteMonthlyFeesPayment (RoutePlanID, StopID, MonthName, FeesAmount)
                                                   VALUES (@RoutePlanID, @StopID, @MonthName, @FeesAmount)";
                                await _dbConnection.ExecuteAsync(insertMonthlyPaymentSql, monthlyPayment, transaction);
                            }
                        }
                    }

                    // Delete any remaining stops that were not part of the new stops
                    if (existingStopIDs.Any())
                    {
                        foreach (var stopID in existingStopIDs)
                        {
                            // Delete the stop and its associated payments
                            await DeleteExistingPayments(routePlanID, stopID, transaction);

                            string deleteStopSql = @"DELETE FROM tblRouteStopMaster WHERE StopID = @StopID";
                            await _dbConnection.ExecuteAsync(deleteStopSql, new { StopID = stopID }, transaction);
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



        private async Task DeleteExistingPayments(int routePlanID, int stopID, IDbTransaction transaction)
        {
            // Delete existing Single Payments
            string deleteSinglePaymentSql = @"DELETE FROM tblRouteSingleFeesPayment WHERE RoutePlanID = @RoutePlanID AND StopID = @StopID";
            await _dbConnection.ExecuteAsync(deleteSinglePaymentSql, new { RoutePlanID = routePlanID, StopID = stopID }, transaction);

            // Delete existing Term Payments
            string deleteTermPaymentSql = @"DELETE FROM tblRouteTermFeesPayment WHERE RoutePlanID = @RoutePlanID AND StopID = @StopID";
            await _dbConnection.ExecuteAsync(deleteTermPaymentSql, new { RoutePlanID = routePlanID, StopID = stopID }, transaction);

            // Delete existing Monthly Payments
            string deleteMonthlyPaymentSql = @"DELETE FROM tblRouteMonthlyFeesPayment WHERE RoutePlanID = @RoutePlanID AND StopID = @StopID";
            await _dbConnection.ExecuteAsync(deleteMonthlyPaymentSql, new { RoutePlanID = routePlanID, StopID = stopID }, transaction);
        }




        //private async Task<List<RouteStopResponse>> GetRouteStopsByRoutePlanID(int routePlanID)
        //{
        //    string sql = @"SELECT StopID, RoutePlanID, StopName, PickUpTime, DropTime, FeeAmount
        //           FROM tblRouteStopMaster
        //           WHERE RoutePlanID = @RoutePlanID";

        //    return (await _dbConnection.QueryAsync<RouteStopResponse>(sql, new { RoutePlanID = routePlanID })).ToList();
        //}

        public async Task<ServiceResponse<RouteDetailsResponseDTO>> GetRouteDetails(GetRouteDetailsRequest request)
        {
            try
            {
                // SQL to retrieve the route plan details
                string routeSql = @"
            SELECT 
                rp.RoutePlanID, 
                rp.RouteName
            FROM 
                tblRoutePlan rp
            WHERE 
                rp.RoutePlanID = @RouteID
                AND rp.InstituteID = @InstituteID
                AND rp.IsActive = 1";

                // SQL to retrieve the stops associated with the route
                string stopsSql = @"
            SELECT 
                rs.StopName, 
                rs.PickUpTime, 
                rs.DropTime, 
                rs.FeeAmount AS Fee
            FROM 
                tblRouteStopMaster rs
            WHERE 
                rs.RoutePlanID = @RouteID";

                // First, fetch the route plan details
                var routeDetails = await _dbConnection.QueryFirstOrDefaultAsync<RouteDetailsResponseDTO>(routeSql, new
                {
                    RouteID = request.RouteID,
                    InstituteID = request.InstituteID
                });

                // If the route exists, fetch its stops
                if (routeDetails != null)
                {
                    // Fetch the stops for the given RoutePlanID
                    var stops = await _dbConnection.QueryAsync<RouteStopResponseDTO>(stopsSql, new
                    {
                        RouteID = request.RouteID
                    });

                    // Attach the stops to the route details
                    routeDetails.Stops = stops.ToList();

                    // Return the response
                    return new ServiceResponse<RouteDetailsResponseDTO>(true, "Record Found", routeDetails, StatusCodes.Status200OK);
                }
                else
                {
                    // Return a no content response if no route plan is found
                    return new ServiceResponse<RouteDetailsResponseDTO>(false, "No records found", null, StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                // Return an error response in case of exceptions
                return new ServiceResponse<RouteDetailsResponseDTO>(false, ex.Message, null, StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ServiceResponse<byte[]>> GetRouteDetailsExportExcel(GetRouteDetailsRequest request)
        {
            try
            {
                // SQL to retrieve the route plan details (without RoutePlanID in the Excel export)
                string routeSql = @"
            SELECT 
                rp.RouteName
            FROM 
                tblRoutePlan rp
            WHERE 
                rp.RoutePlanID = @RouteID
                AND rp.InstituteID = @InstituteID
                AND rp.IsActive = 1";

                // SQL to retrieve the stops associated with the route
                string stopsSql = @"
            SELECT 
                rs.StopName, 
                rs.PickUpTime, 
                rs.DropTime, 
                rs.FeeAmount AS Fee
            FROM 
                tblRouteStopMaster rs
            WHERE 
                rs.RoutePlanID = @RouteID";

                // Fetch the route plan details
                var routeDetails = await _dbConnection.QueryFirstOrDefaultAsync<RouteDetailsResponseDTO>(routeSql, new
                {
                    RouteID = request.RouteID,
                    InstituteID = request.InstituteID
                });

                if (routeDetails == null)
                {
                    return new ServiceResponse<byte[]>(false, "No records found", null, StatusCodes.Status204NoContent);
                }

                // Fetch the stops for the given RoutePlanID
                var stops = await _dbConnection.QueryAsync<RouteStopResponseDTO>(stopsSql, new
                {
                    RouteID = request.RouteID
                });

                // Create the Excel file
                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("RouteDetails");

                    // Add the RouteName as a title in the Excel sheet
                    worksheet.Cells[1, 1].Value = "Route Name";
                    worksheet.Cells[1, 2].Value = routeDetails.RouteName;

                    // Add headers for the stops data
                    worksheet.Cells[3, 1].Value = "Stop Name";
                    worksheet.Cells[3, 2].Value = "PickUp Time";
                    worksheet.Cells[3, 3].Value = "Drop Time";
                    worksheet.Cells[3, 4].Value = "Fee";

                    // Add the data for each stop
                    var rowIndex = 4;
                    foreach (var stop in stops)
                    {
                        worksheet.Cells[rowIndex, 1].Value = stop.StopName;
                        worksheet.Cells[rowIndex, 2].Value = stop.PickUpTime;
                        worksheet.Cells[rowIndex, 3].Value = stop.DropTime;
                        worksheet.Cells[rowIndex, 4].Value = stop.Fee.ToString();
                        rowIndex++;
                    }

                    // Auto-fit the columns
                    worksheet.Cells.AutoFitColumns();

                    // Convert the Excel package to a byte array
                    var excelFile = package.GetAsByteArray();

                    return new ServiceResponse<byte[]>(true, "Excel file generated successfully", excelFile, StatusCodes.Status200OK);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return new ServiceResponse<byte[]>(false, ex.Message, null, StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetRoutePlanVehiclesResponse>>> GetRoutePlanVehicles(int instituteID)
        {
            string sql = @"
            SELECT VehicleID, VehicleNumber 
            FROM tblVehicleMaster 
            WHERE InstituteID = @InstituteID AND IsActive = 1";

            var vehicles = await _dbConnection.QueryAsync<GetRoutePlanVehiclesResponse>(sql, new { InstituteID = instituteID });

            if (vehicles == null || !vehicles.Any())
            {
                return new ServiceResponse<IEnumerable<GetRoutePlanVehiclesResponse>>(false, "No vehicles found for the given InstituteID", new List<GetRoutePlanVehiclesResponse>(), StatusCodes.Status204NoContent);
            }

            return new ServiceResponse<IEnumerable<GetRoutePlanVehiclesResponse>>(true, "Vehicles fetched successfully", vehicles, StatusCodes.Status200OK);
        }



    }
}
