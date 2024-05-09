using Dapper;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Models;
using Attendance_API.Repository.Interfaces;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_API.Repository.Implementations
{
    public class EmployeeAttendanceStatusMasterRepository : IEmployeeAttendanceStatusMasterRepository
    {
        private readonly IDbConnection _connection;

        public EmployeeAttendanceStatusMasterRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<EmployeeAttendanceStatusMasterDTO>>> GetEmployeeAttendanceStatusMasterList()
        {
            var response = new List<EmployeeAttendanceStatusMasterDTO>();
            string sql = @"SELECT *
                       FROM [dbo].[tbl_EmployeeAttendanceStatusMaster]";

            var data = await _connection.QueryAsync<EmployeeAttendanceStatusMaster>(sql);
            if (data != null)
            {
                foreach (var item in data)
                {
                    var record = new EmployeeAttendanceStatusMasterDTO
                    {
                        Employee_Attendance_Status_id = item.Employee_Attendance_Status_id,
                        Employee_Attendance_Status_Type = item.Employee_Attendance_Status_Type,
                        Short_Name = item.Short_Name
                    };
                    response.Add(record);
                }
                return new ServiceResponse<List<EmployeeAttendanceStatusMasterDTO>>(true, "Record found", response, 200);
            }
            else
            {
                return new ServiceResponse<List<EmployeeAttendanceStatusMasterDTO>>(false, "record not found", new List<EmployeeAttendanceStatusMasterDTO>(), 500);
            }
        }

        public async Task<ServiceResponse<EmployeeAttendanceStatusMasterDTO>> GetEmployeeAttendanceStatusMasterById(int Employee_Attendance_Status_id)
        {
            var response = new EmployeeAttendanceStatusMasterDTO();
            string sql = @"SELECT *
                       FROM [dbo].[tbl_EmployeeAttendanceStatusMaster]
                       WHERE Employee_Attendance_Status_id = @Employee_Attendance_Status_id";

            var data = await _connection.QueryFirstOrDefaultAsync<EmployeeAttendanceStatusMaster>(sql, new { Employee_Attendance_Status_id });
            if (data != null)
            {
                response.Employee_Attendance_Status_id = data.Employee_Attendance_Status_id;
                response.Employee_Attendance_Status_Type = data.Employee_Attendance_Status_Type;
                response.Short_Name = data.Short_Name;
                return new ServiceResponse<EmployeeAttendanceStatusMasterDTO>(true, "Record found", response, 200);
            }
            else
            {
                return new ServiceResponse<EmployeeAttendanceStatusMasterDTO>(false, "record not found", new EmployeeAttendanceStatusMasterDTO(), 500);
            }
        }

        public async Task<ServiceResponse<string>> AddEmployeeAttendanceStatusMaster(EmployeeAttendanceStatusMasterDTO request)
        {
            string sql = @"INSERT INTO [dbo].[tbl_EmployeeAttendanceStatusMaster] (Employee_Attendance_Status_Type, Short_Name)
                       VALUES (@Employee_Attendance_Status_Type, @Short_Name);
                       SELECT SCOPE_IDENTITY();";

            int insertedId = await _connection.ExecuteScalarAsync<int>(sql, new
            {
                Employee_Attendance_Status_Type = request.Employee_Attendance_Status_Type,
                Short_Name = request.Short_Name
            });
            if (insertedId > 0)
            {
                return new ServiceResponse<string>(true, "Operation successful", "Data added successfully", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Some error occured", string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<string>> UpdateEmployeeAttendanceStatusMaster(EmployeeAttendanceStatusMasterDTO request)
        {
            string sql = @"UPDATE [dbo].[tbl_EmployeeAttendanceStatusMaster]
                       SET Employee_Attendance_Status_Type = @Employee_Attendance_Status_Type,
                           Short_Name = @Short_Name
                       WHERE Employee_Attendance_Status_id = @Employee_Attendance_Status_id";

            int affectedRows = await _connection.ExecuteAsync(sql, new
            {
                Employee_Attendance_Status_Type = request.Employee_Attendance_Status_Type,
                Short_Name = request.Short_Name,
                Employee_Attendance_Status_id = request.Employee_Attendance_Status_id
            });
            if (affectedRows > 0)
            {
                return new ServiceResponse<string>(true, "Operation successful", "Data updated successfully", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Some error occured", string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteEmployeeAttendanceStatusMaster(int Employee_Attendance_Status_id)
        {
            string sql = @"DELETE FROM [dbo].[tbl_EmployeeAttendanceStatusMaster]
                       WHERE Employee_Attendance_Status_id = @Employee_Attendance_Status_id";

            int affectedRows = await _connection.ExecuteAsync(sql, new { Employee_Attendance_Status_id });
            if (affectedRows > 0)
            {
                return new ServiceResponse<string>(true, "Operation successful", "Data deleted successfully", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "operation failed", string.Empty, 500);
            }
        }
    }
}
