using Employee_API.Repository.Interfaces;
using OfficeOpenXml;
using Employee_API.DTOs.ServiceResponse; 
using System.Data;
using Dapper;
using Employee_API.DTOs;

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
        public async Task<ServiceResponse<byte[]>> DownloadExcelSheet(int InstituteId)
        {
            try
            {
                // SQL query to fetch employee details, department, designation, and gender
                string sql = @"
            SELECT emp.Employee_id, emp.First_Name, emp.Middle_Name, emp.Last_Name, emp.Gender_id, 
                   emp.Department_id, emp.Designation_id, emp.mobile_number, emp.Date_of_Birth, emp.EmailID,
                   dep.DepartmentName, des.DesignationName, gen.Gender_Type
            FROM tbl_EmployeeProfileMaster emp
            LEFT JOIN tbl_Department dep ON emp.Department_id = dep.Department_id
            LEFT JOIN tbl_Designation des ON emp.Designation_id = des.Designation_id
            LEFT JOIN tbl_Gender gen ON emp.Gender_id = gen.Gender_id
            WHERE emp.Institute_id = @InstituteId AND emp.Status = 1";

                var employees = await _connection.QueryAsync<dynamic>(sql, new { InstituteId });

                // Initialize EPPlus package to create Excel
                using (var package = new ExcelPackage())
                {
                    // Add a worksheet
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
                        worksheet.Cells[row, 7].Value = employee.Date_of_Birth.ToString("yyyy-MM-dd");
                        worksheet.Cells[row, 8].Value = employee.EmailID;
                        row++;
                    }

                    // Auto-fit the columns to make the content visible
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

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
        public async Task<ServiceResponse<byte[]>> DownloadExcelSheetNonAppUsers(int InstituteId)
        {
            try
            {
                // SQL query to fetch non-app users (IsAppUser = false) based on InstituteId and UserTypeId for employees (UserTypeId = 1)
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
              AND emp.Status = 1";

                // Fetching non-app users from the database
                var nonAppUsers = await _connection.QueryAsync<dynamic>(sql, new { InstituteId });

                // Initialize the EPPlus Excel package
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

                    // Adjust column width for readability
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
        public async Task<ServiceResponse<byte[]>> DownloadExcelSheetEmployeeActivity(int InstituteId)
        {
            try
            {
                // SQL query to fetch employee activity logs and relevant employee details
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
              AND emp.Status = 1";  // Only active employees

                // Fetching employee activity data from the database
                var employeeActivityLogs = await _connection.QueryAsync<dynamic>(sql, new { InstituteId });

                // Initialize the EPPlus Excel package
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

                    // Adjust column width for readability
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
            SELECT emp.Employee_id AS EmployeeId,
                   emp.First_Name + ' ' + emp.Last_Name AS EmployeeName,
                   des.DesignationName AS Designation,
                   dep.DepartmentName AS Department,
                   emp.MobileNo AS MobileNumber,
                   MAX(logs.LoginTime) AS LastActionTaken,  -- Get the most recent login action
                   logs.version_sdkInt AS Version
            FROM tblUserLogs logs
            INNER JOIN tbl_EmployeeProfileMaster emp ON logs.UserId = emp.Employee_id
            LEFT JOIN tblDesignationMaster des ON emp.Designation_id = des.Designation_id
            LEFT JOIN tblDepartmentMaster dep ON emp.Department_id = dep.Department_id
            WHERE emp.InstituteId = @InstituteId
            AND (@DepartmentId = 0 OR emp.Department_id = @DepartmentId)
            AND (@DesignationId = 0 OR emp.Designation_id = @DesignationId)
            AND (emp.First_Name + ' ' + emp.Last_Name LIKE '%' + @SearchText + '%' OR @SearchText = '')
            AND logs.UserTypeId = 1  -- Assuming UserTypeId = 1 is for Employees
            GROUP BY emp.Employee_id, emp.First_Name, emp.Last_Name, des.DesignationName, dep.DepartmentName, emp.MobileNo, logs.version_sdkInt
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
                    return new ServiceResponse<List<EmployeeActivityResponse>>(false, "No records.", [], 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeActivityResponse>>(false, ex.Message, [], 500);
            }
        }
    }
}
