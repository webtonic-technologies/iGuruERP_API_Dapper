using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;

namespace Attendance_SE_API.Repository.Implementations
{
    public class EmployeeAttendanceReportRepository : IEmployeeAttendanceReportRepository
    {
        private readonly IDbConnection _connection;

        public EmployeeAttendanceReportRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<EmployeeAttendanceReportResponse> GetAttendanceReport(EmployeeAttendanceReportRequest request)
        {
            DateTime parsedStartDate;
            DateTime parsedEndDate;

            // Parse the start and end dates
            if (!DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedStartDate) ||
                !DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedEndDate))
            {
                return new EmployeeAttendanceReportResponse
                {
                    Success = false,
                    Message = "Invalid date format. Please use DD-MM-YYYY.",
                    Data = null,
                    StatusCode = 400,
                    TotalCount = 0
                };
            }

            string query = @"
    WITH AttendanceData AS (
        SELECT 
            e.Employee_id, 
            e.Employee_code_id,  
            CONCAT(e.First_Name, ' ', e.Last_Name) AS EmployeeName, 
            a.AttendanceDate,
            CASE 
                WHEN a.StatusID = 1 THEN 'P'
                WHEN a.StatusID = 2 THEN 'A'
                WHEN a.StatusID = 3 THEN 'HD'
                WHEN a.StatusID = 4 THEN 'L'
                WHEN a.StatusID = 5 THEN 'SL'
                WHEN a.StatusID = 6 THEN 'TL'
                ELSE '-'
            END AS AttendanceStatus
        FROM tbl_EmployeeProfileMaster e
        LEFT JOIN tblEmployeeAttendance a ON e.Employee_id = a.EmployeeID
            AND a.AttendanceDate BETWEEN @StartDate AND @EndDate
        WHERE e.Department_id = @DepartmentID  
            AND e.Institute_id = @InstituteID
    )
    SELECT 
        Employee_id,
        Employee_code_id, 
        EmployeeName,
        AttendanceDate,
        AttendanceStatus
    FROM AttendanceData
    ORDER BY EmployeeName, AttendanceDate;";

            var parameters = new
            {
                StartDate = parsedStartDate,
                EndDate = parsedEndDate,
                DepartmentID = request.DepartmentID,
                InstituteID = request.InstituteID,
                TimeSlotTypeID = request.TimeSlotTypeID // If needed
            };

            var attendanceRecords = await _connection.QueryAsync<AttendanceRecord2>(query, parameters);

            // Create a date range from start to end
            var dateRange = Enumerable.Range(0, (parsedEndDate - parsedStartDate).Days + 1)
                .Select(d => parsedStartDate.AddDays(d))
                .ToList();

            // Create a pivot structure
            var pivotedAttendance = attendanceRecords
                .GroupBy(x => new { x.EmployeeID, x.EmployeeCode, x.EmployeeName })
                .Select(g => new AttendanceDetailResponse_EMP
                {
                    EmployeeID = g.Key.EmployeeID,
                    EmployeeCode = g.Key.EmployeeCode,
                    EmployeeName = g.Key.EmployeeName,
                    WorkingDays = g.Count(),  // Total working days based on records
                    Present = g.Count(x => x.AttendanceStatus == "P"),
                    Absent = g.Count(x => x.AttendanceStatus == "A"),
                    AttendancePercentage = Math.Round((double)g.Count(x => x.AttendanceStatus == "P") / g.Count() * 100, 2),
                    Attendance = dateRange.ToDictionary(
                        date => date.ToString("MMM dd, ddd"),  // Format as "Sep 29, Sun"
                        date => g.FirstOrDefault(x => x.AttendanceDate.Date == date.Date)?.AttendanceStatus ?? "-"  // Use attendance status or "-"
                    )
                }).ToList();

            // Create the final response
            return new EmployeeAttendanceReportResponse
            {
                Success = true,
                Message = "Attendance report fetched successfully.",
                Data = pivotedAttendance,
                StatusCode = 200,
                TotalCount = pivotedAttendance.Count()
            };
        }
    }
}
