using Dapper;
using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
using Employee_API.Repository.Interfaces;
using OfficeOpenXml;
using System.Data;
using System.Text;

namespace Employee_API.Repository.Implementations
{
    public class EmployeeLoginsRepository : IEmployeeLoginsRepository
    {
        private readonly IDbConnection _connection;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public EmployeeLoginsRepository(IDbConnection connection, IWebHostEnvironment hostingEnvironment)
        {
            _connection = connection;
            _hostingEnvironment = hostingEnvironment;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }
        public async Task<ServiceResponse<byte[]>> DownloadExcelSheet(ExcelDownloadRequest request, string format)
        {
            try
            {
                // SQL query to fetch employee details along with login information
                string sql = @"
        SELECT emp.Employee_id, emp.First_Name, emp.Middle_Name, emp.Last_Name, emp.Gender_id, 
               emp.Department_id, emp.Designation_id, emp.mobile_number, emp.Date_of_Birth, emp.EmailID,
               dep.DepartmentName, des.DesignationName, gen.Gender_Type, 
               login.UserName AS LoginId, login.Password, login.UserActivity AS LastActivity
        FROM tbl_EmployeeProfileMaster emp
        LEFT JOIN tbl_Department dep ON emp.Department_id = dep.Department_id
        LEFT JOIN tbl_Designation des ON emp.Designation_id = des.Designation_id
        LEFT JOIN tbl_Gender gen ON emp.Gender_id = gen.Gender_id
        LEFT JOIN tblLoginInformationMaster login ON emp.Employee_id = login.UserId
        WHERE emp.Institute_id = @InstituteId AND emp.Status = 1
        AND (@DepartmentId = 0 OR emp.Department_id = @DepartmentId)
        AND (@DesignationId = 0 OR emp.Designation_id = @DesignationId)";

                // Execute the query, using optional filters
                var employees = await _connection.QueryAsync<dynamic>(sql, new { request.InstituteId, request.DepartmetnId, request.DesignationId });

                // Check if employees exist
                if (!employees.Any())
                {
                    return new ServiceResponse<byte[]>(false, "No records found.", null, 404);
                }

                // Check the format and act accordingly
                if (format.ToLower() == "excel")
                {
                    // Generate Excel (.xlsx) file
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Employee Details");

                        // Add headers
                        worksheet.Cells[1, 1].Value = "Sr. No.";
                        worksheet.Cells[1, 2].Value = "Employee ID";
                        worksheet.Cells[1, 3].Value = "Employee Name";
                        worksheet.Cells[1, 4].Value = "Department";
                        worksheet.Cells[1, 5].Value = "Designation";
                        worksheet.Cells[1, 6].Value = "Gender";
                        worksheet.Cells[1, 7].Value = "Mobile";
                        worksheet.Cells[1, 8].Value = "Date of Birth";
                        worksheet.Cells[1, 9].Value = "Email";
                        worksheet.Cells[1, 10].Value = "Login ID";
                        worksheet.Cells[1, 11].Value = "Password";
                        worksheet.Cells[1, 12].Value = "Last Activity";

                        // Fill employee data
                        int row = 2;
                        int serialNumber = 1;
                        foreach (var employee in employees)
                        {
                            string employeeName = $"{employee.First_Name} {employee.Middle_Name} {employee.Last_Name}".Trim();
                            string lastActivityFormatted = employee.LastActivity != null && employee.LastActivity is DateTime
                                ? ((DateTime)employee.LastActivity).ToString("dd-MM-yyyy hh:mm tt")
                                : string.Empty;

                            worksheet.Cells[row, 1].Value = serialNumber++;
                            worksheet.Cells[row, 2].Value = employee.Employee_id;
                            worksheet.Cells[row, 3].Value = employeeName;
                            worksheet.Cells[row, 4].Value = employee.DepartmentName;
                            worksheet.Cells[row, 5].Value = employee.DesignationName;
                            worksheet.Cells[row, 6].Value = employee.Gender_Type;
                            worksheet.Cells[row, 7].Value = employee.mobile_number;
                            worksheet.Cells[row, 8].Value = employee.Date_of_Birth;
                            worksheet.Cells[row, 9].Value = employee.EmailID;
                            worksheet.Cells[row, 10].Value = employee.LoginId;
                            worksheet.Cells[row, 11].Value = employee.Password;
                            worksheet.Cells[row, 12].Value = lastActivityFormatted;
                            row++;
                        }

                        // Auto-fit columns for better visibility
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Convert Excel sheet to byte array
                        var excelData = package.GetAsByteArray();

                        return new ServiceResponse<byte[]>(true, "Excel file generated successfully", excelData, 200);
                    }
                }
                else if (format.ToLower() == "csv")
                {
                    // Generate CSV format
                    var csv = new StringBuilder();
                    csv.AppendLine("Sr. No.,Employee ID,Employee Name,Department,Designation,Gender,Mobile,Date of Birth,Email,Login ID,Password,Last Activity");

                    int serialNumber = 1;
                    foreach (var employee in employees)
                    {
                        string employeeName = $"{employee.First_Name} {employee.Middle_Name} {employee.Last_Name}".Trim();
                        string lastActivityFormatted = employee.LastActivity != null && employee.LastActivity is DateTime
                            ? ((DateTime)employee.LastActivity).ToString("dd-MM-yyyy hh:mm tt")
                            : string.Empty;

                        csv.AppendLine($"{serialNumber++},{employee.Employee_id},{employeeName},{employee.DepartmentName},{employee.DesignationName},{employee.Gender_Type},{employee.mobile_number},{employee.Date_of_Birth},{employee.EmailID},{employee.LoginId},{employee.Password},{lastActivityFormatted}");
                    }

                    // Convert CSV content to byte array
                    var csvData = Encoding.UTF8.GetBytes(csv.ToString());

                    return new ServiceResponse<byte[]>(true, "CSV file generated successfully", csvData, 200);
                }
                else
                {
                    // Invalid format
                    return new ServiceResponse<byte[]>(false, "Invalid file format requested. Supported formats: xlsx, csv", null, 400);
                }
            }
            catch (Exception ex)
            {
                // Handle any errors
                return new ServiceResponse<byte[]>(false, $"Error generating file: {ex.Message}", null, 500);
            }
        }

        //public async Task<ServiceResponse<byte[]>> DownloadExcelSheet(DownloadExcelRequest request)
        //{
        //    try
        //    {
        //        // SQL query to fetch employee details along with login information
        //        string sql = @"
        //    SELECT emp.Employee_id, emp.First_Name, emp.Middle_Name, emp.Last_Name, emp.Gender_id, 
        //           emp.Department_id, emp.Designation_id, emp.mobile_number, emp.Date_of_Birth, emp.EmailID,
        //           dep.DepartmentName, des.DesignationName, gen.Gender_Type, 
        //           login.UserName AS LoginId, login.Password, login.UserActivity AS LastActivity
        //    FROM tbl_EmployeeProfileMaster emp
        //    LEFT JOIN tbl_Department dep ON emp.Department_id = dep.Department_id
        //    LEFT JOIN tbl_Designation des ON emp.Designation_id = des.Designation_id
        //    LEFT JOIN tbl_Gender gen ON emp.Gender_id = gen.Gender_id
        //    LEFT JOIN tblLoginInformationMaster login ON emp.Employee_id = login.UserId
        //    WHERE emp.Institute_id = @InstituteId AND emp.Status = 1
        //    AND (@DepartmentId = 0 OR emp.Department_id = @DepartmentId)
        //    AND (@DesignationId = 0 OR emp.Designation_id = @DesignationId)";

        //        // Execute the query, using optional filters
        //        var employees = await _connection.QueryAsync<dynamic>(sql, new { request.InstituteId, request.DepartmentId, request.DesignationId });

        //        // Initialize EPPlus package to create the Excel sheet
        //        using (var package = new ExcelPackage())
        //        {
        //            var worksheet = package.Workbook.Worksheets.Add("Employee Details");

        //            // Add headers, including the new ones
        //            worksheet.Cells[1, 1].Value = "Sr. No.";
        //            worksheet.Cells[1, 2].Value = "Employee ID";
        //            worksheet.Cells[1, 3].Value = "Employee Name";
        //            worksheet.Cells[1, 4].Value = "Department";
        //            worksheet.Cells[1, 5].Value = "Designation";
        //            worksheet.Cells[1, 6].Value = "Gender";
        //            worksheet.Cells[1, 7].Value = "Mobile";
        //            worksheet.Cells[1, 8].Value = "Date of Birth";
        //            worksheet.Cells[1, 9].Value = "Email";
        //            worksheet.Cells[1, 10].Value = "Login ID";
        //            worksheet.Cells[1, 11].Value = "Password";
        //            worksheet.Cells[1, 12].Value = "Last Activity";

        //            // Check if there are employees returned from the query
        //            if (employees.Any())
        //            {
        //                // Fill the worksheet with employee data and Sr. No.
        //                int row = 2;
        //                int serialNumber = 1;

        //                foreach (var employee in employees)
        //                {
        //                    // Combine first name, middle name, and last name into employee name
        //                    string employeeName = $"{employee.First_Name} {employee.Middle_Name} {employee.Last_Name}".Trim();

        //                    // Check if LastActivity is not null and is a DateTime
        //                    string lastActivityFormatted = employee.LastActivity != null && employee.LastActivity is DateTime
        //                        ? ((DateTime)employee.LastActivity).ToString("dd-MM-yyyy hh:mm tt")
        //                        : string.Empty;

        //                    worksheet.Cells[row, 1].Value = serialNumber; // Sr. No.
        //                    worksheet.Cells[row, 2].Value = employee.Employee_id;
        //                    worksheet.Cells[row, 3].Value = employeeName;
        //                    worksheet.Cells[row, 4].Value = employee.DepartmentName;
        //                    worksheet.Cells[row, 5].Value = employee.DesignationName;
        //                    worksheet.Cells[row, 6].Value = employee.Gender_Type;
        //                    worksheet.Cells[row, 7].Value = employee.mobile_number;
        //                    worksheet.Cells[row, 8].Value = employee.Date_of_Birth;
        //                    worksheet.Cells[row, 9].Value = employee.EmailID;
        //                    worksheet.Cells[row, 10].Value = employee.LoginId; // Login ID
        //                    worksheet.Cells[row, 11].Value = employee.Password; // Password
        //                    worksheet.Cells[row, 12].Value = lastActivityFormatted; // Last Activity

        //                    row++;
        //                    serialNumber++;
        //                }

        //                // Auto-fit the columns for better visibility
        //                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        //            }
        //            else
        //            {
        //                // If no data is found, return an empty Excel with only headers
        //                worksheet.Cells[1, 1].Value = "Sr. No.";
        //                worksheet.Cells[1, 2].Value = "Employee ID";
        //                worksheet.Cells[1, 3].Value = "Employee Name";
        //                worksheet.Cells[1, 4].Value = "Department";
        //                worksheet.Cells[1, 5].Value = "Designation";
        //                worksheet.Cells[1, 6].Value = "Gender";
        //                worksheet.Cells[1, 7].Value = "Mobile";
        //                worksheet.Cells[1, 8].Value = "Date of Birth";
        //                worksheet.Cells[1, 9].Value = "Email";
        //                worksheet.Cells[1, 10].Value = "Login ID";
        //                worksheet.Cells[1, 11].Value = "Password";
        //                worksheet.Cells[1, 12].Value = "Last Activity";
        //            }

        //            // Convert the worksheet into a byte array
        //            var excelData = package.GetAsByteArray();

        //            // Return the byte array as the service response
        //            return new ServiceResponse<byte[]>(true, "Excel file generated successfully", excelData, 200);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any errors during the process
        //        return new ServiceResponse<byte[]>(false, $"Error generating Excel file: {ex.Message}", null, 500);
        //    }
        //}
        public async Task<ServiceResponse<byte[]>> DownloadExcelSheetNonAppUsers(DownloadExcelRequest request, string format)
        {
            try
            {
                // SQL query to fetch non-app users (IsAppUser = false) based on InstituteId and optional filters for DepartmentId and DesignationId
                string sql = @"
        SELECT emp.Employee_id, emp.First_Name, emp.Middle_Name, emp.Last_Name, emp.Gender_id, 
               emp.Department_id, emp.Designation_id, emp.mobile_number, 
               dep.DepartmentName, des.DesignationName, gen.Gender_Type
        FROM tbl_EmployeeProfileMaster emp
        LEFT JOIN tbl_Department dep ON emp.Department_id = dep.Department_id
        LEFT JOIN tbl_Designation des ON emp.Designation_id = des.Designation_id
        LEFT JOIN tbl_Gender gen ON emp.Gender_id = gen.Gender_id
        LEFT JOIN tblUserLogs logs ON logs.UserId = emp.Employee_id
        WHERE emp.Institute_id = @InstituteId
          AND logs.UserTypeId = 1
          AND logs.IsAppUser = 0
          AND emp.Status = 1
          AND (@DepartmentId = 0 OR emp.Department_id = @DepartmentId)
          AND (@DesignationId = 0 OR emp.Designation_id = @DesignationId)";

                // Fetching non-app users from the database with optional filters
                var nonAppUsers = await _connection.QueryAsync<dynamic>(sql, new { request.InstituteId, request.DepartmentId, request.DesignationId });

                // Check the format and act accordingly
                if (format.ToLower() == "xlsx")
                {
                    // Initialize EPPlus package to create the Excel file
                    using (var package = new ExcelPackage())
                    {
                        // Add a worksheet
                        var worksheet = package.Workbook.Worksheets.Add("Non-App Users");

                        // Add headers to the Excel sheet with "Sr No" as the first column
                        worksheet.Cells[1, 1].Value = "Sr No";
                        worksheet.Cells[1, 2].Value = "Employee ID";
                        worksheet.Cells[1, 3].Value = "Employee Name";
                        worksheet.Cells[1, 4].Value = "Designation";
                        worksheet.Cells[1, 5].Value = "Department";
                        worksheet.Cells[1, 6].Value = "Gender";
                        worksheet.Cells[1, 7].Value = "Mobile Number";

                        if (nonAppUsers.Any())
                        {
                            int row = 2;
                            int srNo = 1;
                            foreach (var user in nonAppUsers)
                            {
                                string employeeName = $"{user.First_Name} {user.Middle_Name} {user.Last_Name}".Trim();
                                worksheet.Cells[row, 1].Value = srNo++;
                                worksheet.Cells[row, 2].Value = user.Employee_id;
                                worksheet.Cells[row, 3].Value = employeeName;
                                worksheet.Cells[row, 4].Value = user.DesignationName;
                                worksheet.Cells[row, 5].Value = user.DepartmentName;
                                worksheet.Cells[row, 6].Value = user.Gender_Type;
                                worksheet.Cells[row, 7].Value = user.mobile_number;
                                row++;
                            }
                        }

                        // Auto-fit columns for better readability
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Convert the worksheet to a byte array
                        var excelData = package.GetAsByteArray();

                        // Return the byte array as a successful response
                        return new ServiceResponse<byte[]>(true, "Excel sheet generated successfully", excelData, 200);
                    }
                }
                else if (format.ToLower() == "csv")
                {
                    // Generate CSV format
                    var csv = new StringBuilder();
                    csv.AppendLine("Sr No,Employee ID,Employee Name,Designation,Department,Gender,Mobile Number");

                    int srNo = 1;
                    foreach (var user in nonAppUsers)
                    {
                        string employeeName = $"{user.First_Name} {user.Middle_Name} {user.Last_Name}".Trim();
                        csv.AppendLine($"{srNo++},{user.Employee_id},{employeeName},{user.DesignationName},{user.DepartmentName},{user.Gender_Type},{user.mobile_number}");
                    }

                    // Convert CSV content to byte array
                    var csvData = Encoding.UTF8.GetBytes(csv.ToString());

                    // Return the CSV file as a successful response
                    return new ServiceResponse<byte[]>(true, "CSV file generated successfully", csvData, 200);
                }
                else
                {
                    // Invalid format
                    return new ServiceResponse<byte[]>(false, "Invalid file format requested. Supported formats: xlsx, csv", null, 400);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response
                return new ServiceResponse<byte[]>(false, $"Error generating file: {ex.Message}", null, 500);
            }
        }
        //public async Task<ServiceResponse<byte[]>> DownloadExcelSheetNonAppUsers(DownloadExcelRequest request)
        //{
        //    try
        //    {
        //        // SQL query to fetch non-app users (IsAppUser = false) based on InstituteId and optional filters for DepartmentId and DesignationId
        //        string sql = @"
        //SELECT emp.Employee_id, emp.First_Name, emp.Middle_Name, emp.Last_Name, emp.Gender_id, 
        //       emp.Department_id, emp.Designation_id, emp.mobile_number, 
        //       dep.DepartmentName, des.DesignationName, gen.Gender_Type
        //FROM tbl_EmployeeProfileMaster emp
        //LEFT JOIN tbl_Department dep ON emp.Department_id = dep.Department_id
        //LEFT JOIN tbl_Designation des ON emp.Designation_id = des.Designation_id
        //LEFT JOIN tbl_Gender gen ON emp.Gender_id = gen.Gender_id
        //LEFT JOIN tblUserLogs logs ON logs.UserId = emp.Employee_id
        //WHERE emp.Institute_id = @InstituteId
        //  AND logs.UserTypeId = 1
        //  AND logs.IsAppUser = 0
        //  AND emp.Status = 1
        //  AND (@DepartmentId = 0 OR emp.Department_id = @DepartmentId)
        //  AND (@DesignationId = 0 OR emp.Designation_id = @DesignationId)";

        //        // Fetching non-app users from the database with optional filters
        //        var nonAppUsers = await _connection.QueryAsync<dynamic>(sql, new { request.InstituteId, request.DepartmentId, request.DesignationId });

        //        // Initialize EPPlus package to create the Excel file
        //        using (var package = new ExcelPackage())
        //        {
        //            // Add a worksheet
        //            var worksheet = package.Workbook.Worksheets.Add("Non-App Users");

        //            // Add headers to the Excel sheet with "Sr No" as the first column
        //            worksheet.Cells[1, 1].Value = "Sr No";
        //            worksheet.Cells[1, 2].Value = "Employee ID";
        //            worksheet.Cells[1, 3].Value = "Employee Name";
        //            worksheet.Cells[1, 4].Value = "Designation";
        //            worksheet.Cells[1, 5].Value = "Department";
        //            worksheet.Cells[1, 6].Value = "Gender";
        //            worksheet.Cells[1, 7].Value = "Mobile Number";

        //            // Check if there are any non-app users to include in the Excel sheet
        //            if (nonAppUsers.Any())
        //            {
        //                // Populate the worksheet with data from the query
        //                int row = 2;
        //                int srNo = 1; // Initialize Sr No counter
        //                foreach (var user in nonAppUsers)
        //                {
        //                    // Combine first name, middle name, and last name for the full employee name
        //                    string employeeName = $"{user.First_Name} {user.Middle_Name} {user.Last_Name}".Trim();

        //                    worksheet.Cells[row, 1].Value = srNo++; // Increment Sr No for each row
        //                    worksheet.Cells[row, 2].Value = user.Employee_id;
        //                    worksheet.Cells[row, 3].Value = employeeName;
        //                    worksheet.Cells[row, 4].Value = user.DesignationName;
        //                    worksheet.Cells[row, 5].Value = user.DepartmentName;
        //                    worksheet.Cells[row, 6].Value = user.Gender_Type;
        //                    worksheet.Cells[row, 7].Value = user.mobile_number;
        //                    row++;
        //                }
        //            }

        //            // Auto-fit columns for better readability
        //            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        //            // Convert the worksheet to a byte array
        //            var excelData = package.GetAsByteArray();

        //            // Return the byte array as a successful response
        //            return new ServiceResponse<byte[]>(true, "Excel sheet generated successfully", excelData, 200);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions and return an error response
        //        return new ServiceResponse<byte[]>(false, $"Error generating Excel sheet: {ex.Message}", null, 500);
        //    }
        //}
        public async Task<ServiceResponse<byte[]>> DownloadEmployeeActivity(DownloadExcelRequest request, string format)
        {
            try
            {
                // SQL query to fetch employee activity logs with optional filters
                string sql = @"
        SELECT emp.Employee_id, emp.First_Name, emp.Middle_Name, emp.Last_Name, 
               emp.Designation_id, emp.Department_id, 
               dep.DepartmentName, des.DesignationName, 
               logs.LoginTime, logs.appVersion
        FROM tblUserLogs logs
        LEFT JOIN tbl_EmployeeProfileMaster emp ON logs.UserId = emp.Employee_id
        LEFT JOIN tbl_Department dep ON emp.Department_id = dep.Department_id
        LEFT JOIN tbl_Designation des ON emp.Designation_id = des.Designation_id
        WHERE emp.Institute_id = @InstituteId
          AND logs.UserTypeId = 1
          AND emp.Status = 1
          AND (@DepartmentId = 0 OR emp.Department_id = @DepartmentId)
          AND (@DesignationId = 0 OR emp.Designation_id = @DesignationId)";

                // Fetching employee activity data from the database with optional filters
                var employeeActivityLogs = await _connection.QueryAsync<dynamic>(sql, new
                {
                    request.InstituteId,
                    request.DepartmentId,
                    request.DesignationId
                });

                // Check the requested format (Excel or CSV)
                if (format.ToLower() == "excel")
                {
                    // Generate Excel
                    using (var package = new ExcelPackage())
                    {
                        // Add a worksheet
                        var worksheet = package.Workbook.Worksheets.Add("Employee Activity Logs");

                        // Add headers to the Excel sheet
                        worksheet.Cells[1, 1].Value = "Sr. No.";
                        worksheet.Cells[1, 2].Value = "Employee ID";
                        worksheet.Cells[1, 3].Value = "Employee Name";
                        worksheet.Cells[1, 4].Value = "Designation";
                        worksheet.Cells[1, 5].Value = "Department";
                        worksheet.Cells[1, 6].Value = "Last Action Taken (Login Time)";
                        worksheet.Cells[1, 7].Value = "App Version";

                        if (employeeActivityLogs.Any())
                        {
                            int row = 2;
                            int serialNumber = 1;

                            foreach (var log in employeeActivityLogs)
                            {
                                string employeeName = $"{log.First_Name} {log.Middle_Name} {log.Last_Name}".Trim();

                                worksheet.Cells[row, 1].Value = serialNumber++;
                                worksheet.Cells[row, 2].Value = log.Employee_id;
                                worksheet.Cells[row, 3].Value = employeeName;
                                worksheet.Cells[row, 4].Value = log.DesignationName;
                                worksheet.Cells[row, 5].Value = log.DepartmentName;
                                worksheet.Cells[row, 6].Value = log.LoginTime?.ToString("yyyy-MM-dd HH:mm:ss");
                                worksheet.Cells[row, 7].Value = log.appVersion;
                                row++;
                            }
                        }

                        // Auto-fit columns
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Convert to byte array and return
                        var excelData = package.GetAsByteArray();
                        return new ServiceResponse<byte[]>(true, "Excel sheet generated successfully", excelData, 200);
                    }
                }
                else if (format.ToLower() == "csv")
                {
                    // Generate CSV
                    var csvBuilder = new StringBuilder();
                    csvBuilder.AppendLine("Sr. No.,Employee ID,Employee Name,Designation,Department,Last Action Taken (Login Time),App Version");

                    if (employeeActivityLogs.Any())
                    {
                        int serialNumber = 1;

                        foreach (var log in employeeActivityLogs)
                        {
                            string employeeName = $"{log.First_Name} {log.Middle_Name} {log.Last_Name}".Trim();
                            string loginTime = log.LoginTime?.ToString("yyyy-MM-dd HH:mm:ss");

                            csvBuilder.AppendLine($"{serialNumber++},{log.Employee_id},{employeeName},{log.DesignationName},{log.DepartmentName},{loginTime},{log.appVersion}");
                        }
                    }

                    // Convert the CSV string to a byte array and return
                    var csvData = Encoding.UTF8.GetBytes(csvBuilder.ToString());
                    return new ServiceResponse<byte[]>(true, "CSV file generated successfully", csvData, 200);
                }
                else
                {
                    // Invalid format requested
                    return new ServiceResponse<byte[]>(false, "Invalid format requested", null, 400);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response
                return new ServiceResponse<byte[]>(false, $"Error generating file: {ex.Message}", null, 500);
            }
        }

        //public async Task<ServiceResponse<byte[]>> DownloadExcelSheetEmployeeActivity(DownloadExcelRequest request)
        //{
        //    try
        //    {
        //        // SQL query to fetch employee activity logs with optional filters
        //        string sql = @"
        //SELECT emp.Employee_id, emp.First_Name, emp.Middle_Name, emp.Last_Name, 
        //       emp.Designation_id, emp.Department_id, 
        //       dep.DepartmentName, des.DesignationName, 
        //       logs.LoginTime, logs.appVersion
        //FROM tblUserLogs logs
        //LEFT JOIN tbl_EmployeeProfileMaster emp ON logs.UserId = emp.Employee_id
        //LEFT JOIN tbl_Department dep ON emp.Department_id = dep.Department_id
        //LEFT JOIN tbl_Designation des ON emp.Designation_id = des.Designation_id
        //WHERE emp.Institute_id = @InstituteId
        //  AND logs.UserTypeId = 1
        //  AND emp.Status = 1
        //  AND (@DepartmentId = 0 OR emp.Department_id = @DepartmentId)
        //  AND (@DesignationId = 0 OR emp.Designation_id = @DesignationId)";

        //        // Fetching employee activity data from the database with optional filters
        //        var employeeActivityLogs = await _connection.QueryAsync<dynamic>(sql, new
        //        {
        //            InstituteId = request.InstituteId,
        //            DepartmentId = request.DepartmentId,
        //            DesignationId = request.DesignationId
        //        });

        //        // Initialize EPPlus package to create the Excel file
        //        using (var package = new ExcelPackage())
        //        {
        //            // Add a worksheet
        //            var worksheet = package.Workbook.Worksheets.Add("Employee Activity Logs");

        //            // Add headers to the Excel sheet
        //            worksheet.Cells[1, 1].Value = "Sr. No.";
        //            worksheet.Cells[1, 2].Value = "Employee ID";
        //            worksheet.Cells[1, 3].Value = "Employee Name";
        //            worksheet.Cells[1, 4].Value = "Designation";
        //            worksheet.Cells[1, 5].Value = "Department";
        //            worksheet.Cells[1, 6].Value = "Last Action Taken (Login Time)";
        //            worksheet.Cells[1, 7].Value = "App Version";

        //            // Check if there are any logs to include in the Excel sheet
        //            if (employeeActivityLogs.Any())
        //            {
        //                // Populate the worksheet with data from the query
        //                int row = 2;
        //                int serialNumber = 1;  // Initialize serial number

        //                foreach (var log in employeeActivityLogs)
        //                {
        //                    // Combine first name, middle name, and last name for the full employee name
        //                    string employeeName = $"{log.First_Name} {log.Middle_Name} {log.Last_Name}".Trim();

        //                    worksheet.Cells[row, 1].Value = serialNumber;  // Serial number
        //                    worksheet.Cells[row, 2].Value = log.Employee_id;
        //                    worksheet.Cells[row, 3].Value = employeeName;
        //                    worksheet.Cells[row, 4].Value = log.DesignationName;
        //                    worksheet.Cells[row, 5].Value = log.DepartmentName;
        //                    worksheet.Cells[row, 6].Value = log.LoginTime?.ToString("yyyy-MM-dd HH:mm:ss");  // Format login time
        //                    worksheet.Cells[row, 7].Value = log.appVersion;

        //                    row++;
        //                    serialNumber++;  // Increment serial number
        //                }
        //            }
        //            else
        //            {
        //                // If no data is found, just return headers
        //                worksheet.Cells[1, 1].Value = "Sr. No.";
        //                worksheet.Cells[1, 2].Value = "Employee ID";
        //                worksheet.Cells[1, 3].Value = "Employee Name";
        //                worksheet.Cells[1, 4].Value = "Designation";
        //                worksheet.Cells[1, 5].Value = "Department";
        //                worksheet.Cells[1, 6].Value = "Last Action Taken (Login Time)";
        //                worksheet.Cells[1, 7].Value = "App Version";
        //            }

        //            // Auto-fit columns for better readability
        //            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        //            // Convert the worksheet to a byte array
        //            var excelData = package.GetAsByteArray();

        //            // Return the byte array as a successful response
        //            return new ServiceResponse<byte[]>(true, "Excel sheet generated successfully", excelData, 200);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions and return an error response
        //        return new ServiceResponse<byte[]>(false, $"Error generating Excel sheet: {ex.Message}", null, 500);
        //    }
        //}
        public async Task<ServiceResponse<List<EmployeeCredentialsResponse>>> GetAllEmployeeLoginCred(EmployeeLoginRequest request)
        {
            try
            {
                // Calculate the number of rows to skip based on the page number and page size
                int offset = (request.PageNumber - 1) * request.PageSize;

                // Define SQL query to fetch employee login credentials with filters and pagination
                string sqlQuery = @"
            SELECT emp.Employee_id AS EmployeeId,
                   emp.First_Name + ' ' + emp.Last_Name AS EmployeeName,
                   des.DesignationName AS Designation,
                   dep.DepartmentName AS Department,
                   gen.Gender_Type as Gender,
                   login.UserName AS LoginId,
                   login.Password,
                   login.UserActivity AS LastActivity
            FROM tblLoginInformationMaster login
            INNER JOIN tbl_EmployeeProfileMaster emp ON login.UserId = emp.Employee_id
            LEFT JOIN tbl_Designation des ON emp.Designation_id = des.Designation_id
            LEFT JOIN tbl_Gender gen ON emp.Gender_id = gen.Gender_id
            LEFT JOIN tbl_Department dep ON emp.Department_id = dep.Department_id
            WHERE login.InstituteId = @InstituteId
            AND (@DepartmentId = 0 OR emp.Department_id = @DepartmentId)
            AND (@DesignationId = 0 OR emp.Designation_id = @DesignationId)
            AND (emp.First_Name + ' ' + emp.Last_Name LIKE '%' + @SearchText + '%' OR @SearchText = '')
            AND login.UserType = 1  -- Assuming UserType = 1 is for Employees
            ORDER BY emp.Employee_id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";  // Pagination

                // Define SQL query to count total records without pagination
                string countQuery = @"
            SELECT COUNT(*)
            FROM tblLoginInformationMaster login
            INNER JOIN tbl_EmployeeProfileMaster emp ON login.UserId = emp.Employee_id
            LEFT JOIN tbl_Designation des ON emp.Designation_id = des.Designation_id
            LEFT JOIN tbl_Department dep ON emp.Department_id = dep.Department_id
            WHERE login.InstituteId = @InstituteId
            AND (@DepartmentId = 0 OR emp.Department_id = @DepartmentId)
            AND (@DesignationId = 0 OR emp.Designation_id = @DesignationId)
            AND (emp.First_Name + ' ' + emp.Last_Name LIKE '%' + @SearchText + '%' OR @SearchText = '')
            AND login.UserType = 1";  // Counting records for employees only

                // Execute the count query to get total records
                int totalRecords = await _connection.ExecuteScalarAsync<int>(countQuery, new
                {
                    InstituteId = request.InstituteId,
                    DepartmentId = request.DepartmentId,
                    DesignationId = request.DesignationId,
                    SearchText = request.SearchText
                });

                // Execute the main query with pagination
                var employeeCredentials = await _connection.QueryAsync<EmployeeCredentialsResponse>(sqlQuery, new
                {
                    InstituteId = request.InstituteId,
                    DepartmentId = request.DepartmentId,
                    DesignationId = request.DesignationId,
                    SearchText = request.SearchText,
                    Offset = offset,
                    PageSize = request.PageSize
                });

                // Format LastActivity to "ddMMyyyy hh:mm tt" if it has a value
                var formattedEmployeeCredentials = employeeCredentials.Select(e => new EmployeeCredentialsResponse
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.EmployeeName,
                    Designation = e.Designation,
                    Department = e.Department,
                    Gender = e.Gender,
                    LoginId = e.LoginId,
                    Password = e.Password,
                    LastActivity = !string.IsNullOrEmpty(e.LastActivity)
                    ? DateTime.Parse(e.LastActivity).ToString("dd-MM-yyyy hh:mm tt")
                    : null
                }).ToList();


                // Set the response data including pagination details
                if (totalRecords > 0)
                {
                    return new ServiceResponse<List<EmployeeCredentialsResponse>>(true, "Employee credentials fetched successfully.", formattedEmployeeCredentials, 200, totalRecords);
                }
                else
                {
                    return new ServiceResponse<List<EmployeeCredentialsResponse>>(false, "no records", new List<EmployeeCredentialsResponse>(), 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeCredentialsResponse>>(false, ex.Message, new List<EmployeeCredentialsResponse>(), 500);
            }
        }
        public async Task<ServiceResponse<List<EmployeeNonAppUsersResponse>>> GetAllEmployeeNonAppUsers(EmployeeLoginRequest request)
        {
            try
            {
                // Calculate the number of rows to skip based on the page number and page size
                int offset = (request.PageNumber - 1) * request.PageSize;

                // Define SQL query to fetch non-app users with filters and pagination
                string sqlQuery = @"
            SELECT emp.Employee_id AS EmployeeId,
                   emp.First_Name + ' ' + emp.Last_Name AS EmployeeName,
                   des.DesignationName AS Designation,
                   dep.DepartmentName AS Department,
                   gen.Gender_Type as Gender,
                   emp.mobile_number AS MobileNumber
            FROM tbl_EmployeeProfileMaster emp
            LEFT JOIN tbl_Designation des ON emp.Designation_id = des.Designation_id
            LEFT JOIN tbl_Department dep ON emp.Department_id = dep.Department_id
            LEFT JOIN tbl_Gender gen ON emp.Gender_id = gen.Gender_id
            LEFT JOIN tblLoginInformationMaster login ON login.UserId = emp.Employee_id
LEFT JOIN tblUserLogs us ON us.UserId = emp.Employee_id
            WHERE us.IsAppUser = 0  -- Non-app users (no login information)
            AND emp.Institute_id = @InstituteId
            AND (@DepartmentId = 0 OR emp.Department_id = @DepartmentId)
            AND (@DesignationId = 0 OR emp.Designation_id = @DesignationId)
            AND (emp.First_Name + ' ' + emp.Last_Name LIKE '%' + @SearchText + '%' OR @SearchText = '')
            ORDER BY emp.Employee_id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";  // Pagination

                // Define SQL query to count total records without pagination
                string countQuery = @"
            SELECT COUNT(*)
            FROM tbl_EmployeeProfileMaster emp
            LEFT JOIN tblLoginInformationMaster login ON login.UserId = emp.Employee_id
LEFT JOIN tblUserLogs us ON us.UserId = emp.Employee_id
            WHERE us.IsAppUser = 0  -- Non-app users (no login information)
            AND emp.Institute_id = @InstituteId
            AND (@DepartmentId = 0 OR emp.Department_id = @DepartmentId)
            AND (@DesignationId = 0 OR emp.Designation_id = @DesignationId)
            AND (emp.First_Name + ' ' + emp.Last_Name LIKE '%' + @SearchText + '%' OR @SearchText = '')";

                // Execute the count query to get total records
                int totalRecords = await _connection.ExecuteScalarAsync<int>(countQuery, new
                {
                    InstituteId = request.InstituteId,
                    DepartmentId = request.DepartmentId,
                    DesignationId = request.DesignationId,
                    SearchText = request.SearchText
                });

                // Execute the main query with pagination
                var nonAppUsers = await _connection.QueryAsync<EmployeeNonAppUsersResponse>(sqlQuery, new
                {
                    InstituteId = request.InstituteId,
                    DepartmentId = request.DepartmentId,
                    DesignationId = request.DesignationId,
                    SearchText = request.SearchText,
                    Offset = offset,
                    PageSize = request.PageSize
                });
                if (totalRecords > 0)
                {
                    return new ServiceResponse<List<EmployeeNonAppUsersResponse>>(true, "Non-app users fetched successfully.", nonAppUsers.ToList(), 200, totalRecords);
                }
                else
                {
                    return new ServiceResponse<List<EmployeeNonAppUsersResponse>>(false, "No records", [], 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeNonAppUsersResponse>>(false, ex.Message, [], 500);
            }
        }
        public async Task<ServiceResponse<List<EmployeeActivityResponse>>> GetAllEmployeeActivity(EmployeeLoginRequest request)
        {
            try
            {
                // Calculate the number of rows to skip based on the page number and page size
                int offset = (request.PageNumber - 1) * request.PageSize;

                // Define SQL query to fetch employee activity with filters and pagination
                string sqlQuery = @"
        WITH LatestUserLogs AS (
            SELECT logs.UserId,
                   logs.LoginTime,
                   logs.version_sdkInt,
                   ROW_NUMBER() OVER (PARTITION BY logs.UserId ORDER BY logs.LoginTime DESC) AS RowNum
            FROM tblUserLogs logs
            WHERE logs.UserTypeId = 1  -- Assuming UserTypeId = 1 is for Employees
        )
        SELECT emp.Employee_id AS EmployeeId,
               emp.First_Name + ' ' + emp.Last_Name AS EmployeeName,
               des.DesignationName AS Designation,
               dep.DepartmentName AS Department,
               emp.mobile_number AS MobileNumber,
               logs.LoginTime AS LastActionTaken,  -- Latest login action
               logs.version_sdkInt AS Version
        FROM LatestUserLogs logs
        INNER JOIN tbl_EmployeeProfileMaster emp ON logs.UserId = emp.Employee_id
        LEFT JOIN tbl_Designation des ON emp.Designation_id = des.Designation_id
        LEFT JOIN tbl_Department dep ON emp.Department_id = dep.Department_id
        WHERE emp.Institute_id = @InstituteId
        AND (@DepartmentId = 0 OR emp.Department_id = @DepartmentId)
        AND (@DesignationId = 0 OR emp.Designation_id = @DesignationId)
        AND (emp.First_Name + ' ' + emp.Last_Name LIKE '%' + @SearchText + '%' OR @SearchText = '')
        AND logs.RowNum = 1  -- Ensure we only take the latest log per user
        ORDER BY emp.Employee_id
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";  // Pagination

                // Define SQL query to count total records without pagination
                string countQuery = @"
        SELECT COUNT(DISTINCT emp.Employee_id)
        FROM tblUserLogs logs
        INNER JOIN tbl_EmployeeProfileMaster emp ON logs.UserId = emp.Employee_id
        WHERE emp.Institute_id = @InstituteId
        AND (@DepartmentId = 0 OR emp.Department_id = @DepartmentId)
        AND (@DesignationId = 0 OR emp.Designation_id = @DesignationId)
        AND (emp.First_Name + ' ' + emp.Last_Name LIKE '%' + @SearchText + '%' OR @SearchText = '')
        AND logs.UserTypeId = 1";  // Assuming UserTypeId = 1 is for Employees

                // Execute the count query to get total records
                int totalRecords = await _connection.ExecuteScalarAsync<int>(countQuery, new
                {
                    InstituteId = request.InstituteId,
                    DepartmentId = request.DepartmentId,
                    DesignationId = request.DesignationId,
                    SearchText = request.SearchText
                });

                // Execute the main query with pagination
                var employeeActivity = await _connection.QueryAsync<EmployeeActivityResponse>(sqlQuery, new
                {
                    InstituteId = request.InstituteId,
                    DepartmentId = request.DepartmentId,
                    DesignationId = request.DesignationId,
                    SearchText = request.SearchText,
                    Offset = offset,
                    PageSize = request.PageSize
                });
                var formattedemployeeActivity = employeeActivity.Select(e => new EmployeeActivityResponse
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.EmployeeName,
                    Designation = e.Designation,
                    Departemnt = e.Departemnt,
                    MobileNumber = e.MobileNumber,
                    Version = e.Version,
                    LastActionTaken = !string.IsNullOrEmpty(e.LastActionTaken)
                   ? DateTime.Parse(e.LastActionTaken).ToString("dd-MM-yyyy hh:mm tt")
                   : null
                }).ToList();
                if (totalRecords > 0)
                {
                    return new ServiceResponse<List<EmployeeActivityResponse>>(true, "Employee activity fetched successfully.", formattedemployeeActivity.ToList(), 200, totalRecords);
                }
                else
                {
                    return new ServiceResponse<List<EmployeeActivityResponse>>(false, "No records.", new List<EmployeeActivityResponse>(), 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeActivityResponse>>(false, ex.Message, new List<EmployeeActivityResponse>(), 500);
            }
        }
        public async Task<ServiceResponse<EmployeeLoginResposne>> UserLogin(string username)
        {
            try
            {
                // Ensure the connection is open
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }

                // Query to get user login info
                var loginInfoQuery = @"
            SELECT TOP 1 [LoginInfoId], [UserId], [UserType], [UserName], [Password], [InstituteId], [UserActivity]
            FROM [tblLoginInformationMaster]
            WHERE [UserName] = @Username";

                var loginInfo = await _connection.QueryFirstOrDefaultAsync<dynamic>(loginInfoQuery, new { Username = username });

                if (loginInfo == null)
                {
                    return new ServiceResponse<EmployeeLoginResposne>(false, "Invalid Username.", new EmployeeLoginResposne(), 500);
                }
                var response = new EmployeeLoginResposne
                {
                    Username = loginInfo.UserName,
                    InstituteId = loginInfo.InstituteId
                };
                // Query to get institute logo based on InstituteId
                var instituteLogoQuery = @"
            SELECT TOP 1 [InstituteLogoId], [InstituteId], [InstituteLogo]
            FROM [tbl_InstituteLogo]
            WHERE [InstituteId] = @InstituteId";

                var instituteLogo = await _connection.QueryFirstOrDefaultAsync<dynamic>(instituteLogoQuery, new { InstituteId = loginInfo.InstituteId });

                if (instituteLogo != null)
                {
                    response.InstituteLogo = instituteLogo.InstituteLogo;
                }

                // Set success response
                return new ServiceResponse<EmployeeLoginResposne>(true, "Login successful.", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeLoginResposne>(false, ex.Message, new EmployeeLoginResposne(), 500);
            }
            finally
            {
                // Ensure the connection is closed
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }
        public async Task<ServiceResponse<LoginResposne>> UserLoginPasswordScreen(UserLoginRequest request)
        {
            try
            {
                var response = new LoginResposne();
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }

                // Step 1: Validate login credentials
                var loginInfoQuery = @"
            SELECT TOP 1 [UserId], [UserType], [UserName], [Password], [InstituteId]
            FROM tblLoginInformationMaster
            WHERE [UserName] = @Username AND [Password] = @Password";

                var loginInfo = await _connection.QueryFirstOrDefaultAsync<dynamic>(loginInfoQuery, new { Username = request.Username, Password = request.Password });

                if (loginInfo == null)
                {
                    return new ServiceResponse<LoginResposne>(false, "Invalid username or password.", new LoginResposne(), 500);
                }

                // Step 2: Fetch user-specific information based on UserType
                if (loginInfo.UserType == 1) // Employee
                {
                    var employeeQuery = @"
                SELECT TOP 1 Employee_id, [First_Name], [Last_Name], [Institute_id]
                FROM tbl_EmployeeProfileMaster
                WHERE [Employee_id] = @UserId";

                    var employeeInfo = await _connection.QueryFirstOrDefaultAsync<dynamic>(employeeQuery, new { UserId = loginInfo.UserId });

                    if (employeeInfo != null)
                    {
                        response = new LoginResposne
                        {
                            Username = $"{employeeInfo.First_Name} {employeeInfo.Last_Name}",
                            InstituteId = employeeInfo.Institute_id,
                            UserType = "Employee",
                            UserId = employeeInfo.Employee_id
                        };
                    }
                }
                else if (loginInfo.UserType == 2) // Student
                {
                    var studentQuery = @"
                SELECT TOP 1 student_id, [First_Name], [Last_Name], [Institute_id]
                FROM tbl_StudentMaster
                WHERE [student_id] = @UserId";

                    var studentInfo = await _connection.QueryFirstOrDefaultAsync<dynamic>(studentQuery, new { UserId = loginInfo.UserId });

                    if (studentInfo != null)
                    {
                        response = new LoginResposne
                        {
                            Username = $"{studentInfo.First_Name} {studentInfo.Last_Name}",
                            InstituteId = studentInfo.Institute_id,
                            UserType = "Student",
                            UserId = studentInfo.student_id
                        };
                    }
                }
                else if (loginInfo.UserType == 3) // Chairman
                {
                    var chairmanQuery = @"
                SELECT TOP 1 [ChairmanID], [Name], [MobileNumber], [EmailID], [UserName], [InstituteId]
                FROM [tblInstituteChairman]
                WHERE [ChairmanID] = @UserId";

                    var chairmanInfo = await _connection.QueryFirstOrDefaultAsync<dynamic>(chairmanQuery, new { UserId = loginInfo.UserId });

                    if (chairmanInfo != null)
                    {
                        response = new LoginResposne
                        {
                            Username = chairmanInfo.Name,
                            InstituteId = chairmanInfo.InstituteId,
                            UserType = "Chairman",
                            UserId = chairmanInfo.ChairmanID
                        };
                    }
                }
                else
                {
                    return new ServiceResponse<LoginResposne>(false, "Unsupported user type.", new LoginResposne(), 500);
                }

                // Step 3: Fetch roles for the user
                var roleQuery = @"
            SELECT DISTINCT [RoleID]
            FROM [tblUserRoleMapping]
            WHERE [EmployeeID] = @UserId";

                var roleIds = await _connection.QueryAsync<int>(roleQuery, new { UserId = response.UserId });

                if (!roleIds.Any())
                {
                    return new ServiceResponse<LoginResposne>(false, "No roles assigned to this user.", new LoginResposne(), 500);
                }

                // Step 4: Fetch modules, submodules, and functionalities for all roles
                var modulesQuery = @"
            SELECT DISTINCT m.[ModuleID], m.[ModuleName], m.[IsActive],
                            sm.[SubModuleID], sm.[SubModuleName], sm.[IsActive] AS SubModuleIsActive,
                            f.[FunctionalityId], f.[Functionality], f.[IsActive] AS FunctionalityIsActive
            FROM [tblUserRoleSettingMapping] rsm
            INNER JOIN [tblModule] m ON rsm.[ModuleID] = m.[ModuleID]
            INNER JOIN [tblSubModule] sm ON rsm.[SubModuleID] = sm.[SubModuleID]
            INNER JOIN [tblFunctionality] f ON rsm.[FunctionalityID] = f.[FunctionalityId]
            WHERE rsm.[RoleID] IN @RoleIds";

                var moduleData = await _connection.QueryAsync<dynamic>(modulesQuery, new { RoleIds = roleIds.ToList() });

                // Step 5: Map the data into the desired structure
                var modules = moduleData.GroupBy(m => new { m.ModuleID, m.ModuleName, m.IsActive })
                                        .Select(moduleGroup => new ModuleResponse
                                        {
                                            ModuleID = moduleGroup.Key.ModuleID,
                                            ModuleName = moduleGroup.Key.ModuleName,
                                            IsActive = moduleGroup.Key.IsActive,
                                            Submodules = moduleGroup.GroupBy(sm => new { sm.SubModuleID, sm.SubModuleName, sm.SubModuleIsActive })
                                                                    .Select(subModuleGroup => new SubModuleResponse
                                                                    {
                                                                        SubModuleID = subModuleGroup.Key.SubModuleID,
                                                                        SubModuleName = subModuleGroup.Key.SubModuleName,
                                                                        IsActive = subModuleGroup.Key.SubModuleIsActive,
                                                                        Functionalities = subModuleGroup.Select(f => new FunctionalityResponse
                                                                        {
                                                                            FunctionalityId = f.FunctionalityId,
                                                                            Functionality = f.Functionality,
                                                                            IsActive = f.FunctionalityIsActive
                                                                        }).ToList()
                                                                    }).ToList()
                                        }).ToList();

                response.ModulesAndSubmodules = modules;
                return new ServiceResponse<LoginResposne>(true, "Login successful.", response, 500);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoginResposne>(false, ex.Message, new LoginResposne(), 500);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }
        public async Task<ServiceResponse<string>> CaptureDeviceDetails(DeviceDetails request)
        {
            try
            {
                var updateLogQuery = @"
UPDATE tblUserLogs
SET 
    [LoginTime] = @LoginTime,
    [IsAppUser] = @IsAppUser,
    [brand] = @Brand,
    [device] = @Device,
    [fingerprint] = @Fingerprint,
    [model] = @Model,
    [serialNumber] = @SerialNumber,
    [type] = @Type,
    [version_sdkInt] = @VersionSdkInt,
    [version_securityPatch] = @VersionSecurityPatch,
    [build_id] = @BuildId,
    [isPhysicalDevice] = @IsPhysicalDevice,
    [systemName] = @SystemName,
    [systemVersion] = @SystemVersion,
    [utsname_version] = @UtsnameVersion,
    [operSysName] = @OperSysName,
    [browserName] = @BrowserName,
    [appName] = @AppName,
    [appVersion] = @AppVersion,
    [deviceMemory] = @DeviceMemory,
    [platform] = @Platform,
    [kernelVersion] = @KernelVersion,
    [computerName] = @ComputerName,
    [systemGUID] = @SystemGUID
WHERE 
    [UserId] = @UserId AND [UserTypeId] = @UserTypeId";

                var logParams = new
                {
                    UserId = request.UserId,
                    UserTypeId = request.UserTypeId,
                    LoginTime = DateTime.Now,
                    request.IsAppUser,
                    request.Brand,
                    request.Device,
                    request.Fingerprint,
                    request.Model,
                    request.SerialNumber,
                    request.Type,
                    request.VersionSdkInt,
                    request.VersionSecurityPatch,
                    request.BuildId,
                    request.IsPhysicalDevice,
                    request.SystemName,
                    request.SystemVersion,
                    request.UtsnameVersion,
                    request.OperSysName,
                    request.BrowserName,
                    request.AppName,
                    request.AppVersion,
                    request.DeviceMemory,
                    request.Platform,
                    request.KernelVersion,
                    request.ComputerName,
                    request.SystemGUID
                };

                await _connection.ExecuteAsync(updateLogQuery, logParams);


                return new ServiceResponse<string>(true, "Operation successful", string.Empty, 500);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }
        public async Task<ServiceResponse<string>> UserLogout(string username)
        {
            try
            {
                // Step 1: Fetch UserId and UserTypeId from tblLoginInformationMaster
                string fetchUserInfoQuery = @"
            SELECT UserId, UserType 
            FROM tblLoginInformationMaster
            WHERE UserName = @username";

                var userInfo = await _connection.QueryFirstOrDefaultAsync<dynamic>(fetchUserInfoQuery, new { username });

                if (userInfo == null)
                {
                    return new ServiceResponse<string>(false, "User not found", null, 404);
                }

                int userId = userInfo.UserId;
                int userTypeId = userInfo.UserType;

                // Step 2: Check if the user is logged in by checking tblUserLogs
                string checkUserLogQuery = @"
            SELECT LogsId 
            FROM tblUserLogs
            WHERE UserId = @userId AND UserTypeId = @userTypeId AND LogoutTime IS NULL";

                var logEntry = await _connection.QueryFirstOrDefaultAsync<int?>(checkUserLogQuery, new { userId, userTypeId });

                if (logEntry == null)
                {
                    return new ServiceResponse<string>(false, "No active session found for this user", null, 404);
                }

                // Step 3: Update the LogoutTime for the active session
                string updateQuery = @"
            UPDATE tblUserLogs
            SET LogoutTime = @LogoutTime
            WHERE LogsId = @LogsId";

                await _connection.ExecuteAsync(updateQuery, new { LogoutTime = DateTime.Now, LogsId = logEntry });

                return new ServiceResponse<string>(true, "User successfully logged out", null, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, $"Error: {ex.Message}", null, 500);
            }
        }
        public async Task<ServiceResponse<UserSwitchOverResponse>> UserSwitchOver(UserSwitchOverRequest request)
        {
            if (string.IsNullOrEmpty(request.MobileNumber) || request.InstituteId <= 0)
            {
                return new ServiceResponse<UserSwitchOverResponse>(
                    false, "Invalid request parameters.", null, 400);
            }

            var response = new UserSwitchOverResponse
            {
                Students = new List<StudentResponse>(),
                Employees = new List<EmployeeResponse>()
            };

            // Fetch student details using parent's mobile number
            var studentQuery = @"
       SELECT 
    std.student_id AS UserId,
    li.UserName,
    std.First_Name as FirstName,
    std.Middle_Name as MiddleName,
    std.Last_Name as LastName,
    cls.class_name AS ClassName,
    sec.section_name AS SectionName
FROM 
tbl_StudentMaster std
  
JOIN 
     tbl_StudentParentsInfo spi  ON std.student_id = spi.Student_id
JOIN 
    tblLoginInformationMaster li ON li.UserId = std.student_id -- Assuming this relation
JOIN 
    tbl_Class cls ON cls.class_id = std.class_id
JOIN 
    tbl_Section sec ON sec.section_id = std.section_id
WHERE 
    spi.Mobile_Number = @MobileNumber AND std.Institute_id = @InstituteId";

            var students = await _connection.QueryAsync<StudentResponse>(studentQuery, new { request.MobileNumber, request.InstituteId });
            response.Students.AddRange(students);

            // Fetch employee details using the mobile number
            var employeeQuery = @"
        SELECT 
            emp.Employee_id AS UserId, 
            li.UserName,
            emp.First_Name AS FirstName, 
            emp.Middle_Name AS MiddleName, 
            emp.Last_Name AS LastName, 
            dep.DepartmentName, 
            des.DesignationName
        FROM 
            tbl_EmployeeProfileMaster emp
        JOIN 
            tblLoginInformationMaster li ON li.UserId = emp.Employee_id -- Assuming this relation
        JOIN 
            tbl_Department dep ON dep.Department_id = emp.Department_id
        JOIN 
            tbl_Designation des ON des.Designation_id = emp.Designation_id
        WHERE 
            emp.mobile_number = @MobileNumber AND emp.Institute_id = @InstituteId";

            var employees = await _connection.QueryAsync<EmployeeResponse>(employeeQuery, new { request.MobileNumber, request.InstituteId });
            response.Employees.AddRange(employees);

            // Prepare and return the response
            if (response.Students.Count == 0 && response.Employees.Count == 0)
            {
                return new ServiceResponse<UserSwitchOverResponse>(
                    false, "No users found for the provided mobile number and institute ID.", response, 404);
            }

            return new ServiceResponse<UserSwitchOverResponse>(
                true, "User switch over successful.", response, 200, response.Students.Count + response.Employees.Count);
        }
        public async Task<ServiceResponse<ForgetPasswordResponse>> ForgetPassword(ForgotPassword request)
        {
            if (string.IsNullOrEmpty(request.UserEmailOrPhoneOrUsername))
            {
                return new ServiceResponse<ForgetPasswordResponse>(
                    false, "Invalid request parameters.", null, 400);
            }

            var response = new ForgetPasswordResponse();

            // Check if the input is a username in tblLoginInformationMaster
            var loginQuery = @"
    SELECT 
        UserId AS UserId, 
        UserName, 
        UserType AS Usertype,
        '' AS Email  -- Email is not stored in this table
    FROM 
        tblLoginInformationMaster 
    WHERE 
        UserName = @UserInput";

            var loginResult = await _connection.QueryFirstOrDefaultAsync<ForgetPasswordResponse>(loginQuery, new { UserInput = request.UserEmailOrPhoneOrUsername });

            if (loginResult != null)
            {
                response.UserId = loginResult.UserId;
                response.UserName = loginResult.UserName;
                

                // Now, fetch the email based on UserType
                if (loginResult.Usertype == "1")  // Employee
                {
                    // Fetch email from tbl_EmployeeProfileMaster
                    var employeeEmailQuery = @"
            SELECT 
                emp.EmailID AS Email
            FROM 
                tbl_EmployeeProfileMaster emp
            WHERE 
                emp.Employee_id = @UserId";

                    var employeeEmailResult = await _connection.QueryFirstOrDefaultAsync<string>(employeeEmailQuery, new { UserId = response.UserId });
                    response.Email = employeeEmailResult ?? string.Empty; // Default to empty string if not found
                    response.Usertype = "Employee";
                }
                else if (loginResult.Usertype == "2")  // Student
                {
                    // Fetch email from tbl_StudentParentsInfo
                    var studentEmailQuery = @"
            SELECT 
                spi.Email_id AS Email
            FROM 
                tbl_StudentParentsInfo spi
            WHERE 
                spi.Student_id = @UserId";

                    var studentEmailResult = await _connection.QueryFirstOrDefaultAsync<string>(studentEmailQuery, new { UserId = response.UserId });
                    response.Email = studentEmailResult ?? string.Empty; // Default to empty string if not found
                    response.Usertype = "Student";
                }

                return new ServiceResponse<ForgetPasswordResponse>(
                    true, "User found by username.", response, 200);
            }

            // Search in tbl_EmployeeProfileMaster for mobile number or email
            var employeeQuery = @"
    SELECT 
        emp.Employee_id AS UserId, 
        emp.First_Name + ' ' + ISNULL(emp.Middle_Name, '') + ' ' + emp.Last_Name AS UserName, 
        'Employee' AS UserType,
        emp.EmailID AS Email
    FROM 
        tbl_EmployeeProfileMaster emp
    WHERE 
        emp.mobile_number = @UserInput OR emp.EmailID = @UserInput";

            var employeeResult = await _connection.QueryFirstOrDefaultAsync<ForgetPasswordResponse>(employeeQuery, new { UserInput = request.UserEmailOrPhoneOrUsername });

            if (employeeResult != null)
            {
                response.UserId = employeeResult.UserId;
                response.UserName = employeeResult.UserName;
                response.Usertype = "Employee";
                response.Email = employeeResult.Email;

                return new ServiceResponse<ForgetPasswordResponse>(
                    true, "User found by mobile number or email (employee).", response, 200);
            }

            // Search in tbl_StudentParentsInfo for mobile number or email (student's parent)
            var studentQuery = @"
    SELECT 
        spi.Student_id AS UserId, 
        spi.First_Name + ' ' + ISNULL(spi.Middle_Name, '') + ' ' + spi.Last_Name AS UserName, 
        'StudentParent' AS UserType,
        spi.Email_id AS Email
    FROM 
        tbl_StudentParentsInfo spi
    WHERE 
        spi.Mobile_Number = @UserInput OR spi.Email_id = @UserInput";

            var studentResult = await _connection.QueryFirstOrDefaultAsync<ForgetPasswordResponse>(studentQuery, new { UserInput = request.UserEmailOrPhoneOrUsername });

            if (studentResult != null)
            {
                response.UserId = studentResult.UserId;
                response.UserName = studentResult.UserName;
                response.Usertype = "Student";
                response.Email = studentResult.Email;

                return new ServiceResponse<ForgetPasswordResponse>(
                    true, "User found by mobile number or email (student's parent).", response, 200);
            }

            // If no user is found
            return new ServiceResponse<ForgetPasswordResponse>(
                false, "No user found with the provided details.", null, 404);
        }
        public Task<ServiceResponse<bool>> ResetPassword(ResetPassword request)
        {
            return Task.Run(async () =>
            {
                if (request.UserId <= 0 || string.IsNullOrEmpty(request.UserName) ||
                    string.IsNullOrEmpty(request.NewPassword))
                {
                    return new ServiceResponse<bool>(false,
                        "Invalid request parameters.", false, 400);
                }

                int userTypeInt = request.Usertype.Equals("Employee",
                    StringComparison.OrdinalIgnoreCase) ? 1 :
                    request.Usertype.Equals("Student", StringComparison.OrdinalIgnoreCase) ? 2 : 0;

                if (userTypeInt == 0)
                {
                    return new ServiceResponse<bool>(false,
                        "Invalid user type.", false, 400);
                }

                var userQuery = @"
        SELECT LoginInfoId, UserId, UserType, UserName 
        FROM tblLoginInformationMaster
        WHERE UserId = @UserId AND UserName = @UserName AND UserType = @UserType";

                var userResult = await _connection.QueryFirstOrDefaultAsync<dynamic>(
                    userQuery,
                    new { UserId = request.UserId, UserName = request.UserName, UserType = userTypeInt });

                if (userResult == null)
                {
                    return new ServiceResponse<bool>(false,
                        "User not found with the provided details.", false, 404);
                }

                var updatePasswordQuery = @"
        UPDATE tblLoginInformationMaster
        SET Password = @NewPassword
        WHERE UserId = @UserId AND UserName = @UserName AND UserType = @UserType";

                var rowsAffected = await _connection.ExecuteAsync(
                    updatePasswordQuery,
                    new
                    {
                        NewPassword = request.NewPassword,
                        UserId = request.UserId,
                        UserName = request.UserName,
                        UserType = userTypeInt
                    });

                return rowsAffected > 0
                    ? new ServiceResponse<bool>(true, "Password reset successfully.", true, 200)
                    : new ServiceResponse<bool>(false, "Password reset failed.", false, 500);
            });
        }
    }
}