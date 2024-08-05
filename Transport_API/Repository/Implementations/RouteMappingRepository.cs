using Dapper;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Numerics;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.ServiceResponse;
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

        public async Task<ServiceResponse<string>> AddUpdateRouteMapping(RouteMapping routeMapping)
        {
            string sql;
            if (routeMapping.AssignRouteID == 0)
            {
                sql = @"INSERT INTO tblAssignRoute (RoutePlanID, VehicleID, DriverID, TransportStaffID, IsActive) 
                        VALUES (@RoutePlanID, @VehicleID, @DriverID, @TransportStaffID, @IsActive)";
            }
            else
            {
                sql = @"UPDATE tblAssignRoute SET RoutePlanID = @RoutePlanID, VehicleID = @VehicleID, DriverID = @DriverID, TransportStaffID = @TransportStaffID, IsActive = @IsActive
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
        //public async Task<ServiceResponse<IEnumerable<RouteMappingResponse>>> GetAllRouteMappings(GetAllRouteMappingRequest request)
        //{
        //    string countSql = @"SELECT COUNT(*) FROM tblAssignRoute";
        //    int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql);

        //    string sql = @"SELECT * FROM tblAssignRoute ORDER BY AssignRouteID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        //    var routeMappings = await _dbConnection.QueryAsync<RouteMappingResponse>(sql, new { Offset = (request.PageNumber - 1) * request.PageSize, PageSize = request.PageSize });

        //    if (routeMappings.Any())
        //    {
        //        return new ServiceResponse<IEnumerable<RouteMappingResponse>>(true, "Records Found", routeMappings, StatusCodes.Status200OK, totalCount);
        //    }
        //    else
        //    {
        //        return new ServiceResponse<IEnumerable<RouteMappingResponse>>(false, "No Records Found", [], StatusCodes.Status204NoContent);
        //    }
        //}
        public async Task<ServiceResponse<IEnumerable<RouteMappingResponse>>> GetAllRouteMappings(GetAllRouteMappingRequest request)
        {
            try
            {
                string countSql = @"SELECT COUNT(*) FROM tblAssignRoute ar
                            JOIN tblRoutePlan rp ON ar.RoutePlanID = rp.RoutePlanID
                            WHERE rp.IsActive = 1";

                string whereClause = "";

                if (request.RoutePlanId > 0)
                {
                    whereClause = " AND ar.RoutePlanID = @RoutePlanID";
                    countSql += whereClause;
                }

                int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql, new { RoutePlanID = request.RoutePlanId });

                string sql = @"
            SELECT ar.AssignRouteID as RouteMappingId, 
                   rp.RouteName, 
                   v.VehicleID, 
                   v.VehicleNumber, 
                   e.Employee_id as EmployeeId,
                   ISNULL(CONCAT(e.First_Name, ' ', e.Last_Name), '') AS DriverName,
                   ts.Employee_id as TransportStaffID,
                   ISNULL(CONCAT(ts.First_Name, ' ', ts.Last_Name), '') AS TransportStaffName,
                   (SELECT COUNT(*) FROM tblStudentStopMapping srm WHERE srm.RoutePlanID = rp.RoutePlanID) AS TotalStudents,
                   (SELECT COUNT(*) FROM tblEmployeeStopMapping erm WHERE erm.RoutePlanID = rp.RoutePlanID) AS TotalEmployees
            FROM tblAssignRoute ar
            JOIN tblRoutePlan rp ON ar.RoutePlanID = rp.RoutePlanID
            JOIN tblVehicleMaster v ON ar.VehicleID = v.VehicleID
            LEFT JOIN tbl_EmployeeProfileMaster e ON v.AssignDriverID = e.Employee_id
            LEFT JOIN tbl_EmployeeProfileMaster ts ON ar.TransportStaffID = ts.Employee_id
            WHERE rp.IsActive = 1"
                    + whereClause + @"
            ORDER BY ar.AssignRouteID 
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var routeMappings = await _dbConnection.QueryAsync<RouteMappingResponse>(sql, new
                {
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize,
                    RoutePlanID = request.RoutePlanId
                });

                if (routeMappings.Any())
                {
                    return new ServiceResponse<IEnumerable<RouteMappingResponse>>(true, "Records Found", routeMappings, StatusCodes.Status200OK, totalCount);
                }
                else
                {
                    return new ServiceResponse<IEnumerable<RouteMappingResponse>>(false, "No Records Found", new List<RouteMappingResponse>(), StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<RouteMappingResponse>>(false, ex.Message, new List<RouteMappingResponse>(), StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<ServiceResponse<RouteMappingResponse>> GetRouteMappingById(int AssignRouteID)
        {
            try
            {
                string sql = @"
            SELECT ar.AssignRouteID as RouteMappingId, 
                   rp.RouteName, 
                   v.VehicleID, 
                   v.VehicleNumber, 
                   e.Employee_id as EmployeeId,
                   ISNULL(CONCAT(e.First_Name, ' ', e.Last_Name), '') AS DriverName,
                   ts.Employee_id as TransportStaffID,
                   ISNULL(CONCAT(ts.First_Name, ' ', ts.Last_Name), '') AS TransportStaffName,
                   (SELECT COUNT(*) FROM tblStudentStopMapping srm WHERE srm.RoutePlanID = rp.RoutePlanID) AS TotalStudents,
                   (SELECT COUNT(*) FROM tblEmployeeStopMapping erm WHERE erm.RoutePlanID = rp.RoutePlanID) AS TotalEmployees
            FROM tblAssignRoute ar
            JOIN tblRoutePlan rp ON ar.RoutePlanID = rp.RoutePlanID
            JOIN tblVehicleMaster v ON ar.VehicleID = v.VehicleID
            LEFT JOIN tbl_EmployeeProfileMaster e ON v.AssignDriverID = e.Employee_id
            LEFT JOIN tbl_EmployeeProfileMaster ts ON ar.TransportStaffID = ts.Employee_id
            WHERE ar.AssignRouteID = @AssignRouteID
            AND ar.IsActive = 1";

                var routeMapping = await _dbConnection.QueryFirstOrDefaultAsync<RouteMappingResponse>(sql, new { AssignRouteID });

                if (routeMapping != null)
                {
                    return new ServiceResponse<RouteMappingResponse>(true, "Record Found", routeMapping, StatusCodes.Status200OK);
                }
                else
                {
                    return new ServiceResponse<RouteMappingResponse>(false, "Record Not Found", new RouteMappingResponse(), StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
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
            WHERE ssm.RoutePlanID = @RoutePlanId AND sm.isActive = 1";

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
            WHERE esm.RoutePlanID = @RoutePlanId AND epm.isActive = 1";

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
    }
}