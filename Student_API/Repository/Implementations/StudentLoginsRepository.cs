using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using System.Data;
using Dapper;
using OfficeOpenXml;

namespace Student_API.Repository.Implementations
{
    public class StudentLoginsRepository : IStudentLoginsRepository
    {
        private readonly IDbConnection _connection;

        public StudentLoginsRepository(IDbConnection connection)
        {
            _connection = connection;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        public async Task<ServiceResponse<byte[]>> DownloadExcelSheet(int InstituteId)
        {
            try
            {
                // SQL query to fetch student data
                string sqlQuery = @"
        SELECT stu.Admission_Number AS AdmissionNumber,
               stu.student_id AS StudentId,
               stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name AS Name,
               cls.ClassName AS Class,
               sec.SectionName AS Section,
               stu.LoginId,
               stu.Password,
               CASE WHEN stu.gender_id = 1 THEN 'Male' 
                    WHEN stu.gender_id = 2 THEN 'Female' 
                    ELSE 'Other' END AS Gender,
               logs.LastActionTaken,
               logs.AppVersion
        FROM tbl_StudentMaster stu
        LEFT JOIN tblClassMaster cls ON stu.class_id = cls.ClassId
        LEFT JOIN tblSectionMaster sec ON stu.section_id = sec.SectionId
        LEFT JOIN tblUserLogs logs ON stu.App_User_id = logs.UserId
        WHERE stu.Institute_id = @InstituteId";

                // Fetch the data from the database
                var studentData = await _connection.QueryAsync<StudentCredentialsResponse>(sqlQuery, new { InstituteId });

                // Create Excel package
                using (var package = new ExcelPackage())
                {
                    // Add a worksheet
                    var worksheet = package.Workbook.Worksheets.Add("StudentLoginData");

                    // Add headers to the Excel sheet
                    worksheet.Cells[1, 1].Value = "Admission Number";
                    worksheet.Cells[1, 2].Value = "Student ID";
                    worksheet.Cells[1, 3].Value = "Name";
                    worksheet.Cells[1, 4].Value = "Class";
                    worksheet.Cells[1, 5].Value = "Section";
                    worksheet.Cells[1, 6].Value = "Login ID";
                    worksheet.Cells[1, 7].Value = "Password";
                    worksheet.Cells[1, 8].Value = "Gender";
                    worksheet.Cells[1, 9].Value = "Last Action Taken";
                    worksheet.Cells[1, 10].Value = "App Version";

                    // Add student data to the Excel sheet
                    int row = 2;
                    foreach (var student in studentData)
                    {
                        worksheet.Cells[row, 1].Value = student.AdmissionNumber;
                        worksheet.Cells[row, 2].Value = student.StudentId;
                        worksheet.Cells[row, 3].Value = student.Name;
                        worksheet.Cells[row, 4].Value = student.Class;
                        worksheet.Cells[row, 5].Value = student.Section;
                        worksheet.Cells[row, 6].Value = student.LoginId;
                        worksheet.Cells[row, 7].Value = student.Password;
                        worksheet.Cells[row, 8].Value = student.Gender;
                        worksheet.Cells[row, 9].Value = student.LastActionTaken;
                        worksheet.Cells[row, 10].Value = student.AppVersion;
                        row++;
                    }

                    // Auto-fit columns for better readability
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Return the Excel file as a byte array
                    return new ServiceResponse<byte[]>(true, "Excel sheet generated successfully", package.GetAsByteArray(), 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<byte[]>> DownloadExcelSheetNonAppUsers(int InstituteId)
        {
            try
            {
                // SQL query to fetch non-app users data
                string sqlQuery = @"
        SELECT stu.Admission_Number AS AdmissionNumber,
               stu.student_id AS StudentId,
               stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name AS Name,
               cls.ClassName AS Class,
               sec.SectionName AS Section,
               stu.MobileNumber
        FROM tbl_StudentMaster stu
        LEFT JOIN tblClassMaster cls ON stu.class_id = cls.ClassId
        LEFT JOIN tblSectionMaster sec ON stu.section_id = sec.SectionId
        WHERE stu.Institute_id = @InstituteId
        AND stu.App_User_id IS NULL";  // Assumes non-app users have a NULL App_User_id

                // Fetch the data from the database
                var nonAppUsersData = await _connection.QueryAsync<StudentsNonAppUsersResponse>(sqlQuery, new { InstituteId });

                // Create Excel package
                using (var package = new ExcelPackage())
                {
                    // Add a worksheet
                    var worksheet = package.Workbook.Worksheets.Add("NonAppUsers");

                    // Add headers to the Excel sheet
                    worksheet.Cells[1, 1].Value = "Admission Number";
                    worksheet.Cells[1, 2].Value = "Student ID";
                    worksheet.Cells[1, 3].Value = "Name";
                    worksheet.Cells[1, 4].Value = "Class";
                    worksheet.Cells[1, 5].Value = "Section";
                    worksheet.Cells[1, 6].Value = "Mobile Number";

                    // Add student data to the Excel sheet
                    int row = 2;
                    foreach (var student in nonAppUsersData)
                    {
                        worksheet.Cells[row, 1].Value = student.AdmissionNumber;
                        worksheet.Cells[row, 2].Value = student.StudentId;
                        worksheet.Cells[row, 3].Value = student.Name;
                        worksheet.Cells[row, 4].Value = student.Class;
                        worksheet.Cells[row, 5].Value = student.Section;
                        worksheet.Cells[row, 6].Value = student.MobileNumber;
                        row++;
                    }

                    // Auto-fit columns for better readability
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Return the Excel file as a byte array
                    return new ServiceResponse<byte[]>(true, "Excel sheet generated successfully", package.GetAsByteArray(), 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<byte[]>> DownloadExcelSheetStudentActivity(int InstituteId)
        {
            try
            {
                // SQL query to fetch student activity data for the given InstituteId
                string sqlQuery = @"
        SELECT stu.Admission_Number AS AdmissionNumber,
               stu.student_id AS StudentId,
               stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name AS Name,
               cls.ClassName AS Class,
               sec.SectionName AS Section,
               stu.MobileNumber AS Mobile,
               logs.LastActionTaken,
               logs.AppVersion
        FROM tbl_StudentMaster stu
        LEFT JOIN tblClassMaster cls ON stu.class_id = cls.ClassId
        LEFT JOIN tblSectionMaster sec ON stu.section_id = sec.SectionId
        LEFT JOIN tblUserLogs logs ON stu.App_User_id = logs.UserId
        WHERE stu.Institute_id = @InstituteId";

                // Fetch the data from the database
                var studentActivityData = await _connection.QueryAsync<StudentActivityResponse>(sqlQuery, new { InstituteId });

                // Create Excel package
                using (var package = new ExcelPackage())
                {
                    // Add a worksheet
                    var worksheet = package.Workbook.Worksheets.Add("StudentActivity");

                    // Add headers to the Excel sheet
                    worksheet.Cells[1, 1].Value = "Admission Number";
                    worksheet.Cells[1, 2].Value = "Student ID";
                    worksheet.Cells[1, 3].Value = "Name";
                    worksheet.Cells[1, 4].Value = "Class";
                    worksheet.Cells[1, 5].Value = "Section";
                    worksheet.Cells[1, 6].Value = "Mobile";
                    worksheet.Cells[1, 7].Value = "Last Action Taken";
                    worksheet.Cells[1, 8].Value = "App Version";

                    // Add student activity data to the Excel sheet
                    int row = 2;
                    foreach (var student in studentActivityData)
                    {
                        worksheet.Cells[row, 1].Value = student.AdmissionNumber;
                        worksheet.Cells[row, 2].Value = student.StudentId;
                        worksheet.Cells[row, 3].Value = student.Name;
                        worksheet.Cells[row, 4].Value = student.Class;
                        worksheet.Cells[row, 5].Value = student.Section;
                        worksheet.Cells[row, 6].Value = student.Mobile;
                        worksheet.Cells[row, 7].Value = student.LastActionTaken;
                        worksheet.Cells[row, 8].Value = student.AppVersion;
                        row++;
                    }

                    // Auto-fit columns for better readability
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Return the Excel file as a byte array
                    return new ServiceResponse<byte[]>(true, "Excel sheet generated successfully", package.GetAsByteArray(), 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<StudentActivityResponse>>> GetAllStudentActivity(StudentLoginRequest request)
        {
            try
            {
                // Calculate the number of rows to skip based on the page number and page size
                int offset = (request.PageNumber - 1) * request.PageSize;

                // SQL query to fetch student activity with filters and pagination
                string sqlQuery = @"
        WITH LatestStudentLogs AS (
            SELECT logs.UserId,
                   logs.LoginTime,
                   logs.appVersion,
                   ROW_NUMBER() OVER (PARTITION BY logs.UserId ORDER BY logs.LoginTime DESC) AS RowNum
            FROM tblUserLogs logs
            WHERE logs.UserTypeId = 2  -- Assuming UserTypeId = 2 is for Students
        )
        SELECT stu.Admission_Number AS AdmissionNumber,
               stu.student_id AS StudentId,
               stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name AS Name,
               cls.ClassName AS Class,
               sec.SectionName AS Section,
               stu.Mobile AS Mobile,
               logs.LoginTime AS LastActionTaken,  -- Latest login action
               logs.appVersion AS AppVersion
        FROM LatestStudentLogs logs
        INNER JOIN tbl_StudentMaster stu ON logs.UserId = stu.student_id
        LEFT JOIN tblClassMaster cls ON stu.class_id = cls.ClassId
        LEFT JOIN tblSectionMaster sec ON stu.section_id = sec.SectionId
        WHERE stu.Institute_id = @InstituteId
        AND (@ClassId = 0 OR stu.class_id = @ClassId)
        AND (@SectionId = 0 OR stu.section_id = @SectionId)
        AND (stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name LIKE '%' + @SearchText + '%' OR @SearchText = '')
        AND logs.RowNum = 1  -- Get only the latest log per student
        ORDER BY stu.student_id
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";  // Pagination

                // SQL query to count total student records without pagination
                string countQuery = @"
        SELECT COUNT(DISTINCT stu.student_id)
        FROM tblUserLogs logs
        INNER JOIN tbl_StudentMaster stu ON logs.UserId = stu.student_id
        WHERE stu.Institute_id = @InstituteId
        AND (@ClassId = 0 OR stu.class_id = @ClassId)
        AND (@SectionId = 0 OR stu.section_id = @SectionId)
        AND (stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name LIKE '%' + @SearchText + '%' OR @SearchText = '')
        AND logs.UserTypeId = 2";  // Assuming UserTypeId = 2 is for Students

                // Execute the count query to get total records
                int totalRecords = await _connection.ExecuteScalarAsync<int>(countQuery, new
                {
                    InstituteId = request.InstituteId,
                    ClassId = request.ClassId,
                    SectionId = request.SectionId,
                    SearchText = request.SearchText
                });

                // Execute the main query with pagination
                var studentActivity = await _connection.QueryAsync<StudentActivityResponse>(sqlQuery, new
                {
                    InstituteId = request.InstituteId,
                    ClassId = request.ClassId,
                    SectionId = request.SectionId,
                    SearchText = request.SearchText,
                    Offset = offset,
                    PageSize = request.PageSize
                });

                // Return response based on the fetched data
                if (totalRecords > 0)
                {
                    return new ServiceResponse<List<StudentActivityResponse>>(true, "Student activity fetched successfully.", studentActivity.ToList(), 200, totalRecords);
                }
                else
                {
                    return new ServiceResponse<List<StudentActivityResponse>>(false, "No records found.", new List<StudentActivityResponse>(), 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentActivityResponse>>(false, ex.Message, new List<StudentActivityResponse>(), 500);
            }
        }
        public async Task<ServiceResponse<List<StudentCredentialsResponse>>> GetAllStudentLoginCred(StudentLoginRequest request)
        {
            try
            {
                // Calculate offset for pagination
                int offset = (request.PageNumber - 1) * request.PageSize;

                // SQL query to fetch student login credentials along with latest activity
                string sqlQuery = @"
        WITH LatestStudentLogs AS (
            SELECT logs.UserId,
                   logs.LoginTime,
                   logs.appVersion,
                   ROW_NUMBER() OVER (PARTITION BY logs.UserId ORDER BY logs.LoginTime DESC) AS RowNum
            FROM tblUserLogs logs
            WHERE logs.UserTypeId = 2  -- Assuming UserTypeId = 2 is for Students
        )
        SELECT stu.Admission_Number AS AdmissionNumber,
               stu.student_id AS StudentId,
               stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name AS Name,
               cls.ClassName AS Class,
               sec.SectionName AS Section,
               login.LoginId AS LoginId,
               login.Password AS Password,
               gen.GenderName AS Gender,
               logs.LoginTime AS LastActionTaken,  -- Latest login action
               logs.appVersion AS AppVersion
        FROM tbl_StudentMaster stu
        LEFT JOIN LatestStudentLogs logs ON stu.student_id = logs.UserId AND logs.RowNum = 1
        LEFT JOIN tblUserLogin login ON stu.App_User_id = login.UserId  -- Assuming App_User_id links to user login table
        LEFT JOIN tblClassMaster cls ON stu.class_id = cls.ClassId
        LEFT JOIN tblSectionMaster sec ON stu.section_id = sec.SectionId
        LEFT JOIN tblGenderMaster gen ON stu.gender_id = gen.GenderId
        WHERE stu.Institute_id = @InstituteId
        AND (@ClassId = 0 OR stu.class_id = @ClassId)
        AND (@SectionId = 0 OR stu.section_id = @SectionId)
        AND (stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name LIKE '%' + @SearchText + '%' OR @SearchText = '')
        ORDER BY stu.student_id
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";  // Pagination

                // SQL query to count total student records without pagination
                string countQuery = @"
        SELECT COUNT(DISTINCT stu.student_id)
        FROM tbl_StudentMaster stu
        LEFT JOIN tblUserLogin login ON stu.App_User_id = login.UserId
        WHERE stu.Institute_id = @InstituteId
        AND (@ClassId = 0 OR stu.class_id = @ClassId)
        AND (@SectionId = 0 OR stu.section_id = @SectionId)
        AND (stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name LIKE '%' + @SearchText + '%' OR @SearchText = '')";

                // Execute the count query to get total records
                int totalRecords = await _connection.ExecuteScalarAsync<int>(countQuery, new
                {
                    InstituteId = request.InstituteId,
                    ClassId = request.ClassId,
                    SectionId = request.SectionId,
                    SearchText = request.SearchText
                });

                // Execute the main query with pagination
                var studentCredentials = await _connection.QueryAsync<StudentCredentialsResponse>(sqlQuery, new
                {
                    InstituteId = request.InstituteId,
                    ClassId = request.ClassId,
                    SectionId = request.SectionId,
                    SearchText = request.SearchText,
                    Offset = offset,
                    PageSize = request.PageSize
                });

                // Return response based on the fetched data
                if (totalRecords > 0)
                {
                    return new ServiceResponse<List<StudentCredentialsResponse>>(true, "Student credentials fetched successfully.", studentCredentials.ToList(), 200, totalRecords);
                }
                else
                {
                    return new ServiceResponse<List<StudentCredentialsResponse>>(false, "No records found.", new List<StudentCredentialsResponse>(), 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentCredentialsResponse>>(false, ex.Message, new List<StudentCredentialsResponse>(), 500);
            }
        }
        public async Task<ServiceResponse<List<StudentsNonAppUsersResponse>>> GetAllStudentNonAppUsers(StudentLoginRequest request)
        {
            try
            {
                // Calculate offset for pagination
                int offset = (request.PageNumber - 1) * request.PageSize;

                // SQL query to fetch students who are non-app users
                string sqlQuery = @"
        SELECT stu.Admission_Number AS AdmissionNumber,
               stu.student_id AS StudentId,
               stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name AS Name,
               cls.ClassName AS Class,
               sec.SectionName AS Section,
               stu.MobileNumber AS MobileNumber
        FROM tbl_StudentMaster stu
        LEFT JOIN tblUserLogs logs ON stu.App_User_id = logs.UserId AND logs.IsAppUser = 0
        LEFT JOIN tblClassMaster cls ON stu.class_id = cls.ClassId
        LEFT JOIN tblSectionMaster sec ON stu.section_id = sec.SectionId
        WHERE stu.Institute_id = @InstituteId
        AND (@ClassId = 0 OR stu.class_id = @ClassId)
        AND (@SectionId = 0 OR stu.section_id = @SectionId)
        AND (stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name LIKE '%' + @SearchText + '%' OR @SearchText = '')
        AND (stu.App_User_id IS NULL OR logs.UserId IS NULL)  -- Non-app users
        ORDER BY stu.student_id
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";  // Pagination

                // SQL query to count total non-app user student records without pagination
                string countQuery = @"
        SELECT COUNT(DISTINCT stu.student_id)
        FROM tbl_StudentMaster stu
        LEFT JOIN tblUserLogs logs ON stu.App_User_id = logs.UserId AND logs.IsAppUser = 0
        WHERE stu.Institute_id = @InstituteId
        AND (@ClassId = 0 OR stu.class_id = @ClassId)
        AND (@SectionId = 0 OR stu.section_id = @SectionId)
        AND (stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name LIKE '%' + @SearchText + '%' OR @SearchText = '')
        AND (stu.App_User_id IS NULL OR logs.UserId IS NULL)";  // Non-app users

                // Execute the count query to get total records
                int totalRecords = await _connection.ExecuteScalarAsync<int>(countQuery, new
                {
                    InstituteId = request.InstituteId,
                    ClassId = request.ClassId,
                    SectionId = request.SectionId,
                    SearchText = request.SearchText
                });

                // Execute the main query with pagination
                var nonAppUsers = await _connection.QueryAsync<StudentsNonAppUsersResponse>(sqlQuery, new
                {
                    InstituteId = request.InstituteId,
                    ClassId = request.ClassId,
                    SectionId = request.SectionId,
                    SearchText = request.SearchText,
                    Offset = offset,
                    PageSize = request.PageSize
                });

                // Return response based on the fetched data
                if (totalRecords > 0)
                {
                    return new ServiceResponse<List<StudentsNonAppUsersResponse>>(true, "Non-app user students fetched successfully.", nonAppUsers.ToList(), 200, totalRecords);
                }
                else
                {
                    return new ServiceResponse<List<StudentsNonAppUsersResponse>>(false, "No non-app user records found.", new List<StudentsNonAppUsersResponse>(), 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentsNonAppUsersResponse>>(false, ex.Message, new List<StudentsNonAppUsersResponse>(), 500);
            }
        }
      
    }
}
