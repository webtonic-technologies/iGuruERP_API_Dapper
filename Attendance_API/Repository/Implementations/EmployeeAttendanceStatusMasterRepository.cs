using Dapper;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Models;
using Attendance_API.Repository.Interfaces;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Attendance_API.Repository.Implementations
{
    public class EmployeeAttendanceStatusMasterRepository : IEmployeeAttendanceStatusMasterRepository
    {
        private readonly IDbConnection _connection;

        public EmployeeAttendanceStatusMasterRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<EmployeeAttendanceStatusMasterDTO>>> GetEmployeeAttendanceStatusMasterList(int InstituteId)
        {
            var response = new List<EmployeeAttendanceStatusMasterDTO>();
            string sql = @"SELECT *
                       FROM [dbo].[tbl_EmployeeAttendanceStatusMaster]
                       WHERE isDelete = 0 AND InstituteId = @InstituteId";

            var data = await _connection.QueryAsync<EmployeeAttendanceStatusMaster>(sql, new { InstituteId });
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
                       WHERE Employee_Attendance_Status_id = @Employee_Attendance_Status_id AND isDelete = 0";

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

        public async Task<ServiceResponse<string>> SaveEmployeeAttendanceStatusMaster(List<EmployeeAttendanceStatusMasterDTO> request)
        {
            try
            {
                string insertSql = @"INSERT INTO [dbo].[tbl_EmployeeAttendanceStatusMaster] (Employee_Attendance_Status_Type, Short_Name, InstituteId)
                                     VALUES (@Employee_Attendance_Status_Type, @Short_Name, @InstituteId);";
                
                string updateSql = @"UPDATE [dbo].[tbl_EmployeeAttendanceStatusMaster]
                                     SET Employee_Attendance_Status_Type = @Employee_Attendance_Status_Type,
                                         Short_Name = @Short_Name
                                     WHERE Employee_Attendance_Status_id = @Employee_Attendance_Status_id;";

                foreach (var item in request)
                {
                    if (item.Employee_Attendance_Status_id == 0)
                    {
                        await _connection.ExecuteAsync(insertSql, new
                        {
                            Employee_Attendance_Status_Type = item.Employee_Attendance_Status_Type,
                            Short_Name = item.Short_Name,
                            InstituteId = item.InstituteId
                        });
                    }
                    else
                    {
                        await _connection.ExecuteAsync(updateSql, new
                        {
                            Employee_Attendance_Status_Type = item.Employee_Attendance_Status_Type,
                            Short_Name = item.Short_Name,
                            Employee_Attendance_Status_id = item.Employee_Attendance_Status_id
                        });
                    }
                }

                return new ServiceResponse<string>(true, "Operation successful", "Data processed successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteEmployeeAttendanceStatusMaster(int Employee_Attendance_Status_id)
        {
            string query1 = @"
                         SELECT COUNT(0)
                         FROM [dbo].[tbl_EmployeeAttendanceMaster]
                         WHERE Employee_Attendance_Status_id = @Employee_Attendance_Status_id"
            ;

            int count = await _connection.ExecuteScalarAsync<int>(query1, new { Employee_Attendance_Status_id });

            if (count > 0)
            {
                return new ServiceResponse<string>(false, "There is a dependency in Employee Attendance, so it cannot be deleted.", string.Empty, 400);
            }


            string sql = @"UPDATE [dbo].[tbl_EmployeeAttendanceStatusMaster]
                           SET isDelete = 1
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
