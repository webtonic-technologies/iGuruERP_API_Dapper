using Dapper;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Numerics;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.Responses;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
using Transport_API.Repository.Interfaces;

namespace Transport_API.Repository.Implementations
{
    public class RouteMappingRepository : IRouteMappingRepository
    {
        private readonly IDbConnection _dbConnection;

        public RouteMappingRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }


        public async Task<ServiceResponse<string>> AddUpdateRouteMapping(RouteMappingRequest routeMapping)
        {
            // Ensure the connection is open
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    string sql;
                    if (routeMapping.AssignRouteID == 0)
                    {
                        sql = @"INSERT INTO tblAssignRoute (RoutePlanID, VehicleID, DriverID, TransportStaffID, IsActive) 
                        VALUES (@RoutePlanID, @VehicleID, @DriverID, @TransportStaffID, @IsActive);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        // Insert into tblAssignRoute and get the newly inserted AssignRouteID
                        routeMapping.AssignRouteID = await _dbConnection.QuerySingleAsync<int>(sql, routeMapping, transaction);
                    }
                    else
                    {
                        sql = @"UPDATE tblAssignRoute SET RoutePlanID = @RoutePlanID, VehicleID = @VehicleID, DriverID = @DriverID, TransportStaffID = @TransportStaffID, IsActive = @IsActive
                        WHERE AssignRouteID = @AssignRouteID";

                        // Update tblAssignRoute
                        await _dbConnection.ExecuteAsync(sql, routeMapping, transaction);
                    }

                    // Handle Student Stop Mappings
                    if (routeMapping.StudentIDs != null && routeMapping.StudentIDs.Any())
                    {
                        // Delete existing mappings for the given StopID
                        string deleteStudentMappingSql = "DELETE FROM tblStudentStopMapping WHERE StopID = @StopID";
                        await _dbConnection.ExecuteAsync(deleteStudentMappingSql, new { routeMapping.StopID }, transaction);

                        // Insert new mappings
                        string insertStudentMappingSql = @"INSERT INTO tblStudentStopMapping (StopID, StudentID) VALUES (@StopID, @StudentID)";
                        foreach (var studentID in routeMapping.StudentIDs)
                        {
                            await _dbConnection.ExecuteAsync(insertStudentMappingSql, new { StopID = routeMapping.StopID, StudentID = studentID }, transaction);
                        }
                    }

                    // Handle Employee Stop Mappings
                    if (routeMapping.EmployeeIDs != null && routeMapping.EmployeeIDs.Any())
                    {
                        // Delete existing mappings for the given StopID
                        string deleteEmployeeMappingSql = "DELETE FROM tblEmployeeStopMapping WHERE StopID = @StopID";
                        await _dbConnection.ExecuteAsync(deleteEmployeeMappingSql, new { routeMapping.StopID }, transaction);

                        // Insert new mappings
                        string insertEmployeeMappingSql = @"INSERT INTO tblEmployeeStopMapping (StopID, EmployeeID) VALUES (@StopID, @EmployeeID)";
                        foreach (var employeeID in routeMapping.EmployeeIDs)
                        {
                            await _dbConnection.ExecuteAsync(insertEmployeeMappingSql, new { StopID = routeMapping.StopID, EmployeeID = employeeID }, transaction);
                        }
                    }

                    // Commit transaction if all queries succeeded
                    transaction.Commit();
                    return new ServiceResponse<string>(true, "Operation Successful", "Route mapping added/updated successfully", StatusCodes.Status200OK);
                }
                catch (Exception ex)
                {
                    // Rollback transaction in case of an error
                    transaction.Rollback();
                    return new ServiceResponse<string>(false, ex.Message, "Error adding/updating route mapping", StatusCodes.Status500InternalServerError);
                }
                finally
                {
                    // Ensure the connection is closed
                    _dbConnection.Close();
                }
            }
        }

        //public async Task<ServiceResponse<IEnumerable<RouteMappingResponse>>> GetAllRouteMappings(GetAllRouteMappingRequest request)
        //{
        //    try
        //    {
        //        string countSql = @"
        //    SELECT COUNT(DISTINCT rp.RoutePlanID)
        //    FROM tblRoutePlan rp
        //    JOIN tblAssignRoute ar ON rp.RoutePlanID = ar.RoutePlanID
        //    WHERE rp.IsActive = 1 AND rp.InstituteID = @InstituteID;
        //";

        //        int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

        //        string sql = @"
        //    SELECT 
        //        rp.RoutePlanID AS RouteMappingId, 
        //        rp.RouteName, 
        //        v.VehicleID,
        //        v.VehicleNumber, 
        //        ISNULL(CONCAT(e.First_Name, ' ', e.Last_Name), '') AS DriverName, 
        //        ISNULL(CONCAT(ts.First_Name, ' ', ts.Last_Name), '') AS TransportStaffName,
        //        (SELECT COUNT(*) 
        //         FROM tblStudentStopMapping ssm 
        //         JOIN tblRouteStopMaster rsm ON ssm.StopID = rsm.StopID 
        //         WHERE rsm.RoutePlanID = rp.RoutePlanID) AS TotalStudents,
        //        (SELECT COUNT(*) 
        //         FROM tblEmployeeStopMapping esm 
        //         JOIN tblRouteStopMaster rsm ON esm.StopID = rsm.StopID 
        //         WHERE rsm.RoutePlanID = rp.RoutePlanID) AS TotalEmployees,
        //        (v.SeatingCapacity - 
        //            (
        //                (SELECT COUNT(*) 
        //                 FROM tblStudentStopMapping ssm 
        //                 JOIN tblRouteStopMaster rsm ON ssm.StopID = rsm.StopID 
        //                 WHERE rsm.RoutePlanID = rp.RoutePlanID) + 
        //                (SELECT COUNT(*) 
        //                 FROM tblEmployeeStopMapping esm 
        //                 JOIN tblRouteStopMaster rsm ON esm.StopID = rsm.StopID 
        //                 WHERE rsm.RoutePlanID = rp.RoutePlanID)
        //            )
        //        ) AS Availability
        //    FROM 
        //        tblRoutePlan rp
        //        JOIN tblAssignRoute ar ON rp.RoutePlanID = ar.RoutePlanID
        //        JOIN tblVehicleMaster v ON ar.VehicleID = v.VehicleID
        //        LEFT JOIN tbl_EmployeeProfileMaster e ON ar.DriverID = e.Employee_id
        //        LEFT JOIN tbl_EmployeeProfileMaster ts ON ar.TransportStaffID = ts.Employee_id
        //    WHERE 
        //        rp.IsActive = 1  AND ar.IsActive = 1 
        //        AND rp.InstituteID = @InstituteID
        //    GROUP BY 
        //        rp.RoutePlanID, rp.RouteName, v.VehicleID, v.VehicleNumber, e.First_Name, e.Last_Name, ts.First_Name, ts.Last_Name, v.SeatingCapacity
        //    ORDER BY 
        //        rp.RoutePlanID  
        //    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        //        var routeMappings = await _dbConnection.QueryAsync<RouteMappingResponse>(sql, new
        //        {
        //            Offset = (request.PageNumber - 1) * request.PageSize,
        //            PageSize = request.PageSize,
        //            InstituteID = request.InstituteID
        //        });

        //        if (routeMappings != null && routeMappings.Any())
        //        {
        //            return new ServiceResponse<IEnumerable<RouteMappingResponse>>(true, "Records found successfully.", routeMappings, StatusCodes.Status200OK, totalCount);
        //        }
        //        else
        //        {
        //            // Instead of returning a 204, we now return a 200 with a clear message that no data was found.
        //            return new ServiceResponse<IEnumerable<RouteMappingResponse>>(false, "No route mappings found for the provided InstituteID.", new List<RouteMappingResponse>(), StatusCodes.Status200OK);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception for debugging
        //        return new ServiceResponse<IEnumerable<RouteMappingResponse>>(false, "An error occurred while fetching route mappings. Please try again later.", new List<RouteMappingResponse>(), StatusCodes.Status500InternalServerError);
        //    }
        //}

        public async Task<ServiceResponse<IEnumerable<RouteMappingResponse>>> GetAllRouteMappings(GetAllRouteMappingRequest request)
        {
            try
            {
                // SQL to get total count of the route mappings
                string countSql = @"
            SELECT COUNT(DISTINCT rp.RoutePlanID)
            FROM tblRoutePlan rp
            JOIN tblAssignRoute ar ON rp.RoutePlanID = ar.RoutePlanID
            WHERE rp.IsActive = 1 AND rp.InstituteID = @InstituteID;
        ";

                int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                // Updated SQL query to get route details, including total students, employees, and availability
                string sql = @"
            SELECT 
                rp.RoutePlanID AS RouteMappingId, 
                rp.RouteName, 
                v.VehicleID,
                v.VehicleNumber, 
                ISNULL(CONCAT(e.First_Name, ' ', e.Last_Name), '') AS DriverName, 
                ISNULL(CONCAT(ts.First_Name, ' ', ts.Last_Name), '') AS TransportStaffName,
                -- Subquery to count total students assigned to the route's stops
                (SELECT COUNT(*) 
                 FROM tblStudentStopMapping ssm 
                 JOIN tblRouteStopMaster rsm ON ssm.StopID = rsm.StopID 
                 WHERE rsm.RoutePlanID = rp.RoutePlanID) AS TotalStudents,
                -- Subquery to count total employees assigned to the route's stops
                (SELECT COUNT(*) 
                 FROM tblEmployeeStopMapping esm 
                 JOIN tblRouteStopMaster rsm ON esm.StopID = rsm.StopID 
                 WHERE rsm.RoutePlanID = rp.RoutePlanID) AS TotalEmployees,
                -- Calculation of available seats
                (v.SeatingCapacity - 
                    (
                        (SELECT COUNT(*) 
                         FROM tblStudentStopMapping ssm 
                         JOIN tblRouteStopMaster rsm ON ssm.StopID = rsm.StopID 
                         WHERE rsm.RoutePlanID = rp.RoutePlanID) + 
                        (SELECT COUNT(*) 
                         FROM tblEmployeeStopMapping esm 
                         JOIN tblRouteStopMaster rsm ON esm.StopID = rsm.StopID 
                         WHERE rsm.RoutePlanID = rp.RoutePlanID)
                    )
                ) AS Availability
            FROM 
                tblRoutePlan rp
                JOIN tblAssignRoute ar ON rp.RoutePlanID = ar.RoutePlanID
                JOIN tblVehicleMaster v ON ar.VehicleID = v.VehicleID
                LEFT JOIN tbl_EmployeeProfileMaster e ON ar.DriverID = e.Employee_id
                LEFT JOIN tbl_EmployeeProfileMaster ts ON ar.TransportStaffID = ts.Employee_id
            WHERE 
                rp.IsActive = 1 
                AND ar.IsActive = 1 
                AND rp.InstituteID = @InstituteID
            GROUP BY 
                rp.RoutePlanID, 
                rp.RouteName, 
                v.VehicleID, 
                v.VehicleNumber, 
                e.First_Name, 
                e.Last_Name, 
                ts.First_Name, 
                ts.Last_Name, 
                v.SeatingCapacity
            ORDER BY 
                rp.RoutePlanID  
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                // Execute the query to fetch the route mappings with the necessary details
                var routeMappings = await _dbConnection.QueryAsync<RouteMappingResponse>(sql, new
                {
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize,
                    InstituteID = request.InstituteID
                });

                // Check if any records are found
                if (routeMappings != null && routeMappings.Any())
                {
                    return new ServiceResponse<IEnumerable<RouteMappingResponse>>(true, "Records found successfully.", routeMappings, StatusCodes.Status200OK, totalCount);
                }
                else
                {
                    // If no records are found, return an appropriate response
                    return new ServiceResponse<IEnumerable<RouteMappingResponse>>(false, "No route mappings found for the provided InstituteID.", new List<RouteMappingResponse>(), StatusCodes.Status200OK);
                }
            }
            catch (Exception ex)
            {
                // Return error response if an exception occurs
                return new ServiceResponse<IEnumerable<RouteMappingResponse>>(false, "An error occurred while fetching route mappings. Please try again later.", new List<RouteMappingResponse>(), StatusCodes.Status500InternalServerError);
            }
        }


        //public async Task<ServiceResponse<RouteMappingResponse>> GetRouteMappingById(int AssignRouteID)
        //{
        //    try
        //    {
        //        string sql = @"
        //    SELECT ar.AssignRouteID as RouteMappingId, 
        //           rp.RouteName, 
        //           v.VehicleID, 
        //           v.VehicleNumber, 
        //           e.Employee_id as EmployeeId,
        //           ISNULL(CONCAT(e.First_Name, ' ', e.Last_Name), '') AS DriverName,
        //           ts.Employee_id as TransportStaffID,
        //           ISNULL(CONCAT(ts.First_Name, ' ', ts.Last_Name), '') AS TransportStaffName,
        //           (SELECT COUNT(*) FROM tblStudentStopMapping srm WHERE srm.RoutePlanID = rp.RoutePlanID) AS TotalStudents,
        //           (SELECT COUNT(*) FROM tblEmployeeStopMapping erm WHERE erm.RoutePlanID = rp.RoutePlanID) AS TotalEmployees
        //    FROM tblAssignRoute ar
        //    JOIN tblRoutePlan rp ON ar.RoutePlanID = rp.RoutePlanID
        //    JOIN tblVehicleMaster v ON ar.VehicleID = v.VehicleID
        //    LEFT JOIN tbl_EmployeeProfileMaster e ON v.AssignDriverID = e.Employee_id
        //    LEFT JOIN tbl_EmployeeProfileMaster ts ON ar.TransportStaffID = ts.Employee_id
        //    WHERE ar.AssignRouteID = @AssignRouteID
        //    AND ar.IsActive = 1";

        //        var routeMapping = await _dbConnection.QueryFirstOrDefaultAsync<RouteMappingResponse>(sql, new { AssignRouteID });

        //        if (routeMapping != null)
        //        {
        //            return new ServiceResponse<RouteMappingResponse>(true, "Record Found", routeMapping, StatusCodes.Status200OK);
        //        }
        //        else
        //        {
        //            return new ServiceResponse<RouteMappingResponse>(false, "Record Not Found", new RouteMappingResponse(), StatusCodes.Status204NoContent);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<RouteMappingResponse>(false, ex.Message, new RouteMappingResponse(), StatusCodes.Status500InternalServerError);
        //    }
        //}

        public async Task<ServiceResponse<RouteMappingResponse>> GetRouteMappingById(int AssignRouteID)
        {
            try
            {
                string sql = @"
            SELECT 
                ar.AssignRouteID as RouteMappingId, 
                rp.RouteName, 
                v.VehicleID, 
                v.VehicleNumber, 
                e.Employee_id as EmployeeId,
                ISNULL(CONCAT(e.First_Name, ' ', e.Last_Name), '') AS DriverName,
                ts.Employee_id as TransportStaffID,
                ISNULL(CONCAT(ts.First_Name, ' ', ts.Last_Name), '') AS TransportStaffName,
                -- Subquery to count total students assigned to the route's stops
                (SELECT COUNT(*) 
                 FROM tblStudentStopMapping srm 
                 JOIN tblRouteStopMaster rs ON srm.StopID = rs.StopID
                 WHERE rs.RoutePlanID = rp.RoutePlanID) AS TotalStudents,
                -- Subquery to count total employees assigned to the route's stops
                (SELECT COUNT(*) 
                 FROM tblEmployeeStopMapping erm 
                 JOIN tblRouteStopMaster rs ON erm.StopID = rs.StopID
                 WHERE rs.RoutePlanID = rp.RoutePlanID) AS TotalEmployees
            FROM tblAssignRoute ar
            JOIN tblRoutePlan rp ON ar.RoutePlanID = rp.RoutePlanID
            JOIN tblVehicleMaster v ON ar.VehicleID = v.VehicleID
            LEFT JOIN tbl_EmployeeProfileMaster e ON v.AssignDriverID = e.Employee_id
            LEFT JOIN tbl_EmployeeProfileMaster ts ON ar.TransportStaffID = ts.Employee_id
            WHERE ar.AssignRouteID = @AssignRouteID
            AND ar.IsActive = 1";

                // Execute the query with the AssignRouteID as parameter
                var routeMapping = await _dbConnection.QueryFirstOrDefaultAsync<RouteMappingResponse>(sql, new { AssignRouteID });

                // If route mapping is found, return it with a success message
                if (routeMapping != null)
                {
                    return new ServiceResponse<RouteMappingResponse>(true, "Record Found", routeMapping, StatusCodes.Status200OK);
                }
                else
                {
                    // If no route mapping is found, return a no content response
                    return new ServiceResponse<RouteMappingResponse>(false, "Record Not Found", new RouteMappingResponse(), StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                // Return an error response if an exception occurs
                return new ServiceResponse<RouteMappingResponse>(false, ex.Message, new RouteMappingResponse(), StatusCodes.Status500InternalServerError);
            }
        }



        public async Task<ServiceResponse<bool>> UpdateRouteMappingStatus(int AssignRouteID)
        {
            string sql = @"UPDATE tblAssignRoute SET IsActive = @IsActive WHERE AssignRouteID = @AssignRouteID";
            var result = await _dbConnection.ExecuteAsync(sql, new { IsActive = false, AssignRouteID = AssignRouteID });

            if (result > 0)
            {
                return new ServiceResponse<bool>(true, "Status Updated Successfully", true, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<bool>(false, "Status Update Failed", false, StatusCodes.Status400BadRequest);
            }
        }
        public async Task<ServiceResponse<string>> AddUpdateStudentStopMapping(List<StudentStopMapping> request)
        {
            try
            {
                foreach (var mapping in request)
                {
                    // Check if the mapping exists
                    string checkSql = "SELECT COUNT(1) FROM tblStudentStopMapping WHERE StudentStopID = @StudentStopID";
                    var exists = await _dbConnection.ExecuteScalarAsync<bool>(checkSql, new { mapping.StudentStopID });

                    if (exists)
                    {
                        // Update the existing mapping
                        string updateSql = @"
                    UPDATE tblStudentStopMapping
                    SET StopID = @StopID, StudentID = @StudentID
                    WHERE StudentStopID = @StudentStopID";
                        await _dbConnection.ExecuteAsync(updateSql, mapping);
                    }
                    else
                    {
                        // Insert new mapping
                        string insertSql = @"
                    INSERT INTO tblStudentStopMapping (StopID, StudentID)
                    VALUES (@StopID, @StudentID)";
                        await _dbConnection.ExecuteAsync(insertSql, mapping);
                    }
                }

                return new ServiceResponse<string>(true, "Operation Successful", "Student stop mappings added/updated successfully", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error adding/updating student stop mappings", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<ServiceResponse<string>> AddUpdateEmployeeStopMapping(List<EmployeeStopMapping> request)
        {
            try
            {
                foreach (var mapping in request)
                {
                    // Check if the mapping exists
                    string checkSql = "SELECT COUNT(1) FROM tblEmployeeStopMapping WHERE EmployeeStopID = @EmployeeStopID";
                    var exists = await _dbConnection.ExecuteScalarAsync<bool>(checkSql, new { mapping.EmployeeStopID });

                    if (exists)
                    {
                        // Update the existing mapping
                        string updateSql = @"
                    UPDATE tblEmployeeStopMapping
                    SET StopID = @StopID, EmployeeID = @EmployeeID
                    WHERE EmployeeStopID = @EmployeeStopID";
                        await _dbConnection.ExecuteAsync(updateSql, mapping);
                    }
                    else
                    {
                        // Insert new mapping
                        string insertSql = @"
                    INSERT INTO tblEmployeeStopMapping (StopID, EmployeeID)
                    VALUES (@StopID, @EmployeeID)";
                        await _dbConnection.ExecuteAsync(insertSql, mapping);
                    }
                }

                return new ServiceResponse<string>(true, "Operation Successful", "Employee stop mappings added/updated successfully", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error adding/updating employee stop mappings", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<ServiceResponse<string>> RemoveEmployeeStopMapping(List<EmployeeStopMapping> request)
        {
            try
            {
                if (request == null || !request.Any())
                {
                    return new ServiceResponse<string>(false, "No records provided", "No mappings to remove", StatusCodes.Status400BadRequest);
                }

                string deleteSql = "DELETE FROM tblEmployeeStopMapping WHERE EmployeeStopID = @EmployeeStopID";

                foreach (var mapping in request)
                {
                    await _dbConnection.ExecuteAsync(deleteSql, new { mapping.EmployeeStopID });
                }

                return new ServiceResponse<string>(true, "Operation Successful", "Employee stop mappings removed successfully", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error removing employee stop mappings", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<ServiceResponse<string>> RemoveStudentStopMapping(List<StudentStopMapping> request)
        {
            try
            {
                if (request == null || !request.Any())
                {
                    return new ServiceResponse<string>(false, "No records provided", "No mappings to remove", StatusCodes.Status400BadRequest);
                }

                string deleteSql = "DELETE FROM tblStudentStopMapping WHERE StudentStopID = @StudentStopID";

                foreach (var mapping in request)
                {
                    await _dbConnection.ExecuteAsync(deleteSql, new { mapping.StudentStopID });
                }

                return new ServiceResponse<string>(true, "Operation Successful", "Student stop mappings removed successfully", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error removing student stop mappings", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<ServiceResponse<List<StudentStopMappingResponse>>> GetStudentStopMappings(int RoutePlanId)
        {
            try
            {
                string sql = @"
            SELECT ssm.StudentStopID, ssm.StopID, sm.student_id AS StudentID, 
                   ISNULL(CONCAT(sm.First_Name, ' ', sm.Last_Name), '') AS StudentName,
                   sm.Roll_Number AS RollNumber
            FROM tblStudentStopMapping ssm
            JOIN tbl_StudentMaster sm ON ssm.StudentID = sm.student_id
            WHERE sm.isActive = 1";

                //WHERE ssm.RoutePlanID = @RoutePlanId AND sm.isActive = 1";

                var studentStopMappings = (await _dbConnection.QueryAsync<StudentStopMappingResponse>(sql, new { RoutePlanId })).ToList();

                if (studentStopMappings.Any())
                {
                    return new ServiceResponse<List<StudentStopMappingResponse>>(true, "Records Found", studentStopMappings, StatusCodes.Status200OK);
                }
                else
                {
                    return new ServiceResponse<List<StudentStopMappingResponse>>(false, "No Records Found", new List<StudentStopMappingResponse>(), StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentStopMappingResponse>>(false, ex.Message, new List<StudentStopMappingResponse>(), StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<ServiceResponse<List<EmployeeStopMappingResponse>>> GetEmployeeStopMappings(int RoutePlanId)
        {
            try
            {
                string sql = @"
            SELECT esm.EmployeeStopID, esm.StopID, epm.Employee_id AS EmployeeID, 
                   ISNULL(CONCAT(epm.First_Name, ' ', epm.Last_Name), '') AS EmployeeName,
                   epm.Designation_id AS DesignationId,
                   d.DesignationName
            FROM tblEmployeeStopMapping esm
            JOIN tbl_EmployeeProfileMaster epm ON esm.EmployeeID = epm.Employee_id
            LEFT JOIN tbl_Designation d ON epm.Designation_id = d.Designation_id
            WHERE epm.isActive = 1";
            
                
                //WHERE esm.RoutePlanID = @RoutePlanId AND epm.isActive = 1";

                var employeeStopMappings = (await _dbConnection.QueryAsync<EmployeeStopMappingResponse>(sql, new { RoutePlanId })).ToList();

                if (employeeStopMappings.Any())
                {
                    return new ServiceResponse<List<EmployeeStopMappingResponse>>(true, "Records Found", employeeStopMappings, StatusCodes.Status200OK);
                }
                else
                {
                    return new ServiceResponse<List<EmployeeStopMappingResponse>>(false, "No Records Found", new List<EmployeeStopMappingResponse>(), StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeStopMappingResponse>>(false, ex.Message, new List<EmployeeStopMappingResponse>(), StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<RouteVehicleDriverInfoResponse> GetRouteVehicleDriverInfo(int routePlanID)
        {
            string sql = @"
                SELECT 
                    rp.VehicleID, 
                    vm.VehicleNumber,
                    vm.AssignDriverID as EmployeeID,
                    ep.First_Name + ' ' + ep.Last_Name as EmployeeName
                FROM tblRoutePlan rp
                INNER JOIN tblVehicleMaster vm ON rp.VehicleID = vm.VehicleID
                LEFT JOIN tbl_EmployeeProfileMaster ep ON vm.AssignDriverID = ep.Employee_id
                WHERE rp.RoutePlanID = @RoutePlanID AND rp.IsActive = 1";

            var result = await _dbConnection.QuerySingleOrDefaultAsync<RouteVehicleDriverInfoResponse>(sql, new { RoutePlanID = routePlanID });

            return result;
        }

        public async Task<IEnumerable<GetStudentsForRouteMappingResponse>> GetStudentsForRouteMapping(int classID, int sectionID, int instituteID, string search)
        {
            string sql = @"
        SELECT 
            sm.student_id AS StudentID,
            sm.First_Name + ' ' + sm.Last_Name AS StudentName,
            sm.Roll_Number AS RollNumber,
            sm.Admission_Number AS AdmissionNumber,
            c.class_name AS ClassName,   -- Correct alias
            s.section_name AS SectionName -- Correct alias
        FROM tbl_StudentMaster sm
        INNER JOIN tbl_Class c ON sm.class_id = c.class_id
        INNER JOIN tbl_Section s ON sm.section_id = s.section_id
        WHERE sm.class_id = @ClassID
            AND sm.section_id = @SectionID
            AND sm.Institute_id = @InstituteID
            AND (sm.First_Name LIKE @Search OR sm.Last_Name LIKE @Search OR sm.Roll_Number LIKE @Search OR sm.Admission_Number LIKE @Search)
        ORDER BY sm.First_Name";

            var result = await _dbConnection.QueryAsync<GetStudentsForRouteMappingResponse>(sql,
                new { ClassID = classID, SectionID = sectionID, InstituteID = instituteID, Search = "%" + search + "%" });

           

            return result;
        }

        public async Task<IEnumerable<GetEmployeesForRouteMappingResponse>> GetEmployeesForRouteMapping(int departmentID, int designationID, int instituteID, string search)
        {
            string sql = @"
                SELECT 
                    e.Employee_id AS EmployeeID,
                    e.First_Name + ' ' + e.Last_Name AS EmployeeName,
                    e.Employee_code_id AS EmployeeCode,
                    d.DepartmentName AS DepartmentName,
                    des.DesignationName AS DesignationName
                FROM tbl_EmployeeProfileMaster e
                INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
                INNER JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
                WHERE e.Department_id = @DepartmentID
                    AND e.Designation_id = @DesignationID
                    AND e.Institute_id = @InstituteID
                    AND (e.First_Name LIKE @Search OR e.Last_Name LIKE @Search OR e.Employee_code_id LIKE @Search)
                ORDER BY e.First_Name";

            var result = await _dbConnection.QueryAsync<GetEmployeesForRouteMappingResponse>(sql,
                new { DepartmentID = departmentID, DesignationID = designationID, InstituteID = instituteID, Search = "%" + search + "%" });

            return result;
        }

        public async Task<ServiceResponse<IEnumerable<GetRouteListResponse>>> GetRouteList(int instituteID)
        {
            string sql = @"
        SELECT RoutePlanID, RouteName 
        FROM tblRoutePlan 
        WHERE InstituteID = @InstituteID AND IsActive = 1";

            var routePlans = await _dbConnection.QueryAsync<GetRouteListResponse>(sql, new { InstituteID = instituteID });

            if (routePlans == null || !routePlans.Any())
            {
                return new ServiceResponse<IEnumerable<GetRouteListResponse>>(false, "No route plans found for the given InstituteID", new List<GetRouteListResponse>(), StatusCodes.Status204NoContent);
            }

            return new ServiceResponse<IEnumerable<GetRouteListResponse>>(true, "Route plans fetched successfully", routePlans, StatusCodes.Status200OK);
        }

        public async Task<RouteDetailsResponseDTO1> GetRouteDetailsWithStopInfo(GetAllRouteAssignedInfoRequest request)
        {
            var routeSql = @"
                SELECT 
                    rp.RoutePlanID, 
                    rp.RouteName
                FROM 
                    tblRoutePlan rp
                WHERE 
                    rp.RoutePlanID = @RouteID
                    AND rp.InstituteID = @InstituteID
                    AND rp.IsActive = 1";

            var stopsSql = @"
                SELECT 
                    rs.StopID,
                    rs.StopName, 
                    rs.PickUpTime, 
                    rs.DropTime, 
                    rs.FeeAmount AS Fee,
                    (SELECT COUNT(*) FROM tblStudentStopMapping ss WHERE ss.StopID = rs.StopID) AS StudentCount,
                    (SELECT COUNT(*) FROM tblEmployeeStopMapping es WHERE es.StopID = rs.StopID) AS EmployeeCount
                FROM 
                    tblRouteStopMaster rs
                WHERE 
                    rs.RoutePlanID = @RouteID";

            var routeDetails = await _dbConnection.QueryFirstOrDefaultAsync<RouteDetailsResponseDTO1>(routeSql, new
            {
                RouteID = request.RouteID,
                InstituteID = request.InstituteID
            });

            if (routeDetails != null)
            {
                var stops = await _dbConnection.QueryAsync<RouteStopResponseDTO1>(stopsSql, new
                {
                    RouteID = request.RouteID
                });

                routeDetails.Stops = stops.ToList();

                return routeDetails;
            }

            return null;
        }

    }
}