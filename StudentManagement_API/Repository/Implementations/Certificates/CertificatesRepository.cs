using System.Data.SqlClient;
using Dapper;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using StudentManagement_API.DTOs.Responses;
using System.Data.Common;

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


        //public async Task<List<int>> GenerateCertificatesAsync(GenerateCertificateRequest request)
        //{
        //    var insertedIds = new List<int>();

        //    using var connection = new SqlConnection(_connectionString);
        //    connection.Open();
        //    using var transaction = connection.BeginTransaction();

        //    // Step 1: Retrieve certificate template content from tblCertificateTemplate
        //    string getTemplateSql = @"
        //SELECT TemplateContent 
        //FROM tblCertificateTemplate 
        //WHERE TemplateID = @TemplateID 
        //  AND InstituteID = @InstituteID";
        //    string templateContent = await connection.QueryFirstOrDefaultAsync<string>(
        //        getTemplateSql,
        //        new { TemplateID = request.TemplateID, InstituteID = request.InstituteID },
        //        transaction);

        //    if (string.IsNullOrEmpty(templateContent))
        //    {
        //        throw new Exception("Certificate template not found.");
        //    }

        //    // SQL for inserting into tblStudentCertificate
        //    string insertSql = @"
        //INSERT INTO tblStudentCertificate
        //(
        //    TemplateID,
        //    StudentID,
        //    InstituteID,
        //    CerficateContent,
        //    CertificateFile,
        //    CertificateDate
        //)
        //VALUES
        //(
        //    @TemplateID,
        //    @StudentID,
        //    @InstituteID,
        //    @CerficateContent,
        //    @CertificateFile,
        //    GETDATE()
        //);
        //SELECT CAST(SCOPE_IDENTITY() AS int);";

        //    // Process each student ID in the request
        //    foreach (var studentId in request.StudentIDs)
        //    {
        //        // Step 2: Get student data for tag replacement
        //        var studentData = await GetStudentDataAsync(studentId, connection, transaction);

        //        // Step 3: Replace all tags in the template content with the corresponding student values
        //        string finalCertificateContent = templateContent;
        //        foreach (var kvp in studentData)
        //        {
        //            finalCertificateContent = finalCertificateContent.Replace(kvp.Key, kvp.Value);
        //        }

        //        // For this flow, we assume CertificateFile is not provided via the request.
        //        var parameters = new
        //        {
        //            TemplateID = request.TemplateID,
        //            StudentID = studentId,
        //            InstituteID = request.InstituteID,
        //            CerficateContent = finalCertificateContent,
        //            CertificateFile = "" // Set to empty or null if not used.
        //        };

        //        int certificateId = await connection.QuerySingleAsync<int>(insertSql, parameters, transaction);
        //        insertedIds.Add(certificateId);
        //    }

        //    transaction.Commit();
        //    return insertedIds;
        //}

        //public async Task<GenerateCertificateResponse> GenerateCertificatesAsync(GenerateCertificateRequest request)
        //{
        //    var certificateContents = new List<string>();

        //    using var connection = new SqlConnection(_connectionString);
        //    connection.Open();
        //    using var transaction = connection.BeginTransaction();

        //    // Step 1: Retrieve certificate template content from tblCertificateTemplate
        //    string getTemplateSql = @"
        //        SELECT TemplateContent 
        //        FROM tblCertificateTemplate 
        //        WHERE TemplateID = @TemplateID 
        //          AND InstituteID = @InstituteID";
        //    string templateContent = await connection.QueryFirstOrDefaultAsync<string>(
        //        getTemplateSql,
        //        new { TemplateID = request.TemplateID, InstituteID = request.InstituteID },
        //        transaction);

        //    if (string.IsNullOrEmpty(templateContent))
        //    {
        //        throw new Exception("Certificate template not found.");
        //    }

        //    // SQL for inserting into tblStudentCertificate
        //    string insertSql = @"
        //        INSERT INTO tblStudentCertificate
        //        (
        //            TemplateID,
        //            StudentID,
        //            InstituteID,
        //            CerficateContent,
        //            CertificateFile,
        //            CertificateDate
        //        )
        //        VALUES
        //        (
        //            @TemplateID,
        //            @StudentID,
        //            @InstituteID,
        //            @CerficateContent,
        //            @CertificateFile,
        //            GETDATE()
        //        );
        //        SELECT CAST(SCOPE_IDENTITY() AS int);";

        //    // Process each student ID in the request
        //    foreach (var studentId in request.StudentIDs)
        //    {
        //        // Step 2: Get student data for tag replacement
        //        var studentData = await GetStudentDataAsync(studentId, connection, transaction);

        //        // Step 3: Replace all tags in the template content with the corresponding student values
        //        string finalCertificateContent = templateContent;
        //        foreach (var kvp in studentData)
        //        {
        //            finalCertificateContent = finalCertificateContent.Replace(kvp.Key, kvp.Value);
        //        }

        //        // For this flow, we assume CertificateFile is not provided via the request.
        //        var parameters = new
        //        {
        //            TemplateID = request.TemplateID,
        //            StudentID = studentId,
        //            InstituteID = request.InstituteID,
        //            CerficateContent = finalCertificateContent,
        //            CertificateFile = "" // or null if you prefer
        //        };

        //        int certificateId = await connection.QuerySingleAsync<int>(insertSql, parameters, transaction);
        //        // Add the certificate content to our response list
        //        certificateContents.Add(finalCertificateContent);
        //    }

        //    transaction.Commit();
        //    //return new GenerateCertificateResponse { StudentCertificates = certificateContents };
        //}

        public async Task<GenerateCertificateResponse> GenerateCertificatesAsync(GenerateCertificateRequest request)
        {
            var certificateResponses = new List<StudentCertificateResponse>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            // Step 1: Retrieve certificate template content from tblCertificateTemplate
            string getTemplateSql = @"
                SELECT TemplateContent 
                FROM tblCertificateTemplate 
                WHERE TemplateID = @TemplateID 
                  AND InstituteID = @InstituteID";
            string templateContent = await connection.QueryFirstOrDefaultAsync<string>(
                getTemplateSql,
                new { TemplateID = request.TemplateID, InstituteID = request.InstituteID },
                transaction);

            if (string.IsNullOrEmpty(templateContent))
            {
                throw new Exception("Certificate template not found.");
            }

            // SQL for inserting into tblStudentCertificate
            string insertSql = @"
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

            // Process each student ID in the request
            foreach (var studentId in request.StudentIDs)
            {
                // Step 2: Get student data for tag replacement
                var studentData = await GetStudentDataAsync(studentId, connection, transaction);

                // Step 3: Replace all tags in the template content with the corresponding student values
                string finalCertificateContent = templateContent;
                foreach (var kvp in studentData)
                {
                    finalCertificateContent = finalCertificateContent.Replace(kvp.Key, kvp.Value);
                }

                // For this flow, we assume CertificateFile is not provided via the request.
                var parameters = new
                {
                    TemplateID = request.TemplateID,
                    StudentID = studentId,
                    InstituteID = request.InstituteID,
                    CerficateContent = finalCertificateContent,
                    CertificateFile = "" // or null if not used
                };

                int certificateId = await connection.QuerySingleAsync<int>(insertSql, parameters, transaction);

                // Add the generated certificate content to our response list
                certificateResponses.Add(new StudentCertificateResponse
                {
                    TemplateID = request.TemplateID,
                    StudentID = studentId,
                    Certificate = finalCertificateContent
                });
            }

            transaction.Commit();
            return new GenerateCertificateResponse { StudentCertificate = certificateResponses };
        }

        /// <summary>
        /// Retrieves student data and parent information and returns a dictionary mapping certificate tag placeholders to actual values.
        /// </summary>
        private async Task<Dictionary<string, string>> GetStudentDataAsync(int studentId, SqlConnection connection, SqlTransaction transaction)
        {
            // Query student master and other info (adjust columns as needed)
            string sql = @"
            SELECT 
                 sm.File_Name AS StudentImage,
                 sm.First_Name,
                 sm.Middle_Name,
                 sm.Last_Name,
                 sm.gender_id AS Gender,
                 c.class_name AS Class,
                 s.section_name AS Section,
                 sm.Admission_Number,
                 sm.Roll_Number,
                 CONVERT(varchar, sm.Date_of_Joining, 23) AS DateOfJoining,
                 sm.AcademicYearCode AS AcademicYear,
                 sm.Nationality_id AS Nationality,
                 sm.Religion_id AS Religion,
                 CONVERT(varchar, sm.Date_of_Birth, 23) AS DateOfBirth,
                 sm.Mother_Tongue_id AS MotherTongue,
                 sm.Caste_id AS Caste,
                 sm.Blood_Group_id AS BloodGroup,
                 sm.Aadhar_Number AS AadharNo,
                 sm.PEN,
                 sm.StudentType_id AS StudentType,
                 sm.Institute_house_id AS StudentHouse,
                 sm.QR_code AS SamagraID,
                 soi.email_id AS EmailID
            FROM tbl_StudentMaster sm
            LEFT JOIN tbl_StudentOtherInfo soi ON sm.student_id = soi.student_id
            LEFT JOIN tbl_Class c ON sm.class_id = c.class_id
            LEFT JOIN tbl_Section s ON sm.section_id = s.section_id
            WHERE sm.student_id = @StudentID";


            var student = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { StudentID = studentId }, transaction);
            if (student == null)
            {
                throw new Exception("Student not found.");
            }

            // Retrieve parent info – using separate queries for Father, Mother, and Guardian
            string fatherSql = @"
                SELECT 
                    First_Name AS FatherFirstName,
                    Middle_Name AS FatherMiddleName,
                    Last_Name AS FatherLastName,
                    Mobile_Number AS FatherPrimaryContactNo,
                    CONVERT(varchar, Date_of_Birth, 23) AS FatherDateOfBirth,
                    Aadhar_no AS FatherAadharNo,
                    Occupation AS FatherOccupation,
                    Email_id AS FatherEmailID,
                    Residential_Address AS FatherResidentialAddress
                FROM tbl_StudentParentsInfo
                WHERE student_id = @StudentID AND Parent_Type_id = 1";
            var father = await connection.QueryFirstOrDefaultAsync<dynamic>(fatherSql, new { StudentID = studentId }, transaction);

            string motherSql = @"
                SELECT 
                    First_Name AS MotherFirstName,
                    Middle_Name AS MotherMiddleName,
                    Last_Name AS MotherLastName,
                    Mobile_Number AS MotherPrimaryContactNo,
                    CONVERT(varchar, Date_of_Birth, 23) AS MotherDateOfBirth,
                    Aadhar_no AS MotherAadharNo,
                    Occupation AS MotherOccupation,
                    Email_id AS MotherEmailID,
                    Residential_Address AS MotherResidentialAddress
                FROM tbl_StudentParentsInfo
                WHERE student_id = @StudentID AND Parent_Type_id = 2";
            var mother = await connection.QueryFirstOrDefaultAsync<dynamic>(motherSql, new { StudentID = studentId }, transaction);

            string guardianSql = @"
                SELECT 
                    First_Name AS GuardianFirstName,
                    Middle_Name AS GuardianMiddleName,
                    Last_Name AS GuardianLastName,
                    Mobile_Number AS GuardianPrimaryContactNo,
                    CONVERT(varchar, Date_of_Birth, 23) AS GuardianDateOfBirth,
                    Aadhar_no AS GuardianAadharNo,
                    Occupation AS GuardianOccupation,
                    Email_id AS GuardianEmailID,
                    Residential_Address AS GuardianResidentialAddress
                FROM tbl_StudentParentsInfo
                WHERE student_id = @StudentID AND Parent_Type_id = 3";
            var guardian = await connection.QueryFirstOrDefaultAsync<dynamic>(guardianSql, new { StudentID = studentId }, transaction);

            // Build and return the dictionary mapping tags to values.
            var data = new Dictionary<string, string>
            {
                { "<%StudentImage%>", student.StudentImage ?? "" },
                { "<%FirstName%>", student.First_Name ?? "" },
                { "<%MiddleName%>", student.Middle_Name ?? "" },
                { "<%LastName%>", student.Last_Name ?? "" },
                { "<%Gender%>", student.Gender?.ToString() ?? "" },
                { "<%Class%>", student.Class?.ToString() ?? "" },
                { "<%Section%>", student.Section?.ToString() ?? "" },
                { "<%AdmissionNo%>", student.Admission_Number ?? "" },
                { "<%RollNo%>", student.Roll_Number ?? "" },
                { "<%DateOfJoining%>", student.DateOfJoining ?? "" },
                { "<%AcademicYear%>", student.AcademicYear ?? "" },
                { "<%Nationality%>", student.Nationality?.ToString() ?? "" },
                { "<%Religion%>", student.Religion?.ToString() ?? "" },
                { "<%DateOfBirth%>", student.DateOfBirth ?? "" },
                { "<%MotherTongue%>", student.MotherTongue?.ToString() ?? "" },
                { "<%Caste%>", student.Caste?.ToString() ?? "" },
                { "<%BloodGroup%>", student.BloodGroup?.ToString() ?? "" },
                { "<%AadharNo%>", student.AadharNo ?? "" },
                { "<%PEN%>", student.PEN ?? "" },
                { "<%StudentType%>", student.StudentType?.ToString() ?? "" },
                { "<%StudentHouse%>", student.StudentHouse?.ToString() ?? "" },
                { "<%SamagraID%>", student.SamagraID ?? "" },
                { "<%EmailID%>", student.EmailID ?? "" }
            };

            // Add Father's details
            if (father != null)
            {
                data.Add("<%FatherFirstName%>", father.FatherFirstName ?? "");
                data.Add("<%FatherMiddleName%>", father.FatherMiddleName ?? "");
                data.Add("<%FatherLastName%>", father.FatherLastName ?? "");
                data.Add("<%FatherPrimaryContactNo%>", father.FatherPrimaryContactNo ?? "");
                data.Add("<%FatherDateOfBirth%>", father.FatherDateOfBirth ?? "");
                data.Add("<%FatherAadharNo%>", father.FatherAadharNo ?? "");
                data.Add("<%FatherOccupation%>", father.FatherOccupation ?? "");
                data.Add("<%FatherEmailID%>", father.FatherEmailID ?? "");
                data.Add("<%FatherResidentialAddress%>", father.FatherResidentialAddress ?? "");
            }
            else
            {
                data.Add("<%FatherFirstName%>", "");
                data.Add("<%FatherMiddleName%>", "");
                data.Add("<%FatherLastName%>", "");
                data.Add("<%FatherPrimaryContactNo%>", "");
                data.Add("<%FatherDateOfBirth%>", "");
                data.Add("<%FatherAadharNo%>", "");
                data.Add("<%FatherOccupation%>", "");
                data.Add("<%FatherEmailID%>", "");
                data.Add("<%FatherResidentialAddress%>", "");
            }

            // Add Mother's details
            if (mother != null)
            {
                data.Add("<%MotherFirstName%>", mother.MotherFirstName ?? "");
                data.Add("<%MotherMiddleName%>", mother.MotherMiddleName ?? "");
                data.Add("<%MotherLastName%>", mother.MotherLastName ?? "");
                data.Add("<%MotherPrimaryContactNo%>", mother.MotherPrimaryContactNo ?? "");
                data.Add("<%MotherDateOfBirth%>", mother.MotherDateOfBirth ?? "");
                data.Add("<%MotherAadharNo%>", mother.MotherAadharNo ?? "");
                data.Add("<%MotherOccupation%>", mother.MotherOccupation ?? "");
                data.Add("<%MotherEmailID%>", mother.MotherEmailID ?? "");
                data.Add("<%MotherResidentialAddress%>", mother.MotherResidentialAddress ?? "");
            }
            else
            {
                data.Add("<%MotherFirstName%>", "");
                data.Add("<%MotherMiddleName%>", "");
                data.Add("<%MotherLastName%>", "");
                data.Add("<%MotherPrimaryContactNo%>", "");
                data.Add("<%MotherDateOfBirth%>", "");
                data.Add("<%MotherAadharNo%>", "");
                data.Add("<%MotherOccupation%>", "");
                data.Add("<%MotherEmailID%>", "");
                data.Add("<%MotherResidentialAddress%>", "");
            }

            // Add Guardian's details
            if (guardian != null)
            {
                data.Add("<%GuardianFirstName%>", guardian.GuardianFirstName ?? "");
                data.Add("<%GuardianMiddleName%>", guardian.GuardianMiddleName ?? "");
                data.Add("<%GuardianLastName%>", guardian.GuardianLastName ?? "");
                data.Add("<%GuardianPrimaryContactNo%>", guardian.GuardianPrimaryContactNo ?? "");
                data.Add("<%GuardianDateOfBirth%>", guardian.GuardianDateOfBirth ?? "");
                data.Add("<%GuardianAadharNo%>", guardian.GuardianAadharNo ?? "");
                data.Add("<%GuardianOccupation%>", guardian.GuardianOccupation ?? "");
                data.Add("<%GuardianEmailID%>", guardian.GuardianEmailID ?? "");
                data.Add("<%GuardianResidentialAddress%>", guardian.GuardianResidentialAddress ?? "");
            }
            else
            {
                data.Add("<%GuardianFirstName%>", "");
                data.Add("<%GuardianMiddleName%>", "");
                data.Add("<%GuardianLastName%>", "");
                data.Add("<%GuardianPrimaryContactNo%>", "");
                data.Add("<%GuardianDateOfBirth%>", "");
                data.Add("<%GuardianAadharNo%>", "");
                data.Add("<%GuardianOccupation%>", "");
                data.Add("<%GuardianEmailID%>", "");
                data.Add("<%GuardianResidentialAddress%>", "");
            }

            return data;
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


        public async Task<ServiceResponse<GetCertificateTagValueResponse>> GetCertificateTagValue(GetCertificateTagValueRequest request)
        {
            try
            {
                string sql = @"
            SELECT 
                il.InstituteLogo,
                d.Institute_name AS InstituteName,
                CONCAT(a.house, ', ', a.Locality, ', ', a.Landmark, ', ', a.pincode) AS InstituteAdress,
                YEAR(d.en_date) AS AcademicYear,
                CONCAT(a.Mobile_number, ', ', a.Email) AS InstituteContact,
                STRING_AGG(acc.Accreditation_Number, ', ') AS AccreditationNumber,
                aff.AffiliationNumber,
                aff.AffiliationBoardLogo,
                aff.AffiliationBoardName,
                aff.InstituteCode
            FROM tbl_InstituteDetails d
                LEFT JOIN tbl_InstituteAddress a ON d.Institute_id = a.Institute_id
                LEFT JOIN tbl_InstituteLogo il ON d.Institute_id = il.InstituteId
                LEFT JOIN tbl_AffiliationInfo aff ON d.Institute_id = aff.Institute_id
                LEFT JOIN tbl_Accreditation acc ON aff.Affiliation_info_id = acc.Affiliation_id
            WHERE d.Institute_id = @InstituteID
            GROUP BY 
                il.InstituteLogo,
                d.Institute_name,
                a.house, a.Locality, a.Landmark, a.pincode,
                d.en_date,
                a.Mobile_number, a.Email,
                aff.AffiliationNumber, 
                aff.AffiliationBoardLogo, 
                aff.AffiliationBoardName, 
                aff.InstituteCode;";

                // Create a new connection using the connection string
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { InstituteID = request.InstituteID });

                if (result == null)
                {
                    return new ServiceResponse<GetCertificateTagValueResponse>(false, "Institute not found", null, 404);
                }

                string tagValue = string.Empty;
                string tag = request.Tag.Trim();

                // Match the tag value to the corresponding column
                switch (tag)
                {
                    case "<%InstituteLogo%>":
                        tagValue = result.InstituteLogo;
                        break;
                    case "<%InstituteName%>":
                        tagValue = result.InstituteName;
                        break;
                    case "<%InstituteAdress%>":
                        tagValue = result.InstituteAdress;
                        break;
                    case "<%AcademicYear%>":
                        tagValue = result.AcademicYear.ToString();
                        break;
                    case "<%InstituteContact%>":
                        tagValue = result.InstituteContact;
                        break;
                    case "<%AccrediationNumber%>": // Note: Typo as provided. Adjust if needed.
                        tagValue = result.AccreditationNumber;
                        break;
                    case "<%AffiliationNumber%>":
                        tagValue = result.AffiliationNumber;
                        break;
                    case "<%AffiliationBoardLogo%>":
                        tagValue = result.AffiliationBoardLogo;
                        break;
                    case "<%AffiliationBoardName%>":
                        tagValue = result.AffiliationBoardName;
                        break;
                    case "<%InstituteCode%>":
                        tagValue = result.InstituteCode;
                        break;
                    default:
                        return new ServiceResponse<GetCertificateTagValueResponse>(false, "Invalid tag", null, 400);
                }

                var response = new GetCertificateTagValueResponse
                {
                    TagValue = tagValue
                };

                return new ServiceResponse<GetCertificateTagValueResponse>(true, "Tag value retrieved successfully", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetCertificateTagValueResponse>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<int>> AttachCertificatewithStudent(AttachCertificatewithStudentRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // Update the CertificateFile and update the CertificateDate to current date.
            string sql = @"
            UPDATE tblStudentCertificate
            SET CertificateFile = @Certificate, 
                CertificateDate = GETDATE()
            WHERE TemplateID = @TemplateID
              AND StudentID = @StudentID
              AND InstituteID = @InstituteID;
            SELECT @@ROWCOUNT;";

            int rowsAffected = await connection.QuerySingleAsync<int>(sql, new
            {
                TemplateID = request.TemplateID,
                StudentID = request.StudentID,
                InstituteID = request.InstituteID,
                Certificate = request.Certificate
            });

            if (rowsAffected > 0)
            {
                return new ServiceResponse<int>(true, "Certificate attached successfully", rowsAffected, 200);
            }
            else
            {
                return new ServiceResponse<int>(false, "Certificate record not found or update failed", 0, 404);
            }
        }


    }
}
