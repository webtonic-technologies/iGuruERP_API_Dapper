using Dapper;
using System.Data;
using System.Data.SqlClient;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.Responses;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;
using VisitorManagement_API.Repository.Interfaces;

namespace VisitorManagement_API.Repository.Implementations
{
    public class EmployeeGatePassRepository : IEmployeeGatePassRepository
    {
        private readonly IDbConnection _dbConnection;

        public EmployeeGatePassRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateEmployeeGatePass(EmployeeGatePass employeeGatePass)
        {
            try
            {
                if (employeeGatePass.GatePassID == 0)
                {
                    // Insert new employee gate pass
                    string query = @"INSERT INTO tblGatePass (EmployeeID, PassNo, VisitorFor, CheckOutTime, CheckInTime, Purpose, PlanOfVisit, Remarks, StatusID, InstituteId, IsDeleted)
                                     VALUES (@EmployeeID, @PassNo, @VisitorFor, @CheckOutTime, @CheckInTime, @Purpose, @PlanOfVisit, @Remarks, @StatusID, @InstituteId,@IsDeleted)";
                    employeeGatePass.IsDeleted = false;
                    int insertedValue = await _dbConnection.ExecuteAsync(query, employeeGatePass);
                    if (insertedValue > 0)
                    {
                        return new ServiceResponse<string>(true, "Employee Gate Pass Added Successfully", "Success", 201);
                    }
                    return new ServiceResponse<string>(false, "Failed to Add Employee Gate Pass", "Failure", 400);
                }
                else
                {
                    // Update existing employee gate pass
                    string query = @"UPDATE tblGatePass SET EmployeeID = @EmployeeID, PassNo = @PassNo, VisitorFor = @VisitorFor, CheckOutTime = @CheckOutTime, CheckInTime = @CheckInTime, Purpose = @Purpose, PlanOfVisit = @PlanOfVisit, Remarks = @Remarks, StatusID = @StatusID, InstituteId = @InstituteId
                                     WHERE GatePassID = @GatePassID";
                    int rowsAffected = await _dbConnection.ExecuteAsync(query, employeeGatePass);
                    if (rowsAffected > 0)
                    {
                        return new ServiceResponse<string>(true, "Employee Gate Pass Updated Successfully", "Success", 200);
                    }
                    return new ServiceResponse<string>(false, "Failed to Update Employee Gate Pass", "Failure", 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<EmployeeGatepassResponse>>> GetAllEmployeeGatePass(GetAllEmployeeGatePassRequest request)
        {
            try
            {
                string query = @"
        SELECT gp.GatePassID, gp.EmployeeID, 
               e.First_Name + ' ' + e.Middle_Name + ' ' + e.Last_Name AS EmployeeName, 
               e.Department_id AS Departmentid, d.DepartmentName, 
               e.Designation_id AS Designationid, des.DesignationName, 
               gp.PassNo, gp.VisitorFor, vf.VisitedForName as VisitorForName, gp.CheckOutTime, gp.CheckInTime, 
               gp.Purpose, gp.PlanOfVisit, gp.Remarks, gp.StatusID, a.ApprovalType AS StatusName, gp.InstituteId
        FROM tblGatePass gp
        JOIN tbl_EmployeeProfileMaster e ON gp.EmployeeID = e.Employee_id
        JOIN tbl_Department d ON e.Department_id = d.Department_id
        JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
        JOIN tblVisitorApprovalMaster a ON gp.StatusID = a.ApprovalTypeID
        JOIN tblVisitedFor vf ON gp.VisitorFor = vf.VisitedFor
        WHERE gp.IsDeleted = 0 AND gp.InstituteId = @InstituteId";

                var employeeGatePasses = await _dbConnection.QueryAsync<EmployeeGatepassResponse>(query, new { request.InstituteId });

                // Apply filters only if they are provided
                if (request.StartDate.HasValue && request.EndDate.HasValue)
                {
                    employeeGatePasses = employeeGatePasses.Where(gp => gp.CheckInTime >= request.StartDate && gp.CheckInTime <= request.EndDate);
                }

                if (request.EmployeeId > 0)
                {
                    employeeGatePasses = employeeGatePasses.Where(gp => gp.EmployeeID == request.EmployeeId);
                }

                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    employeeGatePasses = employeeGatePasses.Where(gp => gp.EmployeeName.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                                                                        gp.PassNo.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                                                                        gp.Purpose.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase));
                }

                // Pagination
                var paginatedEmployeeGatePasses = employeeGatePasses.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

                return new ServiceResponse<IEnumerable<EmployeeGatepassResponse>>(true, "Employee Gate Passes Retrieved Successfully", paginatedEmployeeGatePasses, 200, employeeGatePasses.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<EmployeeGatepassResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<EmployeeGatepassResponse>> GetEmployeeGatePassById(int gatePassId) 
        {
            try
            {
                string query = @"
        SELECT gp.GatePassID, gp.EmployeeID, 
               e.First_Name + ' ' + e.Middle_Name + ' ' + e.Last_Name AS EmployeeName, 
               e.Department_id AS Departmentid, d.DepartmentName, 
               e.Designation_id AS Designationid, des.DesignationName, 
               gp.PassNo, gp.VisitorFor, vf.VisitedForName as VisitorForName, gp.CheckOutTime, gp.CheckInTime, 
               gp.Purpose, gp.PlanOfVisit, gp.Remarks, gp.StatusID, a.ApprovalType AS StatusName, gp.InstituteId
        FROM tblGatePass gp
        JOIN tbl_EmployeeProfileMaster e ON gp.EmployeeID = e.Employee_id
        JOIN tbl_Department d ON e.Department_id = d.Department_id
        JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
        JOIN tblVisitorApprovalMaster a ON gp.StatusID = a.ApprovalTypeID
        JOIN tblVisitedFor vf ON gp.VisitorFor = vf.VisitedFor
        WHERE gp.GatePassID = @GatePassID AND gp.IsDeleted = 0";

                var employeeGatePass = await _dbConnection.QueryFirstOrDefaultAsync<EmployeeGatepassResponse>(query, new { GatePassID = gatePassId });

                if (employeeGatePass != null)
                {
                    return new ServiceResponse<EmployeeGatepassResponse>(true, "Employee Gate Pass Retrieved Successfully", employeeGatePass, 200);
                }

                return new ServiceResponse<EmployeeGatepassResponse>(false, "Employee Gate Pass Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeGatepassResponse>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateEmployeeGatePassStatus(int gatePassId)
        {
            try
            {
                // Assuming there is an IsDeleted column in tblGatePass table and a value of 1 means 'soft deleted'
                string query = "UPDATE tblGatePass SET IsDeleted = 1 WHERE GatePassID = @GatePassID";
                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { GatePassID = gatePassId });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Employee Gate Pass Status Updated Successfully (Soft Deleted)", true, 200);
                }

                return new ServiceResponse<bool>(false, "Failed to Update Employee Gate Pass Status", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        public async Task<ServiceResponse<List<Visitedfor>>> GetAllVisitedForReason()
        {
            try
            {
                string query = "select * from tblVisitedFor";
                var rowsAffected = await _dbConnection.QueryAsync<Visitedfor>(query);

                if (rowsAffected.Any())
                {
                    return new ServiceResponse<List<Visitedfor>>(true, "Records found", rowsAffected.AsList(), 200);
                }

                return new ServiceResponse<List<Visitedfor>>(false, "Failed to Update Employee Gate Pass Status", [], 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Visitedfor>>(false, ex.Message, [], 500);
            }
        }
    }
}
