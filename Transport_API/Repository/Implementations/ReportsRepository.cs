using Dapper;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
using Transport_API.Repository.Interfaces;
using System.Data;
using Transport_API.DTOs.Response;
using OfficeOpenXml;
using System.Text;  // Required for StringBuilder and Encoding
using System.Linq;  // For LINQ usage (if not already present)


namespace Transport_API.Repository.Implementations
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly IDbConnection _dbConnection;

        public ReportsRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<IEnumerable<ReportResponse>>> GetTransportPendingFeeReport(GetReportsRequest request)
        {
            string sql = @"SELECT ReportType, ReportData FROM tbl_Reports WHERE ReportType = 'TransportPendingFee' ORDER BY ReportId OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var reports = await _dbConnection.QueryAsync<ReportResponse>(sql, new { Offset = (request.PageNumber - 1) * request.PageSize, PageSize = request.PageSize });

            if (reports.Any())
            {
                return new ServiceResponse<IEnumerable<ReportResponse>>(true, "Records Found", reports, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<IEnumerable<ReportResponse>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }


        public async Task<ServiceResponse<GetEmployeeTransportationReportResponse>> GetEmployeeTransportationReport(GetReportsRequest request)
        {
            // Fetch vehicle details for the given RoutePlanID and InstituteID
            var vehicleDetails = await GetEmployeeVehicleDetails(request.RoutePlanID.Value, request.InstituteID.Value);

            if (vehicleDetails == null)
            {
                return new ServiceResponse<GetEmployeeTransportationReportResponse>(false, "No Vehicle Found", null, StatusCodes.Status204NoContent);
            }

            // Fetch the total number of employees assigned to the route
            int totalEmployeeCount = await GetTotalEmployeeCount(request.RoutePlanID.Value, vehicleDetails.VehicleID);

            // Fetch employee details for the route
            var employees = await GetEmployeesForRoute(request.RoutePlanID.Value, request.Search);

            // Prepare the response object
            var response = new GetEmployeeTransportationReportResponse
            {
                VehicleID = vehicleDetails.VehicleID,
                VehicleNo = vehicleDetails.VehicleNo,
                VehicleType = vehicleDetails.VehicleType,
                CoordinatorName = vehicleDetails.CoordinatorName,
                DriverName = vehicleDetails.DriverName,
                DriverNumber = vehicleDetails.DriverNumber,
                TotalCount = totalEmployeeCount,
                Employees = employees.ToList()
            };

            return new ServiceResponse<GetEmployeeTransportationReportResponse>(true, "Records Found", response, StatusCodes.Status200OK);
        }



        public async Task<EmployeeVehicleDetails> GetEmployeeVehicleDetails(int routePlanID, int instituteID)
        {
            string sql = @"
        SELECT 
            vm.VehicleID, 
            vm.VehicleNumber AS VehicleNo, 
            vt.Vehicle_type_name AS VehicleType, 
            epDriver.First_Name + ' ' + epDriver.Last_Name AS DriverName, 
            epDriver.mobile_number AS DriverNumber,
            epCoordinator.First_Name + ' ' + epCoordinator.Last_Name AS CoordinatorName
        FROM 
            tblVehicleMaster vm
        JOIN 
            tbl_Vehicle_Type vt ON vm.VehicleTypeID = vt.Vehicle_type_id
        LEFT JOIN 
            tblAssignRoute ar ON ar.VehicleID = vm.VehicleID
        LEFT JOIN 
            tbl_EmployeeProfileMaster epDriver ON ar.DriverID = epDriver.Employee_id
        LEFT JOIN 
            tbl_EmployeeProfileMaster epCoordinator ON ar.TransportStaffID = epCoordinator.Employee_id
        WHERE 
            ar.RoutePlanID = @RoutePlanID
            AND vm.InstituteID = @InstituteID";

            return await _dbConnection.QueryFirstOrDefaultAsync<EmployeeVehicleDetails>(sql, new { RoutePlanID = routePlanID, InstituteID = instituteID });
        }

        public async Task<int> GetTotalEmployeeCount(int routePlanID, int vehicleID)
        {
            string sql = @"
            SELECT 
                COUNT(DISTINCT esm.EmployeeID) AS TotalCount
            FROM 
                tblEmployeeStopMapping esm
            JOIN 
                tblRouteStopMaster rsm ON esm.StopID = rsm.StopID
            JOIN 
                tblAssignRoute ar ON rsm.RoutePlanID = ar.RoutePlanID
            WHERE 
            ar.VehicleID = @VehicleID
            AND ar.RoutePlanID = @RoutePlanID";

            return await _dbConnection.ExecuteScalarAsync<int>(sql, new { RoutePlanID = routePlanID, VehicleID = vehicleID });
        }


        //public async Task<IEnumerable<EmployeeDetails>> GetEmployeesForRoute(int routePlanID, string Search)
        //{
        //    // Base SQL query to fetch employee details
        //    string sql = @"
        //SELECT 
        //    ep.Employee_code_id AS EmployeeID,
        //    CONCAT(ep.First_Name, ' ', ep.Last_Name) AS EmployeeName,
        //    d.DepartmentName AS Department,
        //    des.DesignationName AS Designation,
        //    ep.mobile_number AS MobileNumber,
        //    rsm.StopName AS StopName
        //FROM 
        //    tblEmployeeStopMapping esm
        //JOIN 
        //    tbl_EmployeeProfileMaster ep ON esm.EmployeeID = ep.Employee_id
        //JOIN 
        //    tbl_Department d ON ep.Department_id = d.Department_id
        //JOIN 
        //    tbl_Designation des ON ep.Designation_id = des.Designation_id
        //JOIN 
        //    tblRouteStopMaster rsm ON esm.StopID = rsm.StopID
        //WHERE 
        //    rsm.RoutePlanID = @RoutePlanID";

        //    // Add the search condition if Search is provided
        //    if (!string.IsNullOrEmpty(Search))
        //    {
        //        sql += " AND CONCAT(ep.First_Name, ' ', ep.Last_Name) LIKE @Search";
        //    }

        //    // Execute the query
        //    return await _dbConnection.QueryAsync<EmployeeDetails>(sql, new { RoutePlanID = routePlanID, Search = "%" + Search + "%" });
        //}

        public async Task<IEnumerable<EmployeeDetails>> GetEmployeesForRoute(int routePlanID, string Search)
        {
            // Base SQL query to fetch employee details along with the Transport Fee calculation
            string sql = @"
            SELECT 
                ep.Employee_code_id AS EmployeeID,
                CONCAT(ep.First_Name, ' ', ep.Last_Name) AS EmployeeName,
                d.DepartmentName AS Department,
                des.DesignationName AS Designation,
                ep.mobile_number AS MobileNumber,
                rsm.StopName AS StopName,
                -- Adding Transport Fee calculation
                CASE 
                    WHEN rsf.FeesAmount IS NOT NULL THEN rsf.FeesAmount 
                    WHEN rtf.FeesAmount IS NOT NULL THEN rtf.FeesAmount
                    WHEN rmf.FeesAmount IS NOT NULL THEN rmf.FeesAmount 
                    ELSE 0 
                END AS TransportFee
            FROM 
                tblEmployeeStopMapping esm
            JOIN 
                tbl_EmployeeProfileMaster ep ON esm.EmployeeID = ep.Employee_id
            JOIN 
                tbl_Department d ON ep.Department_id = d.Department_id
            JOIN 
                tbl_Designation des ON ep.Designation_id = des.Designation_id
            JOIN 
                tblRouteStopMaster rsm ON esm.StopID = rsm.StopID
            LEFT JOIN 
                tblRouteSingleFeesPayment rsf ON rsf.StopID = rsm.StopID
            LEFT JOIN 
                tblRouteTermFeesPayment rtf ON rtf.StopID = rsm.StopID
            LEFT JOIN 
                tblRouteMonthlyFeesPayment rmf ON rmf.StopID = rsm.StopID
            WHERE 
                rsm.RoutePlanID = @RoutePlanID";

            // Add the search condition if Search is provided
            if (!string.IsNullOrEmpty(Search))
            {
                sql += " AND CONCAT(ep.First_Name, ' ', ep.Last_Name) LIKE @Search";
            }

            // Execute the query and return the result
            return await _dbConnection.QueryAsync<EmployeeDetails>(sql, new { RoutePlanID = routePlanID, Search = "%" + Search + "%" });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        public async Task<ServiceResponse<GetStudentTransportReportResponse>> GetStudentTransportReport(GetReportsRequest request)
        {
            // Fetch vehicle details based on RoutePlanID and InstituteID
            var vehicleDetails = await GetVehicleDetails(request.RoutePlanID.Value, request.InstituteID.Value);

            if (vehicleDetails == null)
            {
                return new ServiceResponse<GetStudentTransportReportResponse>(false, "No vehicle details found", null, StatusCodes.Status204NoContent);
            }

            // Fetch total student count
            int totalCount = await GetTotalStudentCount(request.RoutePlanID.Value, vehicleDetails.VehicleID);

            // Fetch students for the route
            var students = await GetStudentsForRoute(request.RoutePlanID.Value, request.Search);

            var reportResponse = new GetStudentTransportReportResponse
            {
                VehicleID = vehicleDetails.VehicleID,
                VehicleNo = vehicleDetails.VehicleNo,
                VehicleType = vehicleDetails.VehicleType,
                CoordinatorName = vehicleDetails.CoordinatorName,
                DriverName = vehicleDetails.DriverName,
                DriverNumber = vehicleDetails.DriverNumber,
                TotalCount = totalCount,
                Students = students.ToList()
            };

            return new ServiceResponse<GetStudentTransportReportResponse>(true, "Record Found", reportResponse, StatusCodes.Status200OK, totalCount);
        }



        public async Task<VehicleDetails> GetVehicleDetails(int routePlanID, int instituteID)
        {
            string sql = @"
                SELECT 
                    vm.VehicleID, 
                    vm.VehicleNumber AS VehicleNo, 
                    vt.Vehicle_type_name AS VehicleType, 
                    epDriver.First_Name + ' ' + epDriver.Last_Name AS DriverName, 
                    epDriver.mobile_number AS DriverNumber,
                    epCoordinator.First_Name + ' ' + epCoordinator.Last_Name AS CoordinatorName
                FROM 
                    tblVehicleMaster vm
                JOIN 
                    tbl_Vehicle_Type vt ON vm.VehicleTypeID = vt.Vehicle_type_id
                LEFT JOIN 
                    tblAssignRoute ar ON ar.VehicleID = vm.VehicleID
                LEFT JOIN 
                    tbl_EmployeeProfileMaster epDriver ON ar.DriverID = epDriver.Employee_id
                LEFT JOIN 
                    tbl_EmployeeProfileMaster epCoordinator ON ar.TransportStaffID = epCoordinator.Employee_id
                WHERE 
                    ar.RoutePlanID = @RoutePlanID
                    AND vm.InstituteID = @InstituteID";

            return await _dbConnection.QueryFirstOrDefaultAsync<VehicleDetails>(sql, new { RoutePlanID = routePlanID, InstituteID = instituteID });
        }

        public async Task<int> GetTotalStudentCount(int routePlanID, int vehicleID)
        {
            string sql = @"
                SELECT 
                        COUNT(DISTINCT ssm.StudentID) AS TotalCount
                    FROM 
                        tblStudentStopMapping ssm
                    JOIN 
                        tblRouteStopMaster rsm ON ssm.StopID = rsm.StopID
                    JOIN 
                        tblAssignRoute ar ON rsm.RoutePlanID = ar.RoutePlanID
                    WHERE 
                    ar.VehicleID = @VehicleID
                    AND ar.RoutePlanID = @RoutePlanID";

            return await _dbConnection.ExecuteScalarAsync<int>(sql, new { RoutePlanID = routePlanID, VehicleID = vehicleID });
        }




        // Method to fetch all students assigned to the route with their stop details
        //public async Task<IEnumerable<StudentDetails>> GetStudentsForRoute(int routePlanID, string Search)
        //{
        //    string sql = @"
        //        SELECT 
        //            ssm.StudentID,
        //            CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
        //            sm.Admission_Number AS AdmissionNumber,
        //            CONCAT(c.class_name, ' - ', sec.section_name) AS ClassSection,
        //            sm.Roll_Number AS RollNumber,
        //            '-' AS FatherName,
        //            '-' AS MobileNumber,
        //            rsm.StopName AS StopName
        //        FROM 
        //            tblStudentStopMapping ssm
        //        JOIN 
        //            tbl_StudentMaster sm ON ssm.StudentID = sm.student_id
        //        JOIN 
        //            tbl_Class c ON sm.class_id = c.class_id
        //        JOIN 
        //            tbl_Section sec ON sm.section_id = sec.section_id
        //        JOIN 
        //            tblRouteStopMaster rsm ON ssm.StopID = rsm.StopID
        //        WHERE 
        //            rsm.RoutePlanID = @RoutePlanID
        //        ORDER BY 
        //            sm.Admission_Number";

        //    return await _dbConnection.QueryAsync<StudentDetails>(sql, new { RoutePlanID = routePlanID });
        //}

        public async Task<IEnumerable<StudentDetails>> GetStudentsForRoute(int routePlanID, string Search)
        {
            // Base SQL query to fetch students with Transport Fee calculation
            string sql = @"
            SELECT 
                ssm.StudentID,
                CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
                sm.Admission_Number AS AdmissionNumber,
                CONCAT(c.class_name, ' - ', sec.section_name) AS ClassSection,
                sm.Roll_Number AS RollNumber,
                '-' AS FatherName,
                '-' AS MobileNumber,
                rsm.StopName AS StopName,
                -- Adding Transport Fee calculation
                CASE 
                    WHEN rsf.FeesAmount IS NOT NULL THEN rsf.FeesAmount 
                    WHEN rtf.FeesAmount IS NOT NULL THEN rtf.FeesAmount
                    WHEN rmf.FeesAmount IS NOT NULL THEN rmf.FeesAmount 
                    ELSE 0 
                END AS TransportFee
            FROM 
                tblStudentStopMapping ssm
            JOIN 
                tbl_StudentMaster sm ON ssm.StudentID = sm.student_id
            JOIN 
                tbl_Class c ON sm.class_id = c.class_id
            JOIN 
                tbl_Section sec ON sm.section_id = sec.section_id
            JOIN 
                tblRouteStopMaster rsm ON ssm.StopID = rsm.StopID
            LEFT JOIN 
                tblRouteSingleFeesPayment rsf ON rsf.StopID = rsm.StopID
            LEFT JOIN 
                tblRouteTermFeesPayment rtf ON rtf.StopID = rsm.StopID
            LEFT JOIN 
                tblRouteMonthlyFeesPayment rmf ON rmf.StopID = rsm.StopID
            WHERE 
                rsm.RoutePlanID = @RoutePlanID";

            // Add search condition if the Search parameter is provided
            if (!string.IsNullOrEmpty(Search))
            {
                sql += " AND CONCAT(sm.First_Name, ' ', sm.Last_Name) LIKE @Search";
            }

            // Execute the query
            return await _dbConnection.QueryAsync<StudentDetails>(sql, new { RoutePlanID = routePlanID, Search = "%" + Search + "%" });
        }



        /// <summary>
        /// ////
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        public async Task<ServiceResponse<IEnumerable<GetTransportAttendanceResponse>>> GetTransportAttendanceReport(TransportAttendanceReportRequest request)
        {
            // Convert the StartDate and EndDate strings to DateTime objects
            var startDate = request.GetStartDateAsDateTime();
            var endDate = request.GetEndDateAsDateTime();

            // Generate all dates in the given range
            var allDates = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                                      .Select(offset => startDate.AddDays(offset))
                                      .ToList();

            // Query to get student information
            var students = await _dbConnection.QueryAsync<GetTransportAttendanceResponse>(@"
SELECT 
    sm.student_id AS StudentID,
    CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
    sm.Admission_Number AS AdmissionNo,
    CONCAT(c.class_name, ' - ', s.section_name) AS ClassSection
FROM 
    tbl_StudentMaster sm
JOIN 
    tbl_Class c ON sm.class_id = c.class_id
JOIN 
    tbl_Section s ON sm.section_id = s.section_id
JOIN 
    tblStudentStopMapping ssm ON sm.student_id = ssm.StudentID
JOIN 
    tblRouteStopMaster rsm ON ssm.StopID = rsm.StopID
WHERE 
    rsm.RoutePlanID = @RoutePlanID", new { request.RoutePlanID });

            foreach (var student in students)
            {
                // Get attendance data for each student
                var attendance = await _dbConnection.QueryAsync(@"
    SELECT 
        AttendanceDate,
        TransportAttendanceTypeID,
        AttendanceStatus
    FROM 
        tblTransportAttendance
    WHERE 
        RoutePlanID = @RoutePlanID
        AND StudentID = @StudentID
        AND AttendanceDate BETWEEN @StartDate AND @EndDate",
                    new
                    {
                        RoutePlanID = request.RoutePlanID,
                        StudentID = student.StudentID,
                        StartDate = startDate,
                        EndDate = endDate
                    });

                // Group attendance by date
                var attendanceByDate = attendance
                    .GroupBy(a => a.AttendanceDate)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Create the full day attendance list including days without attendance
                var dayAttendanceList = allDates.Select(date => new DayAttendance
                {
                    AttendanceDate = date.ToString("MMM dd, ddd"),
                    Pickup = attendanceByDate.ContainsKey(date) ? FormatAttendance(attendanceByDate[date], 1) : "Pickup - No Data",
                    Drop = attendanceByDate.ContainsKey(date) ? FormatAttendance(attendanceByDate[date], 2) : "Drop - No Data"
                }).ToList();

                student.Days = dayAttendanceList;
            }

            return new ServiceResponse<IEnumerable<GetTransportAttendanceResponse>>(true, "Records Found", students, StatusCodes.Status200OK);
        }

        private string FormatAttendance(IEnumerable<dynamic> attendanceGroup, int attendanceTypeID)
        {
            var attendanceRecord = attendanceGroup.FirstOrDefault(a => a.TransportAttendanceTypeID == attendanceTypeID);

            if (attendanceRecord == null)
            {
                return attendanceTypeID == 1 ? "Pickup - No Data" : "Drop - No Data";
            }

            if (attendanceRecord.AttendanceStatus == "P")
            {
                return attendanceTypeID == 1 ? "Pickup - Present" : "Drop - Present";
            }
            else if (attendanceRecord.AttendanceStatus == "A")
            {
                return attendanceTypeID == 1 ? "Pickup - Absent" : "Drop - Absent";
            }

            return attendanceTypeID == 1 ? "Pickup - Unknown" : "Drop - Unknown";
        }



        public async Task<ServiceResponse<IEnumerable<ReportResponse>>> GetDeAllocationReport(GetReportsRequest request)
        {
            string sql = @"SELECT ReportType, ReportData FROM tbl_Reports WHERE ReportType = 'DeAllocation' ORDER BY ReportId OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var reports = await _dbConnection.QueryAsync<ReportResponse>(sql, new { Offset = (request.PageNumber - 1) * request.PageSize, PageSize = request.PageSize });

            if (reports.Any())
            {
                return new ServiceResponse<IEnumerable<ReportResponse>>(true, "Records Found", reports, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<IEnumerable<ReportResponse>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }


        public async Task<ServiceResponse<IEnumerable<GetStudentsReportResponse>>> GetStudentsReport(StudentsReportRequest request)
        {
            string sql = @"
        -- Allocated students
        SELECT 
            sm.student_id AS StudentID,
            CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
            sm.Admission_Number AS AdmissionNo,
            sm.Roll_Number AS RollNo,
            '' AS FatherName,
            '' AS MobileNo,
            'Allocated' AS Status
        FROM 
            tbl_StudentMaster sm
        JOIN 
            tblStudentStopMapping ssm ON sm.student_id = ssm.StudentID
        WHERE 
            sm.class_id = @ClassID
            AND sm.section_id = @SectionID
            AND sm.Institute_id = @InstituteID

        UNION

        -- Non-Allocated students
        SELECT 
            sm.student_id AS StudentID,
            CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
            sm.Admission_Number AS AdmissionNo,
            sm.Roll_Number AS RollNo,
            '' AS FatherName,
            '' AS MobileNo,
            'Non-Allocated' AS Status
        FROM 
            tbl_StudentMaster sm
        LEFT JOIN 
            tblStudentStopMapping ssm ON sm.student_id = ssm.StudentID
        WHERE 
            sm.class_id = @ClassID
            AND sm.section_id = @SectionID
            AND sm.Institute_ID = @InstituteID
            AND ssm.StudentID IS NULL
    ";

            // Fetch all students (both Allocated and Non-Allocated)
            var students = await _dbConnection.QueryAsync<GetStudentsReportResponse>(sql, new
            {
                request.ClassID,
                request.SectionID,
                request.InstituteID
            });

            // Filter based on the Status parameter
            List<GetStudentsReportResponse> filteredStudents = students.ToList();

            if (request.Status.Equals("Allocated", StringComparison.OrdinalIgnoreCase))
            {
                filteredStudents = filteredStudents.Where(s => s.Status == "Allocated").ToList();
            }
            else if (request.Status.Equals("Non-Allocated", StringComparison.OrdinalIgnoreCase))
            {
                filteredStudents = filteredStudents.Where(s => s.Status == "Non-Allocated").ToList();
            }
            // If status is "All" or if none of the conditions match, return all records

            if (filteredStudents != null && filteredStudents.Any())
            {
                return new ServiceResponse<IEnumerable<GetStudentsReportResponse>>(true, "Records Found", filteredStudents, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<IEnumerable<GetStudentsReportResponse>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }



        //Export Excel

        public async Task<byte[]> GetEmployeeTransportationReportExportExcel(GetReportsRequest request)
        {
            var reportData = await GetEmployeeTransportationReport(request); // Fetch data using existing method

            if (reportData.Data != null)
            {
                // Ensure the data is wrapped in a collection
                var reportList = new List<GetEmployeeTransportationReportResponse> { reportData.Data };
                return GenerateExcelForEmployeeTransportation(reportList);
            }

            return null;
        }


        private byte[] GenerateExcelForEmployeeTransportation(IEnumerable<GetEmployeeTransportationReportResponse> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("EmployeeTransportation");

                // Add headers
                worksheet.Cells[1, 1].Value = "Vehicle ID";
                worksheet.Cells[1, 2].Value = "Vehicle No";
                worksheet.Cells[1, 3].Value = "Vehicle Type";
                worksheet.Cells[1, 4].Value = "Coordinator Name";
                worksheet.Cells[1, 5].Value = "Driver Name";
                worksheet.Cells[1, 6].Value = "Driver Number";
                worksheet.Cells[1, 7].Value = "Total Count";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.VehicleID;
                    worksheet.Cells[row, 2].Value = item.VehicleNo;
                    worksheet.Cells[row, 3].Value = item.VehicleType;
                    worksheet.Cells[row, 4].Value = item.CoordinatorName;
                    worksheet.Cells[row, 5].Value = item.DriverName;
                    worksheet.Cells[row, 6].Value = item.DriverNumber;
                    worksheet.Cells[row, 7].Value = item.TotalCount;

                    int employeeRow = row + 1;
                    foreach (var emp in item.Employees)
                    {
                        worksheet.Cells[employeeRow, 2].Value = emp.EmployeeName;
                        worksheet.Cells[employeeRow, 3].Value = emp.Department;
                        worksheet.Cells[employeeRow, 4].Value = emp.Designation;
                        worksheet.Cells[employeeRow, 5].Value = emp.MobileNumber;
                        worksheet.Cells[employeeRow, 6].Value = emp.StopName;
                        employeeRow++;
                    }

                    row = employeeRow + 1;
                }

                return package.GetAsByteArray();
            }
        }

        public async Task<byte[]> GetStudentTransportReportExportExcel(GetReportsRequest request)
        {
            var reportData = await GetStudentTransportReport(request); // Fetch data using existing method

            if (reportData.Data != null)
            {
                // Ensure the data is wrapped in a collection
                var reportList = new List<GetStudentTransportReportResponse> { reportData.Data };
                return GenerateExcelForStudentTransport(reportList);
            }

            return null;
        }

        private byte[] GenerateExcelForStudentTransport(IEnumerable<GetStudentTransportReportResponse> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("StudentTransport");

                // Add headers
                worksheet.Cells[1, 1].Value = "Vehicle ID";
                worksheet.Cells[1, 2].Value = "Vehicle No";
                worksheet.Cells[1, 3].Value = "Vehicle Type";
                worksheet.Cells[1, 4].Value = "Coordinator Name";
                worksheet.Cells[1, 5].Value = "Driver Name";
                worksheet.Cells[1, 6].Value = "Driver Number";
                worksheet.Cells[1, 7].Value = "Total Count";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.VehicleID;
                    worksheet.Cells[row, 2].Value = item.VehicleNo;
                    worksheet.Cells[row, 3].Value = item.VehicleType;
                    worksheet.Cells[row, 4].Value = item.CoordinatorName;
                    worksheet.Cells[row, 5].Value = item.DriverName;
                    worksheet.Cells[row, 6].Value = item.DriverNumber;
                    worksheet.Cells[row, 7].Value = item.TotalCount;

                    int studentRow = row + 1;
                    foreach (var student in item.Students)
                    {
                        worksheet.Cells[studentRow, 2].Value = student.StudentName;
                        worksheet.Cells[studentRow, 3].Value = student.AdmissionNumber;
                        worksheet.Cells[studentRow, 4].Value = student.ClassSection;
                        worksheet.Cells[studentRow, 5].Value = student.RollNumber;
                        worksheet.Cells[studentRow, 6].Value = student.StopName;
                        studentRow++;
                    }

                    row = studentRow + 1;
                }

                return package.GetAsByteArray();
            }
        }



        public async Task<byte[]> GetTransportAttendanceReportExportExcel(TransportAttendanceReportRequest request)
        {
            // Convert the start and end date strings to DateTime objects
            var startDate = request.GetStartDateAsDateTime();
            var endDate = request.GetEndDateAsDateTime();

            // Generate all dates in the given range
            var allDates = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                                      .Select(offset => startDate.AddDays(offset))
                                      .ToList();

            // Query to get student information
            var students = await _dbConnection.QueryAsync<GetTransportAttendanceResponse>(@"
        SELECT 
            sm.student_id AS StudentID,
            CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
            sm.Admission_Number AS AdmissionNo,
            CONCAT(c.class_name, ' - ', s.section_name) AS ClassSection,
            '' AS MobileNo
        FROM 
            tbl_StudentMaster sm
        JOIN 
            tbl_Class c ON sm.class_id = c.class_id
        JOIN 
            tbl_Section s ON sm.section_id = s.section_id
        JOIN 
            tblStudentStopMapping ssm ON sm.student_id = ssm.StudentID
        JOIN 
            tblRouteStopMaster rsm ON ssm.StopID = rsm.StopID
        WHERE 
            rsm.RoutePlanID = @RoutePlanID
            AND sm.Institute_id = @InstituteID", new { request.RoutePlanID, request.InstituteID });

            // Prepare data for Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Transport Attendance Report");

                // Add headers
                worksheet.Cells[1, 1].Value = "S. No";
                worksheet.Cells[1, 2].Value = "Admission No";
                worksheet.Cells[1, 3].Value = "Student Name";
                worksheet.Cells[1, 4].Value = "Class & Section";
                worksheet.Cells[1, 5].Value = "Mobile No";

                // Dynamic headers for each date (Pickup and Drop)
                int col = 6;
                foreach (var date in allDates)
                {
                    worksheet.Cells[1, col].Value = date.ToString("MMM dd, ddd");
                    worksheet.Cells[1, col, 1, col + 1].Merge = true;  // Merge Pickup and Drop columns for the date
                    worksheet.Cells[2, col].Value = "Pickup";
                    worksheet.Cells[2, col + 1].Value = "Drop";
                    col += 2;
                }

                // Get attendance data for each student
                int row = 3;
                int serialNo = 1;
                foreach (var student in students)
                {
                    worksheet.Cells[row, 1].Value = serialNo++;  // S. No
                    worksheet.Cells[row, 2].Value = student.AdmissionNo;
                    worksheet.Cells[row, 3].Value = student.StudentName;
                    worksheet.Cells[row, 4].Value = student.ClassSection;
                    worksheet.Cells[row, 5].Value = student.MobileNo;

                    // Get attendance data for each student
                    var attendanceData = await _dbConnection.QueryAsync(@"
                SELECT 
                    AttendanceDate,
                    TransportAttendanceTypeID,
                    AttendanceStatus
                FROM 
                    tblTransportAttendance
                WHERE 
                    RoutePlanID = @RoutePlanID
                    AND StudentID = @StudentID
                    AND AttendanceDate BETWEEN @StartDate AND @EndDate",
                            new
                            {
                                RoutePlanID = request.RoutePlanID,
                                StudentID = student.StudentID,
                                StartDate = startDate,
                                EndDate = endDate
                            });

                    // Dictionary to hold attendance for quick lookup
                    var attendanceDict = attendanceData
                        .GroupBy(a => a.AttendanceDate)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    // Populate attendance for each date
                    col = 6;
                    foreach (var date in allDates)
                    {
                        // Default to "No Data" if no attendance record exists for the date
                        var pickup = "-";
                        var drop = "-";

                        if (attendanceDict.ContainsKey(date))
                        {
                            var attendanceRecords = attendanceDict[date];

                            // Check for Pickup record
                            var pickupRecord = attendanceRecords.FirstOrDefault(a => a.TransportAttendanceTypeID == 1);
                            if (pickupRecord != null)
                            {
                                pickup = pickupRecord.AttendanceStatus == "P" ? "Present" : "Absent";
                            }

                            // Check for Drop record
                            var dropRecord = attendanceRecords.FirstOrDefault(a => a.TransportAttendanceTypeID == 2);
                            if (dropRecord != null)
                            {
                                drop = dropRecord.AttendanceStatus == "P" ? "Present" : "Absent";
                            }
                        }

                        // Set the pickup and drop values in the worksheet
                        worksheet.Cells[row, col].Value = pickup;
                        worksheet.Cells[row, col + 1].Value = drop;
                        col += 2;
                    }

                    row++;
                }

                // Auto-fit columns for better visibility
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Return the Excel file as a byte array
                return package.GetAsByteArray();
            }
        }



        public async Task<byte[]> GetStudentsReportExportExcel(StudentsReportRequest request)
        {
            var reportData = await GetStudentsReport(request); // Fetch data using existing method

            if (reportData.Data != null)
            {
                return GenerateExcelForStudentsReport(reportData.Data);
            }

            return null;
        }

        private byte[] GenerateExcelForStudentsReport(IEnumerable<GetStudentsReportResponse> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("StudentsReport");

                // Add headers
                worksheet.Cells[1, 1].Value = "Student ID";
                worksheet.Cells[1, 2].Value = "Student Name";
                worksheet.Cells[1, 3].Value = "Admission No";
                worksheet.Cells[1, 4].Value = "Roll No";
                worksheet.Cells[1, 5].Value = "Father Name";
                worksheet.Cells[1, 6].Value = "Mobile No";
                worksheet.Cells[1, 7].Value = "Status";

                int row = 2;
                foreach (var student in data)
                {
                    worksheet.Cells[row, 1].Value = student.StudentID;
                    worksheet.Cells[row, 2].Value = student.StudentName;
                    worksheet.Cells[row, 3].Value = student.AdmissionNo;
                    worksheet.Cells[row, 4].Value = student.RollNo;
                    worksheet.Cells[row, 5].Value = student.FatherName;
                    worksheet.Cells[row, 6].Value = student.MobileNo;
                    worksheet.Cells[row, 7].Value = student.Status;
                    row++;
                }

                return package.GetAsByteArray();
            }
        }


        // Employee Transportation Report CSV Export
        public async Task<byte[]> GetEmployeeTransportationReportExportCSV(GetReportsRequest request)
        {
            var reportData = await GetEmployeeTransportationReport(request); // Fetch data using existing method

            if (reportData.Data != null)
            {
                var reportList = new List<GetEmployeeTransportationReportResponse> { reportData.Data };
                return GenerateCSVForEmployeeTransportation(reportList);
            }

            return null;
        }

        private byte[] GenerateCSVForEmployeeTransportation(IEnumerable<GetEmployeeTransportationReportResponse> data)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Vehicle ID, Vehicle No, Vehicle Type, Coordinator Name, Driver Name, Driver Number, Total Count");

            foreach (var item in data)
            {
                csv.AppendLine($"{item.VehicleID}, {item.VehicleNo}, {item.VehicleType}, {item.CoordinatorName}, {item.DriverName}, {item.DriverNumber}, {item.TotalCount}");

                foreach (var emp in item.Employees)
                {
                    csv.AppendLine($", , , , Employee Name: {emp.EmployeeName}, Department: {emp.Department}, Designation: {emp.Designation}, Mobile: {emp.MobileNumber}, Stop: {emp.StopName}");
                }
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        // Student Transport Report CSV Export
        public async Task<byte[]> GetStudentTransportReportExportCSV(GetReportsRequest request)
        {
            var reportData = await GetStudentTransportReport(request); // Fetch data using existing method

            if (reportData.Data != null)
            {
                var reportList = new List<GetStudentTransportReportResponse> { reportData.Data };
                return GenerateCSVForStudentTransport(reportList);
            }

            return null;
        }

        private byte[] GenerateCSVForStudentTransport(IEnumerable<GetStudentTransportReportResponse> data)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Vehicle ID, Vehicle No, Vehicle Type, Coordinator Name, Driver Name, Driver Number, Total Count");

            foreach (var item in data)
            {
                csv.AppendLine($"{item.VehicleID}, {item.VehicleNo}, {item.VehicleType}, {item.CoordinatorName}, {item.DriverName}, {item.DriverNumber}, {item.TotalCount}");

                foreach (var student in item.Students)
                {
                    csv.AppendLine($", , , , Student Name: {student.StudentName}, Admission: {student.AdmissionNumber}, Class-Section: {student.ClassSection}, Roll: {student.RollNumber}, Stop: {student.StopName}");
                }
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        // Transport Attendance Report CSV Export
        public async Task<byte[]> GetTransportAttendanceReportExportCSV(TransportAttendanceReportRequest request)
        {
            var startDate = request.GetStartDateAsDateTime();
            var endDate = request.GetEndDateAsDateTime();
            var allDates = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days).Select(offset => startDate.AddDays(offset)).ToList();

            var students = await _dbConnection.QueryAsync<GetTransportAttendanceResponse>(@"
        SELECT 
            sm.student_id AS StudentID,
            CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
            sm.Admission_Number AS AdmissionNo,
            CONCAT(c.class_name, ' - ', s.section_name) AS ClassSection,
            '' AS MobileNo
        FROM 
            tbl_StudentMaster sm
        JOIN 
            tbl_Class c ON sm.class_id = c.class_id
        JOIN 
            tbl_Section s ON sm.section_id = s.section_id
        JOIN 
            tblStudentStopMapping ssm ON sm.student_id = ssm.StudentID
        JOIN 
            tblRouteStopMaster rsm ON ssm.StopID = rsm.StopID
        WHERE 
            rsm.RoutePlanID = @RoutePlanID
            AND sm.Institute_id = @InstituteID", new { request.RoutePlanID, request.InstituteID });

            // Pass routePlanID when generating the CSV
            return GenerateCSVForTransportAttendance(allDates, students, request.RoutePlanID);
        }


        private byte[] GenerateCSVForTransportAttendance(List<DateTime> allDates, IEnumerable<GetTransportAttendanceResponse> students, int routePlanID)
        {
            var csv = new StringBuilder();
            csv.Append("S. No, Admission No, Student Name, Class & Section, Mobile No");

            foreach (var date in allDates)
            {
                csv.Append($", {date.ToString("MMM dd, ddd")} Pickup, {date.ToString("MMM dd, ddd")} Drop");
            }
            csv.AppendLine();

            int serialNo = 1;
            foreach (var student in students)
            {
                csv.Append($"{serialNo++}, {student.AdmissionNo}, {student.StudentName}, {student.ClassSection}, {student.MobileNo}");

                foreach (var date in allDates)
                {
                    var attendanceData = GetAttendanceForStudentOnDate(student.StudentID, date, routePlanID);

                    var pickup = attendanceData.PickupStatus ?? "-";
                    var drop = attendanceData.DropStatus ?? "-";

                    csv.Append($", {pickup}, {drop}");
                }

                csv.AppendLine();
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }


        // Define a class to hold the attendance data
        public class AttendanceForStudent
        {
            public string PickupStatus { get; set; }
            public string DropStatus { get; set; }
        }

        // Define the method to fetch attendance for a student on a specific date
        private AttendanceForStudent GetAttendanceForStudentOnDate(int studentId, DateTime date, int routePlanID)
        {
            // This query fetches attendance for both Pickup (TransportAttendanceTypeID = 1) 
            // and Drop (TransportAttendanceTypeID = 2) for the given student on the specified date.
            var sql = @"
        SELECT 
            TransportAttendanceTypeID, 
            AttendanceStatus 
        FROM 
            tblTransportAttendance 
        WHERE 
            StudentID = @StudentID 
            AND AttendanceDate = @Date 
            AND RoutePlanID = @RoutePlanID";

            // Execute the query using Dapper and map the result
            var attendanceRecords = _dbConnection.Query(sql, new
            {
                StudentID = studentId,
                Date = date,
                RoutePlanID = routePlanID
            });

            // Initialize attendance status
            var attendance = new AttendanceForStudent
            {
                PickupStatus = "No Data",
                DropStatus = "No Data"
            };

            // Check the fetched attendance records and update the status accordingly
            foreach (var record in attendanceRecords)
            {
                if (record.TransportAttendanceTypeID == 1)  // Pickup
                {
                    attendance.PickupStatus = record.AttendanceStatus == "P" ? "Present" : "Absent";
                }
                else if (record.TransportAttendanceTypeID == 2)  // Drop
                {
                    attendance.DropStatus = record.AttendanceStatus == "P" ? "Present" : "Absent";
                }
            }

            return attendance;
        }




        // Students Report CSV Export
        public async Task<byte[]> GetStudentsReportExportCSV(StudentsReportRequest request)
        {
            var reportData = await GetStudentsReport(request); // Fetch data using existing method

            if (reportData.Data != null)
            {
                return GenerateCSVForStudentsReport(reportData.Data);
            }

            return null;
        }

        private byte[] GenerateCSVForStudentsReport(IEnumerable<GetStudentsReportResponse> data)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Student ID, Student Name, Admission No, Roll No, Father Name, Mobile No, Status");

            foreach (var student in data)
            {
                csv.AppendLine($"{student.StudentID}, {student.StudentName}, {student.AdmissionNo}, {student.RollNo}, {student.FatherName}, {student.MobileNo}, {student.Status}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

    }
}
