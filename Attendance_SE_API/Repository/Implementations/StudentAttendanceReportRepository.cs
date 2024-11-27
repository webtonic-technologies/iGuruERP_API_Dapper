using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.Repository.Interfaces;
using Dapper;
using OfficeOpenXml;

namespace Attendance_SE_API.Repository.Implementations
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
            try
            {
                DateTime parsedStartDate;
                DateTime parsedEndDate;

                // Validate date range format for DD-MM-YYYY
                if (!DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedStartDate) ||
                    !DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedEndDate))
                {
                    throw new ArgumentException("Invalid date format. Please use DD-MM-YYYY.");
                }

                // Query to fetch attendance data along with mobile number from tbl_StudentParentsInfo
                string query = @"
                WITH AttendanceData AS (
                    SELECT 
                        s.student_id AS StudentID, 
                        s.Admission_Number AS AdmissionNumber, 
                        s.Roll_Number AS RollNumber, 
                        CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName, 
                        sp.Contact_Number AS MobileNumber,  -- Fetch mobile number from tbl_StudentParentsInfo
                        a.AttendanceDate,
                        sas.StatusName AS AttendanceStatus  -- Dynamically fetch StatusName from tblStudentAttendanceStatus
                    FROM tbl_StudentMaster s
                    LEFT JOIN tblStudentAttendance a ON s.student_id = a.StudentID 
                        AND a.AttendanceDate BETWEEN @StartDate AND @EndDate
                        AND a.TimeSlotTypeID = @TimeSlotTypeID  -- Include TimeSlotTypeID in the query
                    LEFT JOIN tbl_StudentParentsInfo sp ON s.student_id = sp.Student_id AND sp.Parent_Type_id = 1 -- Join with tbl_StudentParentsInfo to fetch Contact_Number
                    LEFT JOIN tblStudentAttendanceStatus sas ON a.StatusID = sas.StatusID -- Join to get the StatusName dynamically
                    WHERE s.class_id = @ClassID 
                        AND s.section_id = @SectionID 
                        AND s.Institute_id = @InstituteID
                )
                SELECT 
                    StudentID,
                    AdmissionNumber,
                    RollNumber,
                    StudentName,
                    MobileNumber,
                    AttendanceDate,
                    AttendanceStatus
                FROM AttendanceData
                ORDER BY StudentName, AttendanceDate;";

                // Query parameters
                var parameters = new
                {
                    StartDate = parsedStartDate,
                    EndDate = parsedEndDate,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID,
                    InstituteID = request.InstituteID,
                    TimeSlotTypeID = request.TimeSlotTypeID // Added TimeSlotTypeID
                };

                // Step 4: Execute the dynamic query
                var attendanceRecords = await _connection.QueryAsync<AttendanceRecord1>(query, parameters); // Ensure this matches your actual DTO

                // Create a date range from start to end
                var dateRange = Enumerable.Range(0, (parsedEndDate - parsedStartDate).Days + 1)
                    .Select(d => parsedStartDate.AddDays(d))
                    .ToList();

                // Create a pivot structure
                var pivotedAttendance = attendanceRecords
                    .GroupBy(x => new { x.StudentID, x.AdmissionNumber, x.RollNumber, x.StudentName, x.MobileNumber })
                    .Select(g => new AttendanceDetailResponse
                    {
                        StudentID = g.Key.StudentID,
                        AdmissionNumber = g.Key.AdmissionNumber ?? "-", // Ensure AdmissionNumber is populated
                        RollNumber = g.Key.RollNumber ?? "-",           // Ensure RollNumber is populated
                        StudentName = g.Key.StudentName,
                        MobileNumber = g.Key.MobileNumber,  // Include MobileNumber in the response
                        WorkingDays = g.Count(),  // Total working days based on records
                        Present = g.Count(x => x.AttendanceStatus == "Present"),
                        Absent = g.Count(x => x.AttendanceStatus == "Absent"),
                        AttendancePercentage = Math.Round((double)g.Count(x => x.AttendanceStatus == "Present") / g.Count() * 100, 2),
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
            catch (Exception ex)
            {
                // Catch any errors and return a custom error response
                return new StudentAttendanceReportResponse
                {
                    Success = false,
                    Message = $"An error occurred while fetching the attendance report: {ex.Message}",
                    Data = new List<AttendanceDetailResponse>(),
                    StatusCode = 500,
                    TotalCount = 0
                };
            }
        }

         
        public async Task<StudentAttendanceReportPeriodWiseResponse> GetAttendanceReportPeriodWise(StudentAttendanceReportPeriodWiseRequest request)
        {
            try
            {
                // Step 1: Get the subject names dynamically from the tbl_Subjects table
                var subjectsQuery = @"
            SELECT SubjectName
            FROM tbl_Subjects
            WHERE InstituteId = @InstituteID AND IsDeleted = 0";  // Ensure only active subjects are fetched

                var subjectNames = (await _connection.QueryAsync<string>(subjectsQuery, new { request.InstituteID })).ToList();

                // Step 2: Dynamically build the attendance columns for each subject
                var attendanceColumns = string.Join(", ", subjectNames.Select(sub =>
                    $"MAX(CASE WHEN sub.SubjectName = '{sub}' THEN COALESCE(CASE WHEN a.StatusID IS NULL THEN '-' ELSE 'P' END, '-') ELSE '-' END) AS {sub}"));

                // Step 3: Construct the full dynamic query without WorkingDays, Present, Absent, and AttendancePercentage
                string query = $@"
            SELECT
                s.student_id AS StudentID,
                s.Admission_Number AS AdmissionNumber,
                s.Roll_Number AS RollNumber,
                s.First_Name + ' ' + s.Last_Name as StudentName,
                p.Contact_Number as MobileNumber,
                {attendanceColumns}  -- Dynamic attendance columns for subjects
            FROM tbl_StudentMaster s
            LEFT JOIN tblStudentAttendance a
                ON s.student_id = a.StudentID
                AND a.AttendanceTypeID = 2
                AND a.AttendanceDate = CONVERT(DATE, @Date, 105)  -- Filter by AttendanceDate in JOIN
            LEFT JOIN tbl_StudentParentsInfo p
                ON s.student_id = p.student_id
                AND p.Parent_Type_id = 1
            LEFT JOIN tbl_Subjects sub
                ON sub.SubjectID = a.SubjectID  -- Join with subjects
            WHERE s.class_id = @ClassID
              AND s.section_id = @SectionID
              AND s.Institute_id = @InstituteID
            GROUP BY s.student_id, s.Admission_Number, s.Roll_Number, s.First_Name, s.Last_Name, p.Contact_Number;
        ";

                // Query parameters (removed AttendanceTypeID and SubjectID)
                var parameters = new
                {
                    request.ClassID,
                    request.SectionID,
                    request.InstituteID,
                    Date = request.Date // Assuming Date is in string format (e.g., '25-11-2024')
                };

                // Step 4: Execute the dynamic query
                var attendanceRecords = await _connection.QueryAsync(query, parameters);

                // Step 5: Map the results to the response, dynamically generating the attendance for each subject
                var response = new StudentAttendanceReportPeriodWiseResponse
                {
                    Success = true,
                    Message = "Attendance report fetched successfully.",
                    Data = attendanceRecords.Select(record => new AttendanceDetailResponse_PW
                    {
                        StudentID = record.StudentID,
                        AdmissionNumber = record.AdmissionNumber,
                        RollNumber = record.RollNumber,
                        StudentName = record.StudentName,
                        MobileNumber = record.MobileNumber,
                        // Dynamically populate attendance for each subject, replace NULL with "-"
                        Attendance = subjectNames.ToDictionary(
                        subject => subject,
                        subject =>
                        {
                            // Ensure that record is not null before attempting to access its properties
                            if (record == null)
                            {
                                return subject; // If record is null, return the subject name
                            }

                            // Dynamically fetch the subject attendance, check if the key exists and return "-" if null or missing
                            var value = ((IDictionary<string, object>)record).ContainsKey(subject) ? ((IDictionary<string, object>)record)[subject] : "-";

                            // Return the value if not null, otherwise fallback to "-"
                            return value != null ? value.ToString() : "-";
                        }
                        )
                    }).ToList(),
                    StatusCode = 200,
                    TotalCount = attendanceRecords.Count()
                };

                return response;
            }
            catch (Exception ex)
            {
                // Catch any errors and return a custom error response
                return new StudentAttendanceReportPeriodWiseResponse
                {
                    Success = false,
                    Message = $"An error occurred while fetching the attendance report: {ex.Message}",
                    Data = new List<AttendanceDetailResponse_PW>(),
                    StatusCode = 500,
                    TotalCount = 0
                };
            }
        }

        public async Task<byte[]> GetAttendanceReportExport(StudentAttendanceReportRequest request)
        {
            try
            {
                DateTime parsedStartDate;
                DateTime parsedEndDate;

                // Validate date range format for DD-MM-YYYY
                if (!DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedStartDate) ||
                    !DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedEndDate))
                {
                    throw new ArgumentException("Invalid date format. Please use DD-MM-YYYY.");
                }

                // Query to fetch attendance data along with mobile number from tbl_StudentParentsInfo
                string query = @"
        WITH AttendanceData AS (
            SELECT 
                s.student_id AS StudentID, 
                s.Admission_Number AS AdmissionNumber, 
                s.Roll_Number AS RollNumber, 
                CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName, 
                sp.Contact_Number AS MobileNumber,  -- Fetch mobile number from tbl_StudentParentsInfo
                a.AttendanceDate,
                sas.StatusName AS AttendanceStatus  -- Dynamically fetch StatusName from tblStudentAttendanceStatus
            FROM tbl_StudentMaster s
            LEFT JOIN tblStudentAttendance a ON s.student_id = a.StudentID AND a.AttendanceTypeID = 1
                AND a.AttendanceDate BETWEEN @StartDate AND @EndDate
                AND a.TimeSlotTypeID = @TimeSlotTypeID  -- Include TimeSlotTypeID in the query
            LEFT JOIN tbl_StudentParentsInfo sp ON s.student_id = sp.Student_id AND sp.Parent_Type_id = 1 -- Join with tbl_StudentParentsInfo to fetch Contact_Number
            LEFT JOIN tblStudentAttendanceStatus sas ON a.StatusID = sas.StatusID -- Join to get the StatusName dynamically
            WHERE s.class_id = @ClassID 
                AND s.section_id = @SectionID 
                AND s.Institute_id = @InstituteID
        )
        SELECT 
            StudentID,
            AdmissionNumber,
            RollNumber,
            StudentName,
            MobileNumber,
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
                    TimeSlotTypeID = request.TimeSlotTypeID // Added TimeSlotTypeID
                };

                // Execute the query and get the attendance records
                var attendanceRecords = await _connection.QueryAsync<AttendanceRecord1>(query, parameters);

                // Create a date range from start to end
                var dateRange = Enumerable.Range(0, (parsedEndDate - parsedStartDate).Days + 1)
                    .Select(d => parsedStartDate.AddDays(d))
                    .ToList();

                // Create an Excel package using EPPlus
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("AttendanceReport");

                    // Set the header row
                    worksheet.Cells[1, 1].Value = "Admission Number";
                    worksheet.Cells[1, 2].Value = "Roll Number";
                    worksheet.Cells[1, 3].Value = "Student Name";
                    worksheet.Cells[1, 4].Value = "Mobile Number";
                    worksheet.Cells[1, 5].Value = "Working Days";
                    worksheet.Cells[1, 6].Value = "Present";
                    worksheet.Cells[1, 7].Value = "Absent";
                    worksheet.Cells[1, 8].Value = "Attendance Percentage";

                    // Add dynamic date columns for attendance
                    var dateColumnIndex = 9; // Start after the static columns
                    foreach (var date in dateRange)
                    {
                        worksheet.Cells[1, dateColumnIndex++].Value = date.ToString("MMM dd, ddd");
                    }

                    // Populate data starting from row 2
                    var row = 2;
                    foreach (var record in attendanceRecords)
                    {
                        // Calculate attendance for each student
                        var attendanceCount = dateRange.Count(date =>
                            record.AttendanceDate.Date == date.Date && record.AttendanceStatus == "Present");

                        var workingDays = dateRange.Count(date => record.AttendanceDate.Date == date.Date);
                        var attendancePercentage = workingDays > 0 ? Math.Round((double)attendanceCount / workingDays * 100, 2) : 0;

                        worksheet.Cells[row, 1].Value = record.AdmissionNumber;
                        worksheet.Cells[row, 2].Value = record.RollNumber;
                        worksheet.Cells[row, 3].Value = record.StudentName;
                        worksheet.Cells[row, 4].Value = record.MobileNumber;
                        worksheet.Cells[row, 5].Value = workingDays;
                        worksheet.Cells[row, 6].Value = attendanceCount;
                        worksheet.Cells[row, 7].Value = workingDays - attendanceCount;
                        worksheet.Cells[row, 8].Value = attendancePercentage;

                        // Add dynamic attendance values for each date
                        var dateColIndex = 9;
                        foreach (var date in dateRange)
                        {
                            var attendanceStatus = record.AttendanceDate.Date == date.Date ? record.AttendanceStatus : "-";
                            worksheet.Cells[row, dateColIndex++].Value = attendanceStatus;
                        }

                        row++;
                    }

                    // Return the Excel file as byte array
                    return package.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while exporting the attendance report: {ex.Message}");
            }
        }

        public async Task<byte[]> GetAttendanceReportPeriodWiseExport(StudentAttendanceReportPeriodWiseRequest request)
        {
            try
            {
                DateTime parsedStartDate;
                DateTime parsedEndDate;

                // Validate date range format for DD-MM-YYYY
                if (!DateTime.TryParseExact(request.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedStartDate))
                {
                    throw new ArgumentException("Invalid date format. Please use DD-MM-YYYY.");
                }

                parsedEndDate = parsedStartDate; // Assuming it's a single day query

                // Step 1: Get the subject names dynamically from the tbl_Subjects table
                var subjectsQuery = @"
            SELECT SubjectName
            FROM tbl_Subjects
            WHERE InstituteId = @InstituteID AND IsDeleted = 0";  // Ensure only active subjects are fetched

                var subjectNames = (await _connection.QueryAsync<string>(subjectsQuery, new { request.InstituteID })).ToList();

                // Step 2: Dynamically build the attendance columns for each subject
                var attendanceColumns = string.Join(", ", subjectNames.Select(sub =>
                    $"MAX(CASE WHEN sub.SubjectName = '{sub}' THEN COALESCE(CASE WHEN a.StatusID IS NULL THEN '-' ELSE 'P' END, '-') ELSE '-' END) AS {sub}"));

                // Step 3: Construct the full dynamic query
                string query = $@"
            SELECT
                s.student_id AS StudentID,
                s.Admission_Number AS AdmissionNumber,
                s.Roll_Number AS RollNumber,
                s.First_Name + ' ' + s.Last_Name AS StudentName,
                p.Contact_Number AS MobileNumber,
                {attendanceColumns}  -- Dynamic attendance columns for subjects
            FROM tbl_StudentMaster s
            LEFT JOIN tblStudentAttendance a
                ON s.student_id = a.StudentID
                AND a.AttendanceTypeID = 2
                AND a.AttendanceDate = CONVERT(DATE, @Date, 105)  -- Filter by AttendanceDate in JOIN
            LEFT JOIN tbl_StudentParentsInfo p
                ON s.student_id = p.student_id
                AND p.Parent_Type_id = 1
            LEFT JOIN tbl_Subjects sub
                ON sub.SubjectID = a.SubjectID  -- Join with subjects
            WHERE s.class_id = @ClassID
              AND s.section_id = @SectionID
              AND s.Institute_id = @InstituteID
            GROUP BY s.student_id, s.Admission_Number, s.Roll_Number, s.First_Name, s.Last_Name, p.Contact_Number;
        ";

                var parameters = new
                {
                    request.ClassID,
                    request.SectionID,
                    request.InstituteID,
                    Date = parsedStartDate // Using the Date parameter from the request
                };

                // Step 4: Execute the dynamic query
                var attendanceRecords = await _connection.QueryAsync(query, parameters);

                // Step 5: Generate the Excel file using EPPlus
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("AttendanceReport");

                    // Set the header row
                    worksheet.Cells[1, 1].Value = "Admission Number";
                    worksheet.Cells[1, 2].Value = "Roll Number";
                    worksheet.Cells[1, 3].Value = "Student Name";
                    worksheet.Cells[1, 4].Value = "Mobile Number";

                    // Add dynamic columns for attendance per subject
                    var dateColumnIndex = 5;
                    foreach (var subject in subjectNames)
                    {
                        worksheet.Cells[1, dateColumnIndex++].Value = subject;
                    }

                    // Populate data starting from row 2
                    var row = 2;
                    foreach (var record in attendanceRecords)
                    {
                        worksheet.Cells[row, 1].Value = record.AdmissionNumber;
                        worksheet.Cells[row, 2].Value = record.RollNumber;
                        worksheet.Cells[row, 3].Value = record.StudentName;
                        worksheet.Cells[row, 4].Value = record.MobileNumber;

                        // Add dynamic attendance values for each subject
                        var attendanceColumnIndex = 5;
                        foreach (var subject in subjectNames)
                        {
                            //var value = record.GetType().GetProperty(subject)?.GetValue(record, null) ?? "-";
                            var value = "-";
                            worksheet.Cells[row, attendanceColumnIndex++].Value = value;
                        }

                        row++;
                    }

                    // Return the Excel file as byte array
                    return package.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while exporting the attendance report: {ex.Message}");
            }
        }
    }
}

 