using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Repository.Interfaces;
using Dapper;

namespace Attendance_API.Repository.Implementations
{
    public class EmployeeAttendanceRepository : IEmployeeAttendanceRepository
    {
        private readonly IDbConnection _connection;

        public EmployeeAttendanceRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<EmployeeAttendanceMasterResponseDTO>>> GetEmployeeAttendanceMasterList(EmployeeAttendanceMasterRequestDTO request)
        {
            if (request == null || request.Date == DateTime.MinValue || request.Department_id == 0)
            {
                return new ServiceResponse<List<EmployeeAttendanceMasterResponseDTO>>(false, "Invalid request", new List<EmployeeAttendanceMasterResponseDTO>(), 400);
            }
            string sql = $"select eam.Employee_Attendance_Master_id, epm.First_Name + epm.Last_Name as Employee_Name, epm.Employee_id, d.Department_id, d.DepartmentName, eam.Employee_Attendance_Status_id, eam.Remarks from tbl_EmployeeProfileMaster epm " +
                         $"left join tbl_EmployeeAttendanceMaster eam on epm.Employee_id = eam.Employee_id and eam.Date = '{request.Date.ToString("yyyy-MM-dd")}' " +
                         $"join tbl_Department d on epm.Department_id = d.Department_id " +
                         $"where epm.Department_id = {request.Department_id}";

            var result = await _connection.QueryAsync<EmployeeAttendanceMasterResponseDTO>(sql);
            return new ServiceResponse<List<EmployeeAttendanceMasterResponseDTO>>(true, "Operation successful", result.ToList(), 200);
        }

        public async Task<ServiceResponse<EmployeeAttendanceMasterDTO>> InsertOrUpdateEmployeeAttendanceMaster(EmployeeAttendanceMasterDTO employeeAttendanceMaster)
        {
            if (employeeAttendanceMaster == null || employeeAttendanceMaster.Employee_id == 0 || employeeAttendanceMaster.Date == DateTime.MinValue || employeeAttendanceMaster.Employee_Attendance_Status_id == 0)
            {
                return new ServiceResponse<EmployeeAttendanceMasterDTO>(false, "Invalid request", null, 400);
            }

            if (employeeAttendanceMaster.Employee_Attendance_Master_id == 0)
            {
                // Insert new record
                string insertSql = $"INSERT INTO tbl_EmployeeAttendanceMaster (Employee_id, Employee_Attendance_Status_id, Remarks, Date) " +
                                  $"VALUES (@Employee_id, @Employee_Attendance_Status_id, @Remarks, @Date)";
                await _connection.ExecuteAsync(insertSql, employeeAttendanceMaster);
            }
            else
            {
                // Update existing record
                string updateSql = $"UPDATE tbl_EmployeeAttendanceMaster " +
                                  $"SET Employee_id = @Employee_id, Employee_Attendance_Status_id = @Employee_Attendance_Status_id, Remarks = @Remarks, Date = @Date " +
                                  $"WHERE Employee_Attendance_Master_id = @Employee_Attendance_Master_id";
                await _connection.ExecuteAsync(updateSql, employeeAttendanceMaster);
            }

            return new ServiceResponse<EmployeeAttendanceMasterDTO>(true, "Operation successful", employeeAttendanceMaster, 200);
        }

        public async Task<ServiceResponse<bool>> DeleteEmployeeAttendanceMaster(int employeeAttendanceId)
        {
            if (employeeAttendanceId == 0)
            {
                return new ServiceResponse<bool>(false, "Invalid request", false, 400);
            }

            string deleteSql = $"DELETE FROM tbl_EmployeeAttendanceMaster WHERE Employee_Attendance_Master_id = @Employee_Attendance_Master_id";
            await _connection.ExecuteAsync(deleteSql, new { Employee_Attendance_Master_id = employeeAttendanceId });

            return new ServiceResponse<bool>(true, "Operation successful", true, 200);
        }
    }
}