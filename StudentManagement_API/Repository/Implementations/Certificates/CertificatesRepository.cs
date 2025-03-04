using System.Data.SqlClient;
using Dapper;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using StudentManagement_API.DTOs.Responses;

namespace StudentManagement_API.Repository.Implementations
{
    public class CertificatesRepository : ICertificatesRepository
    {
        private readonly string _connectionString;

        public CertificatesRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateCertificateTemplateAsync(CreateCertificateTemplateRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                INSERT INTO tblCertificateTemplate
                (
                    TemplateName,
                    TemplateContent,
                    InstituteID,
                    UserID,
                    CreatedOn
                )
                VALUES
                (
                    @TemplateName,
                    @TemplateContent,
                    @InstituteID,
                    @UserID,
                    GETDATE()
                );
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            var parameters = new
            {
                request.TemplateName,
                request.TemplateContent,
                request.InstituteID,
                request.UserID
            };

            connection.Open();
            var id = await connection.QuerySingleAsync<int>(sql, parameters);
            return id;
        }

        //public async Task<IEnumerable<GetCertificateTemplateResponse>> GetCertificateTemplateAsync(GetCertificateTemplateRequest request)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    string sql = @"
        //        SELECT 
        //            ct.TemplateID, 
        //            ct.TemplateName, 
        //            CONCAT(ep.First_Name, ' ', ISNULL(ep.Middle_Name, ''), ' ', ep.Last_Name) AS UserName,
        //            FORMAT(ct.CreatedOn, 'dd-MM-yyyy ''at'' hh:mm tt') AS CreatedOn
        //        FROM tblCertificateTemplate ct
        //        LEFT JOIN tbl_EmployeeProfileMaster ep ON ct.UserID = ep.Employee_id
        //        WHERE ct.InstituteID = @InstituteID
        //          AND (@Search IS NULL OR @Search = '' OR ct.TemplateName LIKE '%' + @Search + '%')";

        //    connection.Open();
        //    var results = await connection.QueryAsync<GetCertificateTemplateResponse>(
        //        sql,
        //        new { InstituteID = request.InstituteID, Search = request.Search }
        //    );
        //    return results;
        //}

        public async Task<ServiceResponse<IEnumerable<GetCertificateTemplateResponse>>> GetCertificateTemplateAsync(GetCertificateTemplateRequest request)
        {
            using var connection = new SqlConnection(_connectionString);

            // 1) Build a COUNT query (for total records).
            string countSql = @"
                SELECT COUNT(*)
                FROM tblCertificateTemplate ct
                LEFT JOIN tbl_EmployeeProfileMaster ep ON ct.UserID = ep.Employee_id
                WHERE ct.InstituteID = @InstituteID
                  AND (@Search IS NULL OR @Search = '' OR ct.TemplateName LIKE '%' + @Search + '%')
            ";

            // 2) Build the paged query.
            string sql = @"
                SELECT 
                    ct.TemplateID, 
                    ct.TemplateName, 
                    CONCAT(ep.First_Name, ' ', ISNULL(ep.Middle_Name, ''), ' ', ep.Last_Name) AS UserName,
                    FORMAT(ct.CreatedOn, 'dd-MM-yyyy ''at'' hh:mm tt') AS CreatedOn
                FROM tblCertificateTemplate ct
                LEFT JOIN tbl_EmployeeProfileMaster ep ON ct.UserID = ep.Employee_id
                WHERE ct.InstituteID = @InstituteID
                  AND (@Search IS NULL OR @Search = '' OR ct.TemplateName LIKE '%' + @Search + '%')
                ORDER BY ct.TemplateID
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY
            ";

            // 3) Calculate the offset.
            //    Example logic to prevent negative or zero values:
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
            var offset = (pageNumber - 1) * pageSize;

            var parameters = new
            {
                InstituteID = request.InstituteID,
                Search = request.Search,
                Offset = offset,
                PageSize = pageSize
            };

            connection.Open();

            // 4) Get the total count of matching records.
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

            // 5) Get the paged results.
            var results = await connection.QueryAsync<GetCertificateTemplateResponse>(sql, parameters);

            // 6) Wrap it all in your existing ServiceResponse (assuming you return a ServiceResponse).
            return new ServiceResponse<IEnumerable<GetCertificateTemplateResponse>>(
                success: true,
                message: "Templates retrieved successfully.",
                data: results,
                statusCode: 200,
                totalCount: totalCount
            );
        }


        //public async Task<int> GenerateCertificateAsync(GenerateCertificateRequest request)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    string sql = @"
        //        INSERT INTO tblStudentCertificate
        //        (
        //            TemplateID,
        //            StudentID,
        //            InstituteID,
        //            CerficateContent,
        //            CertificateFile
        //        )
        //        VALUES
        //        (
        //            @TemplateID,
        //            @StudentID,
        //            @InstituteID,
        //            @CerficateContent,
        //            @CertificateFile
        //        );
        //        SELECT CAST(SCOPE_IDENTITY() AS int);";

        //    var parameters = new
        //    {
        //        request.TemplateID,
        //        request.StudentID,
        //        request.InstituteID,
        //        request.CerficateContent,
        //        request.CertificateFile
        //    };

        //    connection.Open();
        //    int certificateId = await connection.QuerySingleAsync<int>(sql, parameters);
        //    return certificateId;
        //}


        public async Task<List<int>> GenerateCertificatesAsync(GenerateCertificateRequest request)
        {
            var insertedIds = new List<int>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            string sql = @"
                INSERT INTO tblStudentCertificate
                (
                    TemplateID,
                    StudentID,
                    InstituteID,
                    CerficateContent,
                    CertificateFile,
                    CertificateDate
                )
                VALUES
                (
                    @TemplateID,
                    @StudentID,
                    @InstituteID,
                    @CerficateContent,
                    @CertificateFile,
                    GETDATE()
                );
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            foreach (var cert in request.Cerficates)
            {
                var parameters = new
                {
                    TemplateID = request.TemplateID,
                    StudentID = cert.StudentID,
                    InstituteID = request.InstituteID,
                    CerficateContent = cert.CerficateContent,
                    CertificateFile = cert.CertificateFile
                };

                int certificateId = await connection.QuerySingleAsync<int>(sql, parameters, transaction);
                insertedIds.Add(certificateId);
            }
            transaction.Commit();
            return insertedIds;
        }

        public async Task<ServiceResponse<IEnumerable<GetStudentsResponse>>> GetStudentsAsync(GetStudentsRequest request)
        { 
            using var connection = new SqlConnection(_connectionString);

            string countSql = @"
                SELECT COUNT(*)
                FROM tbl_StudentMaster s
                WHERE s.Institute_id = @InstituteID
                  AND s.AcademicYearCode = @AcademicYearCode
                  AND s.class_id = @ClassID
                  AND s.section_id = @SectionID
            ";


            string sql = @"
                SELECT 
                    s.student_id AS StudentID,
                    CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name, ''), ' ', s.Last_Name) AS StudentName,
                    s.Admission_Number AS AdmissionNumber,
                    c.class_name AS Class,
                    sec.section_name AS Section,
                    g.Gender_Type AS Gender,
                    FORMAT(s.Date_of_Birth, 'dd-MM-yyyy') AS DOB,
                    CASE 
                        WHEN cert.StudentID IS NOT NULL THEN 'Generated'
                        ELSE 'Pending'
                    END AS Status
                FROM tbl_StudentMaster s
                INNER JOIN tbl_Class c ON s.class_id = c.class_id
                INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
                INNER JOIN tbl_Gender g ON s.gender_id = g.Gender_id
                LEFT JOIN (
                    SELECT DISTINCT StudentID
                    FROM tblStudentCertificate
                    WHERE TemplateID = @TemplateID AND InstituteID = @InstituteID
                ) cert ON s.student_id = cert.StudentID
                WHERE s.Institute_id = @InstituteID
                  AND s.AcademicYearCode = @AcademicYearCode
                  AND s.class_id = @ClassID
                  AND s.section_id = @SectionID
                ORDER BY s.student_id
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            ";

            // 3) Calculate the offset.
            //    Example logic to prevent negative or zero values:
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
            var offset = (pageNumber - 1) * pageSize;

            var parameters = new
            {
                InstituteID = request.InstituteID,
                AcademicYearCode = request.AcademicYearCode,
                ClassID = request.ClassID,
                SectionID = request.SectionID,
                TemplateID = request.TemplateID,
                Offset = offset,
                PageSize = pageSize
            };

            connection.Open();

            // 4) Get the total count of matching records.
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

            // 5) Get the paged results.
            var results = await connection.QueryAsync<GetStudentsResponse>(sql, parameters);

            // 6) Wrap it all in your existing ServiceResponse (assuming you return a ServiceResponse).
            return new ServiceResponse<IEnumerable<GetStudentsResponse>>(
                success: true,
                message: "Students retrieved successfully.",
                data: results,
                statusCode: 200,
                totalCount: totalCount
            );
             
        }

        //public async Task<IEnumerable<GetStudentsResponse>> GetStudentsAsync(GetStudentsRequest request)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    string sql = @"
        //        SELECT 
        //            s.student_id AS StudentID,
        //            CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name, ''), ' ', s.Last_Name) AS StudentName,
        //            s.Admission_Number AS AdmissionNumber,
        //            c.class_name AS Class,
        //            sec.section_name AS Section,
        //            g.Gender_Type AS Gender,
        //            FORMAT(s.Date_of_Birth, 'dd-MM-yyyy') AS DOB,
        //            CASE 
        //                WHEN cert.StudentID IS NOT NULL THEN 'Generated'
        //                ELSE 'Pending'
        //            END AS Status
        //        FROM tbl_StudentMaster s
        //        INNER JOIN tbl_Class c ON s.class_id = c.class_id
        //        INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
        //        INNER JOIN tbl_Gender g ON s.gender_id = g.Gender_id
        //        LEFT JOIN (
        //            SELECT DISTINCT StudentID
        //            FROM tblStudentCertificate
        //            WHERE TemplateID = @TemplateID AND InstituteID = @InstituteID
        //        ) cert ON s.student_id = cert.StudentID
        //        WHERE s.Institute_id = @InstituteID
        //          AND s.AcademicYearCode = @AcademicYearCode
        //          AND s.class_id = @ClassID
        //          AND s.section_id = @SectionID";

        //    connection.Open();
        //    var results = await connection.QueryAsync<GetStudentsResponse>(
        //        sql, new
        //        {
        //            InstituteID = request.InstituteID,
        //            AcademicYearCode = request.AcademicYearCode,
        //            ClassID = request.ClassID,
        //            SectionID = request.SectionID,
        //            TemplateID = request.TemplateID
        //        }
        //    );
        //    return results;
        //}

        //public async Task<IEnumerable<GetCertificateReportResponse>> GetCertificateReportAsync(GetCertificateReportRequest request)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    string sql = @"
        //        SELECT 
        //            s.student_id AS StudentID,
        //            CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name, ''), ' ', s.Last_Name) AS StudentName,
        //            s.Admission_Number AS AdmissionNumber,
        //            c.class_name AS Class,
        //            sec.section_name AS Section,
        //            FORMAT(sc.CertificateDate, 'dd-MM-yyyy') AS DateOfGeneration
        //        FROM tblStudentCertificate sc
        //        INNER JOIN tbl_StudentMaster s ON sc.StudentID = s.student_id
        //        INNER JOIN tbl_Class c ON s.class_id = c.class_id
        //        INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
        //        WHERE sc.InstituteID = @InstituteID
        //          AND s.AcademicYearCode = @AcademicYearCode
        //          AND s.class_id = @ClassID
        //          AND s.section_id = @SectionID
        //          AND sc.TemplateID = @TemplateID
        //          AND (
        //                @Search IS NULL OR @Search = '' OR 
        //                CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name, ''), ' ', s.Last_Name) LIKE '%' + @Search + '%'
        //                OR s.Admission_Number LIKE '%' + @Search + '%'
        //          )";

        //    connection.Open();
        //    var results = await connection.QueryAsync<GetCertificateReportResponse>(
        //        sql,
        //        new
        //        {
        //            InstituteID = request.InstituteID,
        //            AcademicYearCode = request.AcademicYearCode,
        //            ClassID = request.ClassID,
        //            SectionID = request.SectionID,
        //            TemplateID = request.TemplateID,
        //            Search = request.Search
        //        }
        //    );
        //    return results;
        //}


        public async Task<ServiceResponse<IEnumerable<GetCertificateReportResponse>>> GetCertificateReportAsync(GetCertificateReportRequest request)
        {
            using var connection = new SqlConnection(_connectionString);

            // Ensure valid paging parameters (default to Page 1 and PageSize 10 if invalid)
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
            var offset = (pageNumber - 1) * pageSize;

            // Count query to get the total number of records matching the filters (ignoring paging)
            string countSql = @"
        SELECT COUNT(*)
        FROM tblStudentCertificate sc
        INNER JOIN tbl_StudentMaster s ON sc.StudentID = s.student_id
        INNER JOIN tbl_Class c ON s.class_id = c.class_id
        INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
        WHERE sc.InstituteID = @InstituteID
          AND s.AcademicYearCode = @AcademicYearCode
          AND s.class_id = @ClassID
          AND s.section_id = @SectionID
          AND sc.TemplateID = @TemplateID
          AND (
                @Search IS NULL OR @Search = '' OR 
                CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name, ''), ' ', s.Last_Name) LIKE '%' + @Search + '%'
                OR s.Admission_Number LIKE '%' + @Search + '%'
          )";

            // Main query with paging (an ORDER BY is required for OFFSET FETCH)
            string sql = @"
        SELECT 
            s.student_id AS StudentID,
            CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name, ''), ' ', s.Last_Name) AS StudentName,
            s.Admission_Number AS AdmissionNumber,
            c.class_name AS Class,
            sec.section_name AS Section,
            FORMAT(sc.CertificateDate, 'dd-MM-yyyy') AS DateOfGeneration
        FROM tblStudentCertificate sc
        INNER JOIN tbl_StudentMaster s ON sc.StudentID = s.student_id
        INNER JOIN tbl_Class c ON s.class_id = c.class_id
        INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
        WHERE sc.InstituteID = @InstituteID
          AND s.AcademicYearCode = @AcademicYearCode
          AND s.class_id = @ClassID
          AND s.section_id = @SectionID
          AND sc.TemplateID = @TemplateID
          AND (
                @Search IS NULL OR @Search = '' OR 
                CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name, ''), ' ', s.Last_Name) LIKE '%' + @Search + '%'
                OR s.Admission_Number LIKE '%' + @Search + '%'
          )
          ORDER BY s.student_id
          OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                InstituteID = request.InstituteID,
                AcademicYearCode = request.AcademicYearCode,
                ClassID = request.ClassID,
                SectionID = request.SectionID,
                TemplateID = request.TemplateID,
                Search = request.Search,
                Offset = offset,
                PageSize = pageSize
            };

            connection.Open();

            // Get the total count of records matching the filter criteria
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

            // Get the paged results
            var results = await connection.QueryAsync<GetCertificateReportResponse>(sql, parameters);

            // Return a ServiceResponse that contains the data and the total record count
            return new ServiceResponse<IEnumerable<GetCertificateReportResponse>>(
                success: true,
                message: "Certificate report retrieved successfully.",
                data: results,
                statusCode: 200,
                totalCount: totalCount
            );
        }


        public async Task<IEnumerable<GetCertificateReportExportResponse>> GetCertificateReportExportAsync(GetCertificateReportExportRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                SELECT  
                    CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name, ''), ' ', s.Last_Name) AS StudentName,
                    s.Admission_Number AS AdmissionNumber,
                    c.class_name AS Class,
                    sec.section_name AS Section,
                    FORMAT(sc.CertificateDate, 'dd-MM-yyyy') AS DateOfGeneration
                FROM tblStudentCertificate sc
                INNER JOIN tbl_StudentMaster s ON sc.StudentID = s.student_id
                INNER JOIN tbl_Class c ON s.class_id = c.class_id
                INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
                WHERE sc.InstituteID = @InstituteID
                  AND s.AcademicYearCode = @AcademicYearCode
                  AND s.class_id = @ClassID
                  AND s.section_id = @SectionID
                  AND sc.TemplateID = @TemplateID
                  AND (
                        @Search IS NULL OR @Search = '' OR 
                        CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name, ''), ' ', s.Last_Name) LIKE '%' + @Search + '%'
                        OR s.Admission_Number LIKE '%' + @Search + '%'
                  )";

            connection.Open();
            var results = await connection.QueryAsync<GetCertificateReportExportResponse>(
                sql,
                new
                {
                    InstituteID = request.InstituteID,
                    AcademicYearCode = request.AcademicYearCode,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID,
                    TemplateID = request.TemplateID,
                    Search = request.Search
                }
            );
            return results;
        }

        public async Task<IEnumerable<GetCertificateInstituteTagsResponse>> GetCertificateInstituteTagsAsync()
        {
            using var connection = new System.Data.SqlClient.SqlConnection(_connectionString);
            string sql = @"
                SELECT ColumnDisplayName, ColumnFieldName, Value
                FROM tblCertificatesInstitutes";

            connection.Open();
            return await connection.QueryAsync<GetCertificateInstituteTagsResponse>(sql);
        }


        public async Task<IEnumerable<CertificateStudentTagDto>> GetCertificateStudentTagsAsync()
        {
            const string sql = @"
                SELECT [Group],
                       ColumnDisplayName,
                       ColumnFieldName,
                       [Value]
                FROM tblCertificatesStudents
            ";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var result = await connection.QueryAsync<CertificateStudentTagDto>(sql);
            return result;
        }

    }
}
