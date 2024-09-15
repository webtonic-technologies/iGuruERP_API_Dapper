using Dapper;
using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
using Employee_API.Repository.Interfaces;
using OfficeOpenXml;
using System.Data;

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
        public async Task<ServiceResponse<byte[]>> DownloadExcelSheet(DownloadExcelRequest request)
        {
            try
            {
                // SQL query to fetch employee details, handling optional filters for DepartmentId and DesignationId
                string sql = @"
        SELECT emp.Employee_id, emp.First_Name, emp.Middle_Name, emp.Last_Name, emp.Gender_id, 
               emp.Department_id, emp.Designation_id, emp.mobile_number, emp.Date_of_Birth, emp.EmailID,
               dep.DepartmentName, des.DesignationName, gen.Gender_Type
        FROM tbl_EmployeeProfileMaster emp
        LEFT JOIN tbl_Department dep ON emp.Department_id = dep.Department_id
        LEFT JOIN tbl_Designation des ON emp.Designation_id = des.Designation_id
        LEFT JOIN tbl_Gender gen ON emp.Gender_id = gen.Gender_id
        WHERE emp.Institute_id = @InstituteId AND emp.Status = 1
        AND (@DepartmentId = 0 OR emp.Department_id = @DepartmentId)
        AND (@DesignationId = 0 OR emp.Designation_id = @DesignationId)";

                // Execute the query, using optional filters
                var employees = await _connection.QueryAsync<dynamic>(sql, new { request.InstituteId, request.DepartmentId, request.DesignationId });

                // Initialize EPPlus package to create the Excel sheet
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Employee Details");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Employee ID";
                    worksheet.Cells[1, 2].Value = "Employee Name";
                    worksheet.Cells[1, 3].Value = "Department";
                    worksheet.Cells[1, 4].Value = "Designation";
                    worksheet.Cells[1, 5].Value = "Gender";
                    worksheet.Cells[1, 6].Value = "Mobile";
                    worksheet.Cells[1, 7].Value = "Date of Birth";
                    worksheet.Cells[1, 8].Value = "Email";

                    // Check if there are employees returned from the query
                    if (employees.Any())
                    {
                        // Fill the worksheet with employee data
                        int row = 2;

                        foreach (var employee in employees)
                        {
                            // Combine first name, middle name, and last name into employee name
                            string employeeName = $"{employee.First_Name} {employee.Middle_Name} {employee.Last_Name}".Trim();

                            worksheet.Cells[row, 1].Value = employee.Employee_id;
                            worksheet.Cells[row, 2].Value = employeeName;
                            worksheet.Cells[row, 3].Value = employee.DepartmentName;
                            worksheet.Cells[row, 4].Value = employee.DesignationName;
                            worksheet.Cells[row, 5].Value = employee.Gender_Type;
                            worksheet.Cells[row, 6].Value = employee.mobile_number;
                            worksheet.Cells[row, 7].Value = employee.Date_of_Birth;
                            worksheet.Cells[row, 8].Value = employee.EmailID;
                            row++;
                        }

                        // Auto-fit the columns for better visibility
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    }
                    else
                    {
                        // If no data is found, return an empty Excel with only headers
                        worksheet.Cells[1, 1].Value = "Employee ID";
                        worksheet.Cells[1, 2].Value = "Employee Name";
                        worksheet.Cells[1, 3].Value = "Department";
                        worksheet.Cells[1, 4].Value = "Designation";
                        worksheet.Cells[1, 5].Value = "Gender";
                        worksheet.Cells[1, 6].Value = "Mobile";
                        worksheet.Cells[1, 7].Value = "Date of Birth";
                        worksheet.Cells[1, 8].Value = "Email";
                    }

                    // Convert the worksheet into a byte array
                    var excelData = package.GetAsByteArray();

                    // Return the byte array as the service response
                    return new ServiceResponse<byte[]>(true, "Excel file generated successfully", excelData, 200);
                }
            }
            catch (Exception ex)
            {
                // Handle any errors during the process
                return new ServiceResponse<byte[]>(false, $"Error generating Excel file: {ex.Message}", null, 500);
            }
        }
        public async Task<ServiceResponse<byte[]>> DownloadExcelSheetNonAppUsers(DownloadExcelRequest request)
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

                // Initialize EPPlus package to create the Excel file
                using (var package = new ExcelPackage())
                {
                    // Add a worksheet
                    var worksheet = package.Workbook.Worksheets.Add("Non-App Users");

                    // Add headers to the Excel sheet
                    worksheet.Cells[1, 1].Value = "Employee ID";
                    worksheet.Cells[1, 2].Value = "Employee Name";
                    worksheet.Cells[1, 3].Value = "Designation";
                    worksheet.Cells[1, 4].Value = "Department";
                    worksheet.Cells[1, 5].Value = "Gender";
                    worksheet.Cells[1, 6].Value = "Mobile Number";

                    // Check if there are any non-app users to include in the Excel sheet
                    if (nonAppUsers.Any())
                    {
                        // Populate the worksheet with data from the query
                        int row = 2;
                        foreach (var user in nonAppUsers)
                        {
                            // Combine first name, middle name, and last name for the full employee name
                            string employeeName = $"{user.First_Name} {user.Middle_Name} {user.Last_Name}".Trim();

                            worksheet.Cells[row, 1].Value = user.Employee_id;
                            worksheet.Cells[row, 2].Value = employeeName;
                            worksheet.Cells[row, 3].Value = user.DesignationName;
                            worksheet.Cells[row, 4].Value = user.DepartmentName;
                            worksheet.Cells[row, 5].Value = user.Gender_Type;
                            worksheet.Cells[row, 6].Value = user.mobile_number;
                            row++;
                        }
                    }
                    else
                    {
                        // If no data is found, just return headers
                        worksheet.Cells[1, 1].Value = "Employee ID";
                        worksheet.Cells[1, 2].Value = "Employee Name";
                        worksheet.Cells[1, 3].Value = "Designation";
                        worksheet.Cells[1, 4].Value = "Department";
                        worksheet.Cells[1, 5].Value = "Gender";
                        worksheet.Cells[1, 6].Value = "Mobile Number";
                    }

                    // Auto-fit columns for better readability
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Convert the worksheet to a byte array
                    var excelData = package.GetAsByteArray();

                    // Return the byte array as a successful response
                    return new ServiceResponse<byte[]>(true, "Excel sheet generated successfully", excelData, 200);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response
                return new ServiceResponse<byte[]>(false, $"Error generating Excel sheet: {ex.Message}", null, 500);
            }
        }
        public async Task<ServiceResponse<byte[]>> DownloadExcelSheetEmployeeActivity(DownloadExcelRequest request)
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
                    InstituteId = request.InstituteId,
                    DepartmentId = request.DepartmentId,
                    DesignationId = request.DesignationId
                });

                // Initialize EPPlus package to create the Excel file
                using (var package = new ExcelPackage())
                {
                    // Add a worksheet
                    var worksheet = package.Workbook.Worksheets.Add("Employee Activity Logs");

                    // Add headers to the Excel sheet
                    worksheet.Cells[1, 1].Value = "Employee ID";
                    worksheet.Cells[1, 2].Value = "Employee Name";
                    worksheet.Cells[1, 3].Value = "Designation";
                    worksheet.Cells[1, 4].Value = "Department";
                    worksheet.Cells[1, 5].Value = "Last Action Taken (Login Time)";
                    worksheet.Cells[1, 6].Value = "App Version";

                    // Check if there are any logs to include in the Excel sheet
                    if (employeeActivityLogs.Any())
                    {
                        // Populate the worksheet with data from the query
                        int row = 2;
                        foreach (var log in employeeActivityLogs)
                        {
                            // Combine first name, middle name, and last name for the full employee name
                            string employeeName = $"{log.First_Name} {log.Middle_Name} {log.Last_Name}".Trim();

                            worksheet.Cells[row, 1].Value = log.Employee_id;
                            worksheet.Cells[row, 2].Value = employeeName;
                            worksheet.Cells[row, 3].Value = log.DesignationName;
                            worksheet.Cells[row, 4].Value = log.DepartmentName;
                            worksheet.Cells[row, 5].Value = log.LoginTime?.ToString("yyyy-MM-dd HH:mm:ss");  // Format login time
                            worksheet.Cells[row, 6].Value = log.appVersion;
                            row++;
                        }
                    }
                    else
                    {
                        // If no data is found, just return headers
                        worksheet.Cells[1, 1].Value = "Employee ID";
                        worksheet.Cells[1, 2].Value = "Employee Name";
                        worksheet.Cells[1, 3].Value = "Designation";
                        worksheet.Cells[1, 4].Value = "Department";
                        worksheet.Cells[1, 5].Value = "Last Action Taken (Login Time)";
                        worksheet.Cells[1, 6].Value = "App Version";
                    }

                    // Auto-fit columns for better readability
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Convert the worksheet to a byte array
                    var excelData = package.GetAsByteArray();

                    // Return the byte array as a successful response
                    return new ServiceResponse<byte[]>(true, "Excel sheet generated successfully", excelData, 200);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response
                return new ServiceResponse<byte[]>(false, $"Error generating Excel sheet: {ex.Message}", null, 500);
            }
        }
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
                   emp.Gender,
                   login.UserName AS LoginId,
                   login.Password,
                   login.UserActivity AS LastActivity
            FROM tblLoginInformationMaster login
            INNER JOIN tbl_EmployeeProfileMaster emp ON login.UserId = emp.Employee_id
            LEFT JOIN tblDesignationMaster des ON emp.Designation_id = des.Designation_id
            LEFT JOIN tblDepartmentMaster dep ON emp.Department_id = dep.Department_id
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
            LEFT JOIN tblDesignationMaster des ON emp.Designation_id = des.Designation_id
            LEFT JOIN tblDepartmentMaster dep ON emp.Department_id = dep.Department_id
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

                // Set the response data including pagination details
                if (totalRecords > 0)
                {
                    return new ServiceResponse<List<EmployeeCredentialsResponse>>(true, "Employee credentials fetched successfully.", employeeCredentials.ToList(), 200, totalRecords);
                }
                else
                {
                    return new ServiceResponse<List<EmployeeCredentialsResponse>>(false, "no records", [], 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeCredentialsResponse>>(false, ex.Message, [], 500);
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
                   emp.Gender,
                   emp.MobileNo AS MobileNumber
            FROM tbl_EmployeeProfileMaster emp
            LEFT JOIN tblDesignationMaster des ON emp.Designation_id = des.Designation_id
            LEFT JOIN tblDepartmentMaster dep ON emp.Department_id = dep.Department_id
            LEFT JOIN tblLoginInformationMaster login ON login.UserId = emp.Employee_id
            WHERE login.UserId IS NULL  -- Non-app users (no login information)
            AND emp.InstituteId = @InstituteId
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
            WHERE login.UserId IS NULL  -- Non-app users (no login information)
            AND emp.InstituteId = @InstituteId
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
               emp.MobileNo AS MobileNumber,
               logs.LoginTime AS LastActionTaken,  -- Latest login action
               logs.version_sdkInt AS Version
        FROM LatestUserLogs logs
        INNER JOIN tbl_EmployeeProfileMaster emp ON logs.UserId = emp.Employee_id
        LEFT JOIN tblDesignationMaster des ON emp.Designation_id = des.Designation_id
        LEFT JOIN tblDepartmentMaster dep ON emp.Department_id = dep.Department_id
        WHERE emp.InstituteId = @InstituteId
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
        WHERE emp.InstituteId = @InstituteId
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

                if (totalRecords > 0)
                {
                    return new ServiceResponse<List<EmployeeActivityResponse>>(true, "Employee activity fetched successfully.", employeeActivity.ToList(), 200, totalRecords);
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
                // Ensure the connection is open
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }

                // Step 1: Check the login credentials
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
                SELECT TOP 1 [First_Name], [Last_Name], [Institute_id]
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
                            UserId = loginInfo.UserId
                        };
                    }
                }
                else if (loginInfo.UserType == 2) // Student
                {
                    var studentQuery = @"
                SELECT TOP 1 [First_Name], [Last_Name], [Institute_id]
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
                            UserId = loginInfo.UserId
                        };
                    }
                }
                else
                {
                    return new ServiceResponse<LoginResposne>(false, "Unknown user type.", new LoginResposne(), 500);
                }

                // Step 3: Log the login details in tblUserLogs
                var insertLogQuery = @"
            INSERT INTO tblUserLogs 
            ([UserId], [UserTypeId], [LoginTime], [IsAppUser], [brand], [device], [fingerprint], [model], [serialNumber], [type], [version_sdkInt], [version_securityPatch], [build_id], [isPhysicalDevice], [systemName], [systemVersion], [utsname_version], [operSysName], [browserName], [appName], [appVersion], [deviceMemory], [platform], [kernelVersion], [computerName], [systemGUID])
            VALUES (@UserId, @UserTypeId, @LoginTime, @IsAppUser, @Brand, @Device, @Fingerprint, @Model, @SerialNumber, @Type, @VersionSdkInt, @VersionSecurityPatch, @BuildId, @IsPhysicalDevice, @SystemName, @SystemVersion, @UtsnameVersion, @OperSysName, @BrowserName, @AppName, @AppVersion, @DeviceMemory, @Platform, @KernelVersion, @ComputerName, @SystemGUID)";

                var logParams = new
                {
                    UserId = loginInfo.UserId,
                    UserTypeId = loginInfo.UserType,
                    LoginTime = DateTime.Now,
                    IsAppUser = false, // You can update this based on your requirement
                    Brand = "Unknown", // Placeholder values
                    Device = "Unknown",
                    Fingerprint = "Unknown",
                    Model = "Unknown",
                    SerialNumber = "Unknown",
                    Type = "Unknown",
                    VersionSdkInt = "Unknown",
                    VersionSecurityPatch = "Unknown",
                    BuildId = "Unknown",
                    IsPhysicalDevice = false,
                    SystemName = "Unknown",
                    SystemVersion = "Unknown",
                    UtsnameVersion = "Unknown",
                    OperSysName = "Unknown",
                    BrowserName = "Unknown",
                    AppName = "Unknown",
                    AppVersion = "Unknown",
                    DeviceMemory = "Unknown",
                    Platform = "Unknown",
                    KernelVersion = "Unknown",
                    ComputerName = "Unknown",
                    SystemGUID = Guid.NewGuid().ToString() // Generate a unique system GUID
                };

                await _connection.ExecuteAsync(insertLogQuery, logParams);

                return new ServiceResponse<LoginResposne>(true, "Login successful.", response, 500);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return failure response

                return new ServiceResponse<LoginResposne>(false, ex.Message, new LoginResposne(), 500);
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
        public async Task<ServiceResponse<string>> UserLogout(string username)
        {
            try
            {
                // SQL query to fetch the user logs based on the username (assuming username is unique)
                string selectQuery = @"
        SELECT LogsId 
        FROM tblUserLogs 
        WHERE UserId = (SELECT App_User_id FROM tbl_StudentMaster WHERE Admission_Number = @username)
          AND LogoutTime IS NULL
        ORDER BY LoginTime DESC";

                // Fetch the most recent log entry where the user is currently logged in (LogoutTime is null)
                var logsId = await _connection.QueryFirstOrDefaultAsync<int?>(selectQuery, new { username });

                if (logsId == null)
                {
                    return new ServiceResponse<string>(false, "No active session found for this user", null, 404);
                }

                // SQL query to update the logout time for the fetched log entry
                string updateQuery = @"
        UPDATE tblUserLogs
        SET LogoutTime = @LogoutTime
        WHERE LogsId = @LogsId";

                // Execute the update query with the current timestamp
                await _connection.ExecuteAsync(updateQuery, new { LogoutTime = DateTime.Now, LogsId = logsId });

                return new ServiceResponse<string>(true, "User successfully logged out", null, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }
    }
}