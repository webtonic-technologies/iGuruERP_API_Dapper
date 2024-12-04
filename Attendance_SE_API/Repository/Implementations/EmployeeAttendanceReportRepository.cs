using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using Dapper;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

            // Fetch Status Mapping Dynamically
            var statusMappings = await _connection.QueryAsync<AttendanceStatusMapping>(
                "SELECT StatusID, ShortName FROM tblEmployeeAttendanceStatus WHERE IsActive = 1");

            // Create a dictionary for easy lookup of StatusID to ShortName
            var statusDict = statusMappings.ToDictionary(x => x.StatusID, x => x.ShortName);

            string query = @"
            WITH AttendanceData AS (
                SELECT 
                    e.Employee_id AS EmployeeID, 
                    e.Employee_code_id AS EmployeeCode,  
                    CONCAT(e.First_Name, ' ', e.Last_Name) AS EmployeeName, 
                    e.mobile_number AS MobileNumber,  
                    a.AttendanceDate,
                    a.StatusID
                FROM tbl_EmployeeProfileMaster e
                LEFT JOIN tblEmployeeAttendance a ON e.Employee_id = a.EmployeeID
                    AND a.AttendanceDate BETWEEN @StartDate AND @EndDate
                WHERE e.Department_id = @DepartmentID  
                    AND e.Institute_id = @InstituteID
            )
            SELECT 
                EmployeeID,
                EmployeeCode, 
                EmployeeName,
                MobileNumber,
                AttendanceDate,
                CASE 
                    WHEN AttendanceStatus.StatusID IS NOT NULL THEN 
                        AttendanceStatus.ShortName
                    ELSE '-'
                END AS AttendanceStatus
            FROM AttendanceData a
            LEFT JOIN tblEmployeeAttendanceStatus AttendanceStatus ON a.StatusID = AttendanceStatus.StatusID
            ORDER BY EmployeeName, AttendanceDate;";

            var parameters = new
            {
                StartDate = parsedStartDate,
                EndDate = parsedEndDate,
                DepartmentID = request.DepartmentID,
                InstituteID = request.InstituteID
            };

            var attendanceRecords = await _connection.QueryAsync<AttendanceRecord2>(query, parameters);

            // Generate the attendance report response, mapping StatusID to the ShortName dynamically
            var dateRange = Enumerable.Range(0, (parsedEndDate - parsedStartDate).Days + 1)
                        .Select(d => parsedStartDate.AddDays(d))
                        .ToList();

            var pivotedAttendance = attendanceRecords
                .GroupBy(x => new { x.EmployeeID, x.EmployeeCode, x.EmployeeName, x.MobileNumber })
                .Select(g => new AttendanceDetailResponse_EMP
                {
                    EmployeeID = g.Key.EmployeeID,
                    EmployeeCode = g.Key.EmployeeCode,
                    EmployeeName = g.Key.EmployeeName,
                    MobileNumber = g.Key.MobileNumber,
                    WorkingDays = g.Count(),
                    Present = g.Count(x => statusDict.ContainsKey(x.StatusID) && statusDict[x.StatusID] == "P"),
                    Absent = g.Count(x => statusDict.ContainsKey(x.StatusID) && statusDict[x.StatusID] == "A"),
                    AttendancePercentage = Math.Round((double)g.Count(x => statusDict.ContainsKey(x.StatusID) && statusDict[x.StatusID] == "P") / g.Count() * 100, 2),
                    AttendanceList = dateRange.Select(date => new AttendanceDateInfo1
                    {
                        AttendanceDate = date.ToString("MMM dd"),  // Format as "Dec 01"
                        AttendanceDay = date.ToString("ddd"),     // Day of the week, e.g., "Sun"
                        AttendanceStatus = g.FirstOrDefault(x => x.AttendanceDate.Date == date.Date)?.AttendanceStatus ?? "-"  // Use attendance status or "-"
                    }).ToList()
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


        //public async Task<EmployeeAttendanceReportResponse> GetAttendanceReport(EmployeeAttendanceReportRequest request)
        //{
        //    DateTime parsedStartDate;
        //    DateTime parsedEndDate;

        //    // Parse the start and end dates
        //    if (!DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedStartDate) ||
        //        !DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedEndDate))
        //    {
        //        return new EmployeeAttendanceReportResponse
        //        {
        //            Success = false,
        //            Message = "Invalid date format. Please use DD-MM-YYYY.",
        //            Data = null,
        //            StatusCode = 400,
        //            TotalCount = 0
        //        };
        //    }


        //    // Fetch Status Mapping Dynamically
        //    var statusMappings = await _connection.QueryAsync<AttendanceStatusMapping>(
        //        "SELECT StatusID, ShortName FROM tblEmployeeAttendanceStatus WHERE IsActive = 1");

        //    // Create a dictionary for easy lookup of StatusID to ShortName
        //    var statusDict = statusMappings.ToDictionary(x => x.StatusID, x => x.ShortName);

        //    string query = @"
        //    WITH AttendanceData AS (
        //        SELECT 
        //            e.Employee_id AS EmployeeID, 
        //            e.Employee_code_id AS EmployeeCode,  
        //            CONCAT(e.First_Name, ' ', e.Last_Name) AS EmployeeName, 
        //            e.mobile_number AS MobileNumber,  
        //            a.AttendanceDate,
        //            a.StatusID
        //        FROM tbl_EmployeeProfileMaster e
        //        LEFT JOIN tblEmployeeAttendance a ON e.Employee_id = a.EmployeeID
        //            AND a.AttendanceDate BETWEEN @StartDate AND @EndDate
        //        WHERE e.Department_id = @DepartmentID  
        //            AND e.Institute_id = @InstituteID
        //    )
        //    SELECT 
        //        EmployeeID,
        //        EmployeeCode, 
        //        EmployeeName,
        //        MobileNumber,
        //        AttendanceDate,
        //        CASE 
        //            WHEN AttendanceStatus.StatusID IS NOT NULL THEN 
        //                AttendanceStatus.ShortName
        //            ELSE '-'
        //        END AS AttendanceStatus
        //    FROM AttendanceData a
        //    LEFT JOIN tblEmployeeAttendanceStatus AttendanceStatus ON a.StatusID = AttendanceStatus.StatusID
        //    ORDER BY EmployeeName, AttendanceDate;";

        //    var parameters = new
        //    {
        //        StartDate = parsedStartDate,
        //        EndDate = parsedEndDate,
        //        DepartmentID = request.DepartmentID,
        //        InstituteID = request.InstituteID
        //    };

        //    var attendanceRecords = await _connection.QueryAsync<AttendanceRecord2>(query, parameters);

        //    // Generate the attendance report response, mapping StatusID to the ShortName dynamically
        //    var dateRange = Enumerable.Range(0, (parsedEndDate - parsedStartDate).Days + 1)
        //                .Select(d => parsedStartDate.AddDays(d))
        //                .ToList();

        //    var pivotedAttendance = attendanceRecords
        //        .GroupBy(x => new { x.EmployeeID, x.EmployeeCode, x.EmployeeName, x.MobileNumber })
        //        .Select(g => new AttendanceDetailResponse_EMP
        //        {
        //            EmployeeID = g.Key.EmployeeID,
        //            EmployeeCode = g.Key.EmployeeCode,
        //            EmployeeName = g.Key.EmployeeName,
        //            MobileNumber = g.Key.MobileNumber,
        //            WorkingDays = g.Count(),
        //            Present = g.Count(x => statusDict.ContainsKey(x.StatusID) && statusDict[x.StatusID] == "P"),
        //            Absent = g.Count(x => statusDict.ContainsKey(x.StatusID) && statusDict[x.StatusID] == "A"),
        //            AttendancePercentage = Math.Round((double)g.Count(x => statusDict.ContainsKey(x.StatusID) && statusDict[x.StatusID] == "P") / g.Count() * 100, 2),
        //            Attendance = dateRange.ToDictionary(
        //                date => date.ToString("MMM dd, ddd"),  // Format as "Sep 29, Sun"
        //                date => g.FirstOrDefault(x => x.AttendanceDate.Date == date.Date)?.AttendanceStatus ?? "-"  // Use attendance status or "-"
        //            )
        //        }).ToList();

        //    // Create the final response
        //    return new EmployeeAttendanceReportResponse
        //    {
        //        Success = true,
        //        Message = "Attendance report fetched successfully.",
        //        Data = pivotedAttendance,
        //        StatusCode = 200,
        //        TotalCount = pivotedAttendance.Count()
        //    };
        //}


        public async Task<IEnumerable<GetAttendanceGeoFencingReportResponse>> GetAttendanceGeoFencingReport(GetAttendanceGeoFencingReportRequest request)
        {
            var query = @"
            SELECT 
                ge.EmployeeID,
                CONCAT(e.First_Name, ' ', e.Last_Name) AS EmployeeName,
                e.mobile_number AS PrimaryMobileNo, 
                ge.GeoFencingDate,
                ge.ClockIn,
                ge.ClockOut,
                ge.Reason,
                DATEDIFF(MINUTE, ge.ClockIn, ge.ClockOut) / 60 AS HoursWorked,
                DATEDIFF(MINUTE, ge.ClockIn, ge.ClockOut) % 60 AS MinutesWorked
            FROM 
                tblGeoFencingEntry ge
            JOIN 
                tbl_EmployeeProfileMaster e ON ge.EmployeeID = e.Employee_id 
            WHERE 
                ge.GeoFencingDate BETWEEN @StartDate AND @EndDate
                AND e.Institute_id = @InstituteID
            ORDER BY 
                ge.GeoFencingDate, e.Employee_id";

            var result = await _connection.QueryAsync<dynamic>(query, new
            {
                InstituteID = request.InstituteID,
                StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null),
                EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null)
            });

            // Define the start and end date for the range (20 Nov to 30 Nov)
            var startDate = DateTime.ParseExact("20-11-2024", "dd-MM-yyyy", null);
            var endDate = DateTime.ParseExact("30-11-2024", "dd-MM-yyyy", null);

            // Create a list of all dates between startDate and endDate
            var allDatesInRange = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .Select(offset => startDate.AddDays(offset).ToString("MMM dd, ddd")) // Format as "Nov 20, Mon"
                .ToList();

            // Group the results by employee and ensure every date in the range is represented
            var groupedResults = result
                .GroupBy(r => new { r.EmployeeID, r.EmployeeName, r.PrimaryMobileNo })
                .Select(g => new GetAttendanceGeoFencingReportResponse
                {
                    EmployeeID = g.Key.EmployeeID,
                    EmployeeName = g.Key.EmployeeName,
                    PrimaryMobileNumber = g.Key.PrimaryMobileNo,
                    Attendance = allDatesInRange.Select(date =>
                    {
                        // Check if attendance exists for the specific date
                        var attendanceRecord = g.FirstOrDefault(x => DateTime.Parse(x.GeoFencingDate.ToString()).ToString("MMM dd, ddd") == date);

                        if (attendanceRecord != null)
                        {
                            // Format ClockIn and ClockOut to 12-hour format with AM/PM
                            var clockInTime = DateTime.Parse(attendanceRecord.ClockIn.ToString()).ToString("hh:mm tt");
                            var clockOutTime = DateTime.Parse(attendanceRecord.ClockOut.ToString()).ToString("hh:mm tt");

                            return new AttendanceDetails
                            {
                                Date = date, // Display the date
                                ClockIn = clockInTime, // Formatted ClockIn
                                ClockOut = clockOutTime, // Formatted ClockOut
                                Time = $"{attendanceRecord.HoursWorked} Hours {attendanceRecord.MinutesWorked} Minutes"
                            };
                        }
                        else
                        {
                            // If no attendance for that day, return default values
                            return new AttendanceDetails
                            {
                                Date = date, // Display the date
                                ClockIn = "N/A", // No attendance
                                ClockOut = "N/A", // No attendance
                                Time = "0 Hours 0 Minutes" // Default time
                            };
                        }

                    }).ToList()
                }).ToList();

            return groupedResults;
        }
         
        /////
        ///Working

        //public async Task<IEnumerable<GetAttendanceGeoFencingReportResponse>> GetAttendanceGeoFencingReport(GetAttendanceGeoFencingReportRequest request)
        //{
        //    var query = @"
        //SELECT 
        //    ge.EmployeeID,
        //    CONCAT(e.First_Name, ' ', e.Last_Name) AS EmployeeName,
        //    e.mobile_number AS PrimaryMobileNo, 
        //    ge.GeoFencingDate,
        //    ge.ClockIn,
        //    ge.ClockOut,
        //    ge.Reason,
        //    DATEDIFF(MINUTE, ge.ClockIn, ge.ClockOut) / 60 AS HoursWorked,
        //    DATEDIFF(MINUTE, ge.ClockIn, ge.ClockOut) % 60 AS MinutesWorked
        //FROM 
        //    tblGeoFencingEntry ge
        //JOIN 
        //    tbl_EmployeeProfileMaster e ON ge.EmployeeID = e.Employee_id 
        //WHERE 
        //    ge.GeoFencingDate BETWEEN @StartDate AND @EndDate
        //    AND e.Institute_id = @InstituteID
        //ORDER BY 
        //    ge.GeoFencingDate, e.Employee_id";

        //    var result = await _connection.QueryAsync<dynamic>(query, new
        //    {
        //        InstituteID = request.InstituteID,
        //        StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null),
        //        EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null)
        //    });

        //    var groupedResults = result
        //    .GroupBy(r => new { r.EmployeeID, r.EmployeeName, r.PrimaryMobileNo })
        //    .Select(g => new GetAttendanceGeoFencingReportResponse
        //    {
        //        EmployeeID = g.Key.EmployeeID,
        //        EmployeeName = g.Key.EmployeeName,
        //        PrimaryMobileNumber = g.Key.PrimaryMobileNo,
        //        Attendance = g.Select(x =>
        //        {
        //            // Debugging log for each entry
        //            try
        //            {
        //                // Parsing GeoFencingDate, ClockIn, and ClockOut
        //                var parsedDate = DateTime.Parse(x.GeoFencingDate.ToString());
        //                var parsedClockIn = TimeSpan.Parse(x.ClockIn.ToString());
        //                var parsedClockOut = TimeSpan.Parse(x.ClockOut.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                // Log the problematic row or error
        //                Console.WriteLine($"Error parsing row: {ex.Message}");
        //                // Optionally, log the problematic row itself
        //                Console.WriteLine($"Problematic row: {x}");
        //            }


        //            return new AttendanceDetails
        //            {
        //                // Format GeoFencingDate as "Nov 20, Wed"
        //                Date = x.GeoFencingDate.ToString("MMM dd, ddd"), // Using the DateTime format "MMM dd, ddd"

        //                // Ensure ClockIn and ClockOut are formatted as "hh:mm"
        //                ClockIn = x.ClockIn.ToString(@"hh\:mm"), // TimeSpan formatting for "hh:mm"
        //                ClockOut = x.ClockOut.ToString(@"hh\:mm"), // TimeSpan formatting for "hh:mm"

        //                // Format worked time as "X Hours Y Minutes"
        //                Time = $"{x.HoursWorked} Hours {x.MinutesWorked} Minutes"
        //            };

        //        }).ToList()
        //    }).ToList(); // Ensure we return a list (IEnumerable)

        //    return groupedResults; 
        //}

        public async Task<MemoryStream> GenerateExcelReport(GetAttendanceGeoFencingReportRequest request)
        {
            // Fetch the data first
            var data = await GetAttendanceGeoFencingReport(request);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("AttendanceReport");

                // Set header row in Excel sheet
                worksheet.Cells[1, 1].Value = "EmployeeID";
                worksheet.Cells[1, 2].Value = "Employee Name";
                worksheet.Cells[1, 3].Value = "Primary Mobile No.";

                // For each attendance date, add a column (e.g., Jan 07, Sun)
                var dateColumns = new List<string>();
                foreach (var item in data)
                {
                    foreach (var attendance in item.Attendance)
                    {
                        if (!dateColumns.Contains(attendance.Date))
                        {
                            dateColumns.Add(attendance.Date);
                        }
                    }
                }

                // Add the date columns in the header with three columns per date (Clock In, Clock Out, Time)
                int col = 4; // Start from the fourth column
                foreach (var date in dateColumns)
                {
                    worksheet.Cells[1, col].Value = date;  // Date column header
                    col++;
                }

                // Fill data for each employee
                int row = 2;  // Start from the second row, as the first row is the header

                foreach (var item in data)
                {
                    // Employee details
                    worksheet.Cells[row, 1].Value = item.EmployeeID;
                    worksheet.Cells[row, 2].Value = item.EmployeeName;
                    worksheet.Cells[row, 3].Value = item.PrimaryMobileNumber;

                    // Fill in attendance data for each date
                    int columnOffset = 0;  // Track the column offset based on the dates
                    foreach (var attendance in item.Attendance)
                    {
                        // Concatenate Clock In, Clock Out, and Time into a single string
                        string attendanceDetails = $"Clock In : {attendance.ClockIn}, Clock Out : {attendance.ClockOut}, Time : {attendance.Time}";

                        // Find the correct column for the attendance date
                        int dateColumnIndex = dateColumns.IndexOf(attendance.Date) + 4;

                        // Fill the concatenated data for the specific date
                        worksheet.Cells[row, dateColumnIndex].Value = attendanceDetails;

                        columnOffset++;
                    }

                    row++;
                }

                // Save the Excel file to a memory stream
                var stream = new MemoryStream();
                await package.SaveAsAsync(stream);
                stream.Position = 0;
                return stream;
            }
        }

        public async Task<List<AttendanceMode>> GetAttendanceModes()
        {
            string query = "SELECT AttendanceModeID, AttendanceMode as AttendanceModeName FROM tblAttendanceMode";
            var attendanceModes = await _connection.QueryAsync<AttendanceMode>(query);
            return attendanceModes.AsList();
        }
    }
}
