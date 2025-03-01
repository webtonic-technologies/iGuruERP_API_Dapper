using Dapper;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System.Text;

namespace StudentManagement_API.Repository.Implementations
{
    public class StudentLoginsRepository : IStudentLoginsRepository
    {
        private readonly string _connectionString;

        public StudentLoginsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ServiceResponse<List<GetLoginCredentialsResponse>>> GetLoginCredentialsAsync(GetLoginCredentialsRequest request)
        {
            try
            {
                string sqlQuery = @"
WITH LatestStudentLogs AS (
    SELECT 
        logs.UserId,
        logs.LoginTime,
        logs.appVersion,
        ROW_NUMBER() OVER (PARTITION BY logs.UserId ORDER BY logs.LoginTime DESC) AS RowNum
    FROM tblUserLogs logs
    WHERE logs.UserTypeId = 2  -- For Students
)
SELECT 
    stu.student_id AS StudentID,
    stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name AS StudentName,
    stu.Admission_Number AS AdmissionNumber,
    cls.class_name AS Class,
    sec.section_name AS Section,
    login.UserName AS LoginID,
    login.Password AS Password,
    gen.Gender_Type AS Gender,
    logs.LoginTime AS LastActionTaken,
    logs.appVersion AS AppVersion
FROM tbl_StudentMaster stu
LEFT JOIN LatestStudentLogs logs ON stu.student_id = logs.UserId AND logs.RowNum = 1
LEFT JOIN tbl_Class cls ON stu.class_id = cls.class_id
LEFT JOIN tbl_Section sec ON stu.section_id = sec.section_id
LEFT JOIN tbl_Gender gen ON stu.gender_id = gen.Gender_id
LEFT JOIN tblLoginInformationMaster login ON stu.student_id = login.UserId AND login.UserType = 2
WHERE stu.Institute_id = @InstituteID
  AND stu.class_id = @ClassID
  AND stu.section_id = @SectionID
  AND (@Search IS NULL OR @Search = '' OR 
       ( (stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name) LIKE '%' + @Search + '%' 
         OR stu.Admission_Number LIKE '%' + @Search + '%' )
  );
";

                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var credentials = await connection.QueryAsync<GetLoginCredentialsResponse>(sqlQuery, new
                    {
                        InstituteID = request.InstituteID,
                        ClassID = request.ClassID,
                        SectionID = request.SectionID,
                        Search = request.Search
                    });

                    var credentialList = credentials.ToList();

                    if (credentialList.Any())
                    {
                        return new ServiceResponse<List<GetLoginCredentialsResponse>>(
                            true,
                            "Student login credentials retrieved successfully.",
                            credentialList,
                            200,
                            credentialList.Count);
                    }
                    else
                    {
                        return new ServiceResponse<List<GetLoginCredentialsResponse>>(
                            false,
                            "No records found.",
                            new List<GetLoginCredentialsResponse>(),
                            404);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetLoginCredentialsResponse>>(
                    false,
                    ex.Message,
                    new List<GetLoginCredentialsResponse>(),
                    500);
            }
        }

        public async Task<ServiceResponse<string>> GetLoginCredentialsExportAsync(GetLoginCredentialsExportRequest request)
        {
            try
            {
                string sqlQuery = @"
WITH LatestStudentLogs AS (
    SELECT 
        logs.UserId,
        logs.LoginTime,
        logs.appVersion,
        ROW_NUMBER() OVER (PARTITION BY logs.UserId ORDER BY logs.LoginTime DESC) AS RowNum
    FROM tblUserLogs logs
    WHERE logs.UserTypeId = 2  -- For Students
)
SELECT  
    stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name AS StudentName,
    stu.Admission_Number AS AdmissionNumber,
    cls.class_name AS Class,
    sec.section_name AS Section,
    login.UserName AS LoginID,
    login.Password AS Password,
    gen.Gender_Type AS Gender,
    logs.LoginTime AS LastActionTaken,
    logs.appVersion AS AppVersion
FROM tbl_StudentMaster stu
LEFT JOIN LatestStudentLogs logs ON stu.student_id = logs.UserId AND logs.RowNum = 1
LEFT JOIN tbl_Class cls ON stu.class_id = cls.class_id
LEFT JOIN tbl_Section sec ON stu.section_id = sec.section_id
LEFT JOIN tbl_Gender gen ON stu.gender_id = gen.Gender_id
LEFT JOIN tblLoginInformationMaster login ON stu.student_id = login.UserId AND login.UserType = 2
WHERE stu.Institute_id = @InstituteID
  AND stu.class_id = @ClassID
  AND stu.section_id = @SectionID
  AND (@Search IS NULL OR @Search = '' OR 
       ((stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name) LIKE '%' + @Search + '%' 
         OR stu.Admission_Number LIKE '%' + @Search + '%')
  );
";

                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var data = await connection.QueryAsync<GetLoginCredentialsExportResponse>(sqlQuery, new
                    {
                        InstituteID = request.InstituteID,
                        ClassID = request.ClassID,
                        SectionID = request.SectionID,
                        Search = request.Search
                    });
                    var list = data.ToList();
                    if (!list.Any())
                    {
                        return new ServiceResponse<string>(false, "No records found", null, 404);
                    }

                    string filePath = string.Empty;
                    if (request.ExportType == 1)
                    {
                        // Export to Excel using EPPlus
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage())
                        {
                            var worksheet = package.Workbook.Worksheets.Add("LoginCredentialsExport");
                            worksheet.Cells["A1"].LoadFromCollection(list, true);
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                            filePath = Path.Combine(Directory.GetCurrentDirectory(), "LoginCredentialsExport.xlsx");
                            File.WriteAllBytes(filePath, package.GetAsByteArray());
                        }
                    }
                    else if (request.ExportType == 2)
                    {
                        // Export to CSV
                        var sb = new StringBuilder();
                        sb.AppendLine("StudentID,StudentName,AdmissionNumber,Class,Section,LoginID,Password,Gender,LastActionTaken,AppVersion");
                        foreach (var item in list)
                        {
                            string lastAction = item.LastActionTaken.HasValue
                                ? item.LastActionTaken.Value.ToString("yyyy-MM-ddTHH:mm:ss")
                                : "";
                            sb.AppendLine($"{item.StudentName},{item.AdmissionNumber},{item.Class},{item.Section},{item.LoginID},{item.Password},{item.Gender},{lastAction},{item.AppVersion}");
                        }
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "LoginCredentialsExport.csv");
                        File.WriteAllText(filePath, sb.ToString());
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Invalid ExportType", null, 400);
                    }

                    return new ServiceResponse<string>(true, "Export file generated", filePath, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<List<GetNonAppUsersResponse>>> GetNonAppUsersAsync(GetNonAppUsersRequest request)
        {
            try
            {
                string sqlQuery = @"
SELECT 
    stu.student_id AS StudentID,
    stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name AS StudentName,
    stu.Admission_Number AS AdmissionNumber,
    cls.class_name AS Class,
    sec.section_name AS Section,
    spi.Mobile_Number AS PrimaryMobileNo,
    login.UserName AS LoginID,
    login.Password AS Password
FROM tbl_StudentMaster stu
LEFT JOIN tbl_Class cls ON stu.class_id = cls.class_id
LEFT JOIN tbl_Section sec ON stu.section_id = sec.section_id
LEFT JOIN tbl_StudentParentsInfo spi 
    ON stu.student_id = spi.Student_id AND spi.Parent_Type_id = 1
LEFT JOIN tblLoginInformationMaster login 
    ON stu.student_id = login.UserId AND login.UserType = 2
WHERE stu.Institute_id = @InstituteID
  AND stu.class_id = @ClassID
  AND stu.section_id = @SectionID
  AND NOT EXISTS (
      SELECT 1 
      FROM tblUserLogs l
      WHERE l.UserId = stu.student_id AND l.IsAppUser = 1
  )
  AND (
       @Search IS NULL OR @Search = '' OR
       (stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name) LIKE '%' + @Search + '%'
       OR stu.Admission_Number LIKE '%' + @Search + '%'
  );
";

                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var result = await connection.QueryAsync<GetNonAppUsersResponse>(sqlQuery, new
                    {
                        InstituteID = request.InstituteID,
                        ClassID = request.ClassID,
                        SectionID = request.SectionID,
                        Search = request.Search
                    });
                    var list = result.ToList();

                    if (list.Any())
                    {
                        return new ServiceResponse<List<GetNonAppUsersResponse>>(
                            true,
                            "Non-app users retrieved successfully.",
                            list,
                            200,
                            list.Count);
                    }
                    else
                    {
                        return new ServiceResponse<List<GetNonAppUsersResponse>>(
                            false,
                            "No records found.",
                            new List<GetNonAppUsersResponse>(),
                            404);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetNonAppUsersResponse>>(
                    false,
                    ex.Message,
                    new List<GetNonAppUsersResponse>(),
                    500);
            }
        }


        public async Task<ServiceResponse<string>> GetNonAppUsersExportAsync(GetNonAppUsersExportRequest request)
        {
            try
            {
                string sqlQuery = @"
SELECT 
    stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name AS StudentName,
    stu.Admission_Number AS AdmissionNumber,
    cls.class_name AS Class,
    sec.section_name AS Section,
    spi.Mobile_Number AS PrimaryMobileNo,
    login.UserName AS LoginID,
    login.Password AS Password
FROM tbl_StudentMaster stu
LEFT JOIN tbl_Class cls ON stu.class_id = cls.class_id
LEFT JOIN tbl_Section sec ON stu.section_id = sec.section_id
LEFT JOIN tbl_StudentParentsInfo spi 
    ON stu.student_id = spi.Student_id AND spi.Parent_Type_id = 1
LEFT JOIN tblLoginInformationMaster login 
    ON stu.student_id = login.UserId AND login.UserType = 2
WHERE stu.Institute_id = @InstituteID
  AND stu.class_id = @ClassID
  AND stu.section_id = @SectionID
  AND NOT EXISTS (
      SELECT 1 
      FROM tblUserLogs l
      WHERE l.UserId = stu.student_id AND l.IsAppUser = 1
  )
  AND (
       @Search IS NULL OR @Search = '' OR
       (stu.First_Name + ' ' + ISNULL(stu.Middle_Name, '') + ' ' + stu.Last_Name) LIKE '%' + @Search + '%'
       OR stu.Admission_Number LIKE '%' + @Search + '%'
  );
";

                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var data = await connection.QueryAsync<GetNonAppUsersExportResponse>(sqlQuery, new
                    {
                        InstituteID = request.InstituteID,
                        ClassID = request.ClassID,
                        SectionID = request.SectionID,
                        Search = request.Search
                    });
                    var list = data.ToList();
                    if (!list.Any())
                    {
                        return new ServiceResponse<string>(false, "No records found", null, 404);
                    }

                    string filePath = string.Empty;
                    if (request.ExportType == 1)
                    {
                        // Export to Excel using EPPlus
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage())
                        {
                            var worksheet = package.Workbook.Worksheets.Add("NonAppUsersExport");
                            worksheet.Cells["A1"].LoadFromCollection(list, true);
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                            filePath = Path.Combine(Directory.GetCurrentDirectory(), "NonAppUsersExport.xlsx");
                            File.WriteAllBytes(filePath, package.GetAsByteArray());
                        }
                    }
                    else if (request.ExportType == 2)
                    {
                        // Export to CSV
                        var sb = new StringBuilder();
                        sb.AppendLine("StudentName,AdmissionNumber,Class,Section,PrimaryMobileNo,LoginID,Password");
                        foreach (var item in list)
                        {
                            sb.AppendLine($"{item.StudentName},{item.AdmissionNumber},{item.Class},{item.Section},{item.PrimaryMobileNo},{item.LoginID},{item.Password}");
                        }
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "NonAppUsersExport.csv");
                        File.WriteAllText(filePath, sb.ToString());
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Invalid ExportType", null, 400);
                    }

                    return new ServiceResponse<string>(true, "Export file generated", filePath, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }
    }
}
