using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Helper;
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

        public async Task<ServiceResponse<EmployeeAttendanceMasterResponseDTO>> GetEmployeeAttendanceMasterList(EmployeeAttendanceMasterRequestDTO request)
        {
            var date = DateTimeHelper.ConvertToDateTime(request.Date);
            if (request == null || date == DateTime.MinValue || request.Department_id == 0)
            {
                return new ServiceResponse<EmployeeAttendanceMasterResponseDTO>(false, "Invalid request", new EmployeeAttendanceMasterResponseDTO(), 400);
            }
            string sql = $"select eam.Employee_Attendance_Master_id, epm.First_Name + epm.Last_Name as Employee_Name, epm.Employee_id, d.Department_id, d.DepartmentName, eam.Employee_Attendance_Status_id, eam.Remarks, eas.Employee_Attendance_Status_Type, eas.Short_Name as Employee_Attendance_Status_Short_Name from tbl_EmployeeProfileMaster epm " +
                         $"left join tbl_EmployeeAttendanceMaster eam on epm.Employee_id = eam.Employee_id and eam.Date = '{date.ToString("yyyy-MM-dd")}' " +
                         $"join tbl_Department d on epm.Department_id = d.Department_id " +
                         $"join tbl_EmployeeAttendanceStatusMaster eas on eas.Employee_Attendance_Status_id = eam.Employee_Attendance_Status_id " +
                         $"where epm.Department_id = {request.Department_id}";
            if (request.pageNumber != null && request.pageSize != null)
            {
                sql += $" Order by 1 OFFSET {(request.pageNumber - 1) * request.pageSize} ROWS FETCH NEXT {request.pageSize} ROWS ONLY;";
            }
            var result = await _connection.QueryAsync<EmployeeAttendanceMasterResponse>(sql);
            sql = $"select COUNT(*) from tbl_EmployeeProfileMaster epm " +
                         $"left join tbl_EmployeeAttendanceMaster eam on epm.Employee_id = eam.Employee_id and eam.Date = '{date.ToString("yyyy-MM-dd")}' " +
                         $"join tbl_Department d on epm.Department_id = d.Department_id " +
                         $"join tbl_EmployeeAttendanceStatusMaster eas on eas.Employee_Attendance_Status_id = eam.Employee_Attendance_Status_id " +
                         $"where epm.Department_id = {request.Department_id}";
            var countRes = await _connection.QueryAsync<long>(sql);
            var count = countRes.FirstOrDefault();
            return new ServiceResponse<EmployeeAttendanceMasterResponseDTO>(true, "Operation successful", new EmployeeAttendanceMasterResponseDTO { Data = result.ToList(), Total = count }, 200);
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
                string insertSql = $"INSERT INTO tbl_EmployeeAttendanceMaster (Employee_id, Employee_Attendance_Status_id, Remarks, Date ,TimeSlot_id , isHoliday) " +
                                  $"VALUES (@Employee_id, @Employee_Attendance_Status_id, @Remarks, @Date,@TimeSlot_id , @isHoliday)";
                await _connection.ExecuteAsync(insertSql, employeeAttendanceMaster);
            }
            else
            {
                // Update existing record
                string updateSql = $"UPDATE tbl_EmployeeAttendanceMaster " +
                                  $"SET Employee_id = @Employee_id, Employee_Attendance_Status_id = @Employee_Attendance_Status_id, Remarks = @Remarks, TimeSlot_id = @TimeSlot_id , isHoliday =@isHoliday" +
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
        public async Task<ServiceResponse<dynamic>> GetEmployeeAttendanceReport(EmployeeAttendanceReportRequestDTO request)
        {
            try
            {
                var query = @"DECLARE @cols AS NVARCHAR(MAX);
DECLARE @query AS NVARCHAR(MAX);

-- Generate a list of all dates between StartDate and EndDate
WITH DateList AS (
    SELECT @StartDate AS Date
    UNION ALL
    SELECT DATEADD(DAY, 1, Date)
    FROM DateList
    WHERE Date < @EndDate
)
SELECT @cols = STRING_AGG(QUOTENAME(CONVERT(VARCHAR, Date, 23)), ', ')
FROM DateList;

SET @query = '
DECLARE @TotalDays INT;

SET @TotalDays = DATEDIFF(DAY, @StartDate, @EndDate);
WITH DateList AS (
    SELECT @StartDate AS Date
    UNION ALL
    SELECT DATEADD(DAY, 1, Date)
    FROM DateList
    WHERE Date < @EndDate
),
AttendanceData AS (
    SELECT 
        e.Employee_id,
        e.First_Name + '' '' + e.Last_Name AS Employee_Name,
        e.mobile_number,
        d.Date,
        ISNULL(
            CASE 
                WHEN a.isHoliday = 1 THEN ''H'' 
                ELSE ast.Short_Name 
            END, 
            ''-''
        ) AS Status,
        @TotalDays - SUM(CASE WHEN a.isHoliday = 1 THEN 1 ELSE 0 END) OVER (PARTITION BY e.Employee_id) AS WorkingDays,
        SUM(CASE WHEN a.isHoliday = 0 AND ast.Short_Name = ''P'' THEN 1 ELSE 0 END) OVER (PARTITION BY e.Employee_id) AS Present,
        SUM(CASE WHEN a.isHoliday = 0 AND ast.Short_Name = ''A'' THEN 1 ELSE 0 END) OVER (PARTITION BY e.Employee_id) AS Absent,
		  ROUND(CAST(SUM(CASE 
                WHEN a.isHoliday = 0 AND ast.Short_Name = ''P'' THEN ts.[value]
                ELSE 0 
            END) OVER (PARTITION BY e.Employee_id) AS FLOAT) 
              / NULLIF(COUNT(*) OVER (PARTITION BY e.Employee_id) - SUM(CASE WHEN a.isHoliday = 1 THEN 1 ELSE 0 END) OVER (PARTITION BY e.Employee_id), 0) * 100, 2) AS AttendancePercentage
       
    FROM 
        DateList d
    LEFT JOIN 
        tbl_EmployeeAttendanceMaster a ON d.Date = a.Date
    LEFT JOIN 
        tbl_EmployeeProfileMaster e ON a.Employee_id = e.Employee_id
    LEFT JOIN 
        tbl_EmployeeAttendanceStatusMaster ast ON a.Employee_Attendance_Status_id = ast.Employee_Attendance_Status_id
	LEFT JOIN
        tbl_timeslot ts ON a.TimeSlot_id = ts.id
    WHERE 
        d.Date BETWEEN @StartDate AND @EndDate
 AND (@InstituteId = 0 OR e.institute_id = @InstituteId)
)
SELECT 
    Employee_id,
    Employee_Name,
    mobile_number,
    WorkingDays,
    Present,
    Absent,
    AttendancePercentage,
    ' + @cols + '
FROM 
    AttendanceData
PIVOT (
    MAX(Status) 
    FOR Date IN (' + @cols + ')
) AS PivotTable
WHERE Employee_id IS NOT NULL
ORDER BY 
    Employee_Name;
';

EXEC sp_executesql @query, N'@StartDate DATE, @EndDate DATE , @InstituteId INT', @StartDate, @EndDate,@InstituteId;";

                var parameters = new { StartDate = request.StartDate, EndDate = request.EndDate, InstituteId = request.instituteId };

                // Execute the query and fetch the result
                var result = await _connection.QueryAsync<dynamic>(query, parameters);
                //var resultJson = JsonSerializer.Deserialize<dynamic>(result);


                return new ServiceResponse<dynamic>(true, "Operation successful", result, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<dynamic>(false, $"Error: {ex.Message}", null, 500);
            }
        }
    }
}