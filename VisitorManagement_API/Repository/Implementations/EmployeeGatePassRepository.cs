using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using VisitorManagement_API.DTOs.Requests;
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
                    string query = @"INSERT INTO tblGatePass (EmployeeID, PassNo, VisitorFor, CheckOutTime, CheckInTime, Purpose, PlanOfVisit, Remarks, StatusID)
                                     VALUES (@EmployeeID, @PassNo, @VisitorFor, @CheckOutTime, @CheckInTime, @Purpose, @PlanOfVisit, @Remarks, @StatusID)";
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
                    string query = @"UPDATE tblGatePass SET EmployeeID = @EmployeeID, PassNo = @PassNo, VisitorFor = @VisitorFor, CheckOutTime = @CheckOutTime, CheckInTime = @CheckInTime, Purpose = @Purpose, PlanOfVisit = @PlanOfVisit, Remarks = @Remarks, StatusID = @StatusID
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

        public async Task<ServiceResponse<IEnumerable<EmployeeGatePass>>> GetAllEmployeeGatePass(GetAllEmployeeGatePassRequest request)
        {
            try
            {
                string query = "SELECT * FROM tblGatePass";
                var employeeGatePasses = await _dbConnection.QueryAsync<EmployeeGatePass>(query);
                var paginatedEmployeeGatePasses = employeeGatePasses.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
                return new ServiceResponse<IEnumerable<EmployeeGatePass>>(true, "Employee Gate Passes Retrieved Successfully", paginatedEmployeeGatePasses, 200, employeeGatePasses.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<EmployeeGatePass>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<EmployeeGatePass>> GetEmployeeGatePassById(int gatePassId)
        {
            try
            {
                string query = "SELECT * FROM tblGatePass WHERE GatePassID = @GatePassID";
                var employeeGatePass = await _dbConnection.QueryFirstOrDefaultAsync<EmployeeGatePass>(query, new { GatePassID = gatePassId });
                if (employeeGatePass != null)
                {
                    return new ServiceResponse<EmployeeGatePass>(true, "Employee Gate Pass Retrieved Successfully", employeeGatePass, 200);
                }
                return new ServiceResponse<EmployeeGatePass>(false, "Employee Gate Pass Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeGatePass>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateEmployeeGatePassStatus(int gatePassId)
        {
            try
            {
                // Assuming there is a StatusID column in tblGatePass table
                string query = "UPDATE tblGatePass SET StatusID = CASE WHEN StatusID = 1 THEN 0 ELSE 1 END WHERE GatePassID = @GatePassID";
                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { GatePassID = gatePassId });
                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Employee Gate Pass Status Updated Successfully", true, 200);
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
