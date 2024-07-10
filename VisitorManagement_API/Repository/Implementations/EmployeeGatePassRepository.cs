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
                    string query = @"INSERT INTO tblGatePass (EmployeeID, PassNo, VisitorFor, CheckOutTime, CheckInTime, Purpose, PlanOfVisit, Remarks, StatusID, InstituteId)
                                     VALUES (@EmployeeID, @PassNo, @VisitorFor, @CheckOutTime, @CheckInTime, @Purpose, @PlanOfVisit, @Remarks, @StatusID, @InstituteId)";
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
            SELECT gp.GatePassID, gp.EmployeeID, gp.Departmentid, d.DepartmentName AS Departmentname,
                   des.DesignationName AS Designationname, gp.Designationid, gp.PassNo, gp.VisitorFor, 
                   gp.CheckOutTime, gp.CheckInTime, gp.Purpose, gp.PlanOfVisit, gp.Remarks, gp.StatusID, gp.InstituteId
            FROM tblGatePass gp
            JOIN tbl_Department d ON gp.Departmentid = d.Department_id
            JOIN tbl_Designation des ON gp.Designationid = des.Designation_id
            WHERE gp.StatusID = 1 and InstituteId = @InstituteId";

                var employeeGatePasses = await _dbConnection.QueryAsync<EmployeeGatepassResponse>(query, new {request.InstituteId});
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
            SELECT gp.GatePassID, gp.EmployeeID, gp.Departmentid, d.DepartmentName AS Departmentname,
                   des.DesignationName AS Designationname, gp.Designationid, gp.PassNo, gp.VisitorFor, 
                   gp.CheckOutTime, gp.CheckInTime, gp.Purpose, gp.PlanOfVisit, gp.Remarks, gp.StatusID, gp.InstituteId
            FROM tblGatePass gp
            JOIN tbl_Department d ON gp.Departmentid = d.Department_id
            JOIN tbl_Designation des ON gp.Designationid = des.Designation_id
            WHERE gp.GatePassID = @GatePassID AND gp.StatusID = 1";

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
                // Assuming there is a StatusID column in tblGatePass table, and 0 means 'soft deleted'
                string query = "UPDATE tblGatePass SET StatusID = 0 WHERE GatePassID = @GatePassID";
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

    }
}
