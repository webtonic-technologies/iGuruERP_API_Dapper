using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Attendance_API.DTOs.Requests;
using Attendance_API.DTOs.Response;
using Attendance_API.Repository.Interfaces;
using Dapper;

namespace Attendance_API.Repository.Implementations
{
    public class StudentAttendanceReportRepository : IStudentAttendanceReportRepository
    {
        private readonly IDbConnection _connection;

        public StudentAttendanceReportRepository(IDbConnection connection)
        {
            _connection = connection;
        }



        public async Task<StudentAttendanceReportResponse> GetAttendanceReport(StudentAttendanceReportRequest request)
        {
            DateTime parsedStartDate;
            DateTime parsedEndDate;

            // Validate date range format for DD-MM-YYYY
            if (!DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedStartDate) ||
                !DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedEndDate))
            {
                throw new ArgumentException("Invalid date format. Please use DD-MM-YYYY.");
            }

            // Validate attendance type ID
            if (request.AttendanceTypeID == 1 && request.TimeSlotTypeID == null)
            {
                throw new ArgumentException("TimeSlotTypeID should not be null when AttendanceTypeID is 1.");
            }

            if (request.AttendanceTypeID == 2 && request.SubjectID == null)
            {
                throw new ArgumentException("SubjectID should not be null when AttendanceTypeID is 2.");
            }

            // Query to fetch attendance data
            string query = @"
WITH AttendanceData AS (
    SELECT 
        s.student_id, 
        s.Admission_Number, 
        s.Roll_Number, 
        CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName, 
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
    FROM tbl_StudentMaster s
    LEFT JOIN tblStudentAttendance a ON s.student_id = a.StudentID
        AND a.AttendanceDate BETWEEN @StartDate AND @EndDate
    WHERE s.class_id = @ClassID 
        AND s.section_id = @SectionID 
        AND s.Institute_id = @InstituteID
)
SELECT 
    student_id,
    Admission_Number,
    Roll_Number,
    StudentName,
    AttendanceDate,
    AttendanceStatus
FROM AttendanceData
ORDER BY StudentName, AttendanceDate;";

            var parameters = new
            {
                StartDate = parsedStartDate,
                EndDate = parsedEndDate,
                ClassID = request.ClassID,
                SectionID = request.SectionID,
                InstituteID = request.InstituteID,
                SubjectID = request.SubjectID,
                TimeSlotTypeID = request.TimeSlotTypeID,
                AttendanceTypeID = request.AttendanceTypeID
            };

            var attendanceRecords = await _connection.QueryAsync<AttendanceRecord1>(query, parameters); // Ensure this matches your actual DTO

            // Create a date range from start to end
            var dateRange = Enumerable.Range(0, (parsedEndDate - parsedStartDate).Days + 1)
                .Select(d => parsedStartDate.AddDays(d))
                .ToList();

            // Create a pivot structure
            var pivotedAttendance = attendanceRecords
                .GroupBy(x => new { x.StudentID, x.AdmissionNumber, x.RollNumber, x.StudentName })
                .Select(g => new AttendanceDetailResponse
                {
                    StudentID = g.Key.StudentID,
                    AdmissionNumber = g.Key.AdmissionNumber ?? "-", // Ensure AdmissionNumber is populated
                    RollNumber = g.Key.RollNumber ?? "-",           // Ensure RollNumber is populated
                    StudentName = g.Key.StudentName,
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
            var response = new StudentAttendanceReportResponse
            {
                Success = true,
                Message = "Attendance report fetched successfully.",
                Data = pivotedAttendance,
                StatusCode = 200,
                TotalCount = pivotedAttendance.Count()
            };

            return response;
        }
    }
}
