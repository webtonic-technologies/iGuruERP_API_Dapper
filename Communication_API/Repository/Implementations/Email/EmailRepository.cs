using Communication_API.DTOs.Requests.Email;
using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Responses.Email;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Email;
using Communication_API.Repository.Interfaces.Email;
using Dapper;
using OfficeOpenXml;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Communication_API.Repository.Implementations.Email
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IDbConnection _connection;

        public EmailRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> ConfigureEmail(ConfigureEmailRequest request)
        {
            var query = "INSERT INTO [tblEmailConfiguration] (SMTPUserName, SMTPPassword, SMTPServer, SMTPPort, Security) VALUES (@SMTPUserName, @SMTPPassword, @SMTPServer, @SMTPPort, @Security)";

            var parameters = new
            {
                request.SMTPUserName,
                request.SMTPPassword,
                request.SMTPServer,
                request.SMTPPort,
                request.Security
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 200 : 400);
        }

        public async Task<ServiceResponse<string>> SendNewEmail(SendNewEmailRequest request)
        {
            // Validate that userTypeID and the IDs provided match
            if (request.UserTypeID == 1 && (request.StudentIDs == null || request.StudentIDs.Count == 0))
            {
                return new ServiceResponse<string>(false, "StudentIDs are required when userTypeID is 1", null, 400);
            }
            if (request.UserTypeID == 2 && (request.EmployeeIDs == null || request.EmployeeIDs.Count == 0))
            {
                return new ServiceResponse<string>(false, "EmployeeIDs are required when userTypeID is 2", null, 400);
            }

            // Further validate that userTypeID is not mismatched
            if (request.UserTypeID == 1 && request.EmployeeIDs != null)
            {
                return new ServiceResponse<string>(false, "userTypeID 1 cannot be used with EmployeeIDs", null, 400);
            }
            if (request.UserTypeID == 2 && request.StudentIDs != null)
            {
                return new ServiceResponse<string>(false, "userTypeID 2 cannot be used with StudentIDs", null, 400);
            }

            // Parse ScheduleDate and ScheduleTime from string to DateTime
            DateTime? scheduleDate = null;
            DateTime? scheduleTime = null;

            // Parse ScheduleDate if provided
            if (!string.IsNullOrEmpty(request.ScheduleDate))
            {
                scheduleDate = DateTime.ParseExact(request.ScheduleDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            // Parse ScheduleTime if provided
            if (!string.IsNullOrEmpty(request.ScheduleTime))
            {
                scheduleTime = DateTime.ParseExact(request.ScheduleTime, "hh:mm tt", CultureInfo.InvariantCulture);
            }

            // Insert or update the Email
            var query = @"INSERT INTO [tblEmailMaster] (EmailSubject, EmailBody, UserTypeID, GroupID, Status, ScheduleNow, ScheduleDate, ScheduleTime, AcademicYearCode, InstituteID, SentBy) 
              VALUES (@EmailSubject, @EmailBody, @UserTypeID, @GroupID, @Status, @ScheduleNow, @ScheduleDate, @ScheduleTime, @AcademicYearCode, @InstituteID, @SentBy);
              SELECT CAST(SCOPE_IDENTITY() as int);";

            // Execute the query and get the EmailSendID
            var emailSendID = await _connection.ExecuteScalarAsync<int>(query, new
            {
                request.EmailSubject,
                request.EmailBody,
                request.UserTypeID,
                request.GroupID,
                request.Status,
                request.ScheduleNow,
                ScheduleDate = scheduleDate,
                ScheduleTime = scheduleTime,
                request.AcademicYearCode,
                request.InstituteID,
                request.SentBy
            });

            // Proceed with student or employee mappings
            if (emailSendID > 0)
            {
                // Handle student mapping
                if (request.UserTypeID == 1)
                {
                    // Insert into tblEmailStudentMapping
                    string insertStudentMappingSql = "INSERT INTO tblEmailStudentMapping (EmailSendID, StudentID) VALUES (@EmailSendID, @StudentID)";
                    foreach (var studentID in request.StudentIDs)
                    {
                        await _connection.ExecuteAsync(insertStudentMappingSql, new { EmailSendID = emailSendID, StudentID = studentID });
                    }
                }
                // Handle employee mapping
                else if (request.UserTypeID == 2)
                {
                    // Insert into tblEmailEmployeeMapping
                    string insertEmployeeMappingSql = "INSERT INTO tblEmailEmployeeMapping (EmailSendID, EmployeeID) VALUES (@EmailSendID, @EmployeeID)";
                    foreach (var employeeID in request.EmployeeIDs)
                    {
                        await _connection.ExecuteAsync(insertEmployeeMappingSql, new { EmailSendID = emailSendID, EmployeeID = employeeID });
                    }
                }

                return new ServiceResponse<string>(true, "Email added successfully", "Email added/updated successfully", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating email", 400);
            }
        }

        public async Task<ServiceResponse<List<EmailReportResponse>>> GetEmailReports(GetEmailReportsRequest request)
        {
            string sql = string.Empty;

            // Query for students
            if (request.UserTypeID == 1)
            {
                sql = @"
        SELECT 
            s.student_id AS StudentID,
            s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name AS StudentName,
            CONCAT(c.class_name, '-', sec.section_name) AS ClassSection,
            em.ScheduleDate AS [DateTime],
            em.EmailSubject,
            em.Status
        FROM tblEmailMaster em
        INNER JOIN tblEmailStudentMapping esm ON em.EmailSendID = esm.EmailSendID
        INNER JOIN tbl_StudentMaster s ON esm.StudentID = s.student_id
        INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.GroupID = em.GroupID
        INNER JOIN tbl_Class c ON gcsm.ClassID = c.class_id
        INNER JOIN tbl_Section sec ON gcsm.SectionID = sec.section_id
        WHERE em.ScheduleDate BETWEEN @StartDate AND @EndDate
        ORDER BY em.ScheduleDate
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
            }
            // Query for employees
            else if (request.UserTypeID == 2)
            {
                sql = @"
        SELECT 
            e.Employee_id AS EmployeeID,
            e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name AS EmployeeName,
            CONCAT(d.DepartmentName, '-', des.DesignationName) AS DepartmentDesignation,
            em.ScheduleDate AS [DateTime],
            em.EmailSubject,
            em.Status
        FROM tblEmailMaster em
        INNER JOIN tblEmailEmployeeMapping eem ON em.EmailSendID = eem.EmailSendID
        INNER JOIN tbl_EmployeeMaster e ON eem.EmployeeID = e.Employee_id
        INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
        INNER JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
        WHERE em.ScheduleDate BETWEEN @StartDate AND @EndDate
        ORDER BY em.ScheduleDate
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
            }

            // Calculate the offset for pagination
            int offset = (request.PageNumber - 1) * request.PageSize;

            // Execute the query and map results to EmailReportResponse class
            var result = await _connection.QueryAsync<EmailReportResponse>(sql, new { request.StartDate, request.EndDate, Offset = offset, PageSize = request.PageSize });

            // Return the response
            if (result != null && result.Any())
            {
                return new ServiceResponse<List<EmailReportResponse>>(true, "Email Reports Found", result.ToList(), 200);
            }
            else
            {
                return new ServiceResponse<List<EmailReportResponse>>(false, "No records found", null, 404);
            }
        }

        public async Task InsertEmailForStudent(int groupID, int instituteID, int studentID, string emailSubject, string emailBody, DateTime emailDate, int emailStatusID, int SentBy)
        {
            string sql = @"
                INSERT INTO tblEmailStudent (GroupID, InstituteID, StudentID, EmailSubject, EmailBody, EmailDate, EmailStatusID, SentBy)
                VALUES (@GroupID, @InstituteID, @StudentID, @EmailSubject, @EmailBody, @EmailDate, @EmailStatusID, @SentBy)";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                StudentID = studentID,
                EmailSubject = emailSubject,
                EmailBody = emailBody,
                EmailDate = emailDate,
                EmailStatusID = emailStatusID,
                SentBy = SentBy
            });

            string receiverEmailQuery = @"SELECT s.student_id, spi.Email_id FROM tbl_StudentMaster s LEFT JOIN tbl_StudentParentsInfo spi ON s.student_id = spi.student_id where s.student_id = @student_id";
            string receiverEmail = await _connection.QueryFirstOrDefaultAsync<string>(receiverEmailQuery, new { student_id = studentID });
            var email = new SendEmail();
            email.SendEmailWithAttachmentAsync(receiverEmail, string.Empty, emailSubject, emailBody);
        }

        public async Task InsertEmailForEmployee(int groupID, int instituteID, int employeeID, string emailSubject, string emailBody, DateTime emailDate, int emailStatusID, int SentBy)
        {
            string sql = @"
                INSERT INTO tblEmailEmployee (GroupID, InstituteID, EmployeeID, EmailSubject, EmailBody, EmailDate, EmailStatusID, SentBy)
                VALUES (@GroupID, @InstituteID, @EmployeeID, @EmailSubject, @EmailBody, @EmailDate, @EmailStatusID, @SentBy)";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                EmployeeID = employeeID,
                EmailSubject = emailSubject,
                EmailBody = emailBody,
                EmailDate = emailDate,
                EmailStatusID = emailStatusID,
                SentBy = SentBy
            });
            string receiverEmail = await _connection.QueryFirstOrDefaultAsync<string>(@"select EmailID from tbl_EmployeeProfileMaster where Employee_id = @Employee_id", new { Employee_id = employeeID });
            var email = new SendEmail();
            email.SendEmailWithAttachmentAsync(receiverEmail, string.Empty, emailSubject, emailBody);
        }

        public async Task UpdateEmailStatusForStudent(int groupID, int instituteID, int studentID, int emailStatusID)
        {
            string sql = @"
                UPDATE tblEmailStudent
                SET EmailStatusID = @EmailStatusID
                WHERE GroupID = @GroupID
                  AND InstituteID = @InstituteID
                  AND StudentID = @StudentID";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                StudentID = studentID,
                EmailStatusID = emailStatusID
            });
        }

        public async Task UpdateEmailStatusForEmployee(int groupID, int instituteID, int employeeID, int emailStatusID)
        {
            string sql = @"
                UPDATE tblEmailEmployee
                SET EmailStatusID = @EmailStatusID
                WHERE GroupID = @GroupID
                  AND InstituteID = @InstituteID
                  AND EmployeeID = @EmployeeID";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                EmployeeID = employeeID,
                EmailStatusID = emailStatusID
            });
        }


        public async Task<ServiceResponse<List<EmailStudentReportsResponse>>> GetEmailStudentReport(GetEmailStudentReportRequest request)
        {
            string sql = string.Empty;

            // Parse StartDate and EndDate from string to DateTime
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            // SQL query to get the total count of records based on the search, date filters, and InstituteID
            string countSql = @"
            SELECT COUNT(*) 
            FROM tblEmailStudent ss
            INNER JOIN tbl_StudentMaster s ON ss.StudentID = s.student_id
            --INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.GroupID = ss.GroupID
            INNER JOIN tbl_Class c ON s.class_id = c.class_id
            INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
            INNER JOIN tblEmailStatus sts ON ss.EmailStatusID = sts.EmailStatusID
            WHERE ss.EmailDate BETWEEN @StartDate AND @EndDate
            AND (s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name) LIKE '%' + @Search + '%'
            AND s.Institute_id = @InstituteID;";  // Added InstituteID filter

            // Get the total count
            int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { StartDate = startDate, EndDate = endDate, Search = request.Search ?? "", InstituteID = request.InstituteID });

            // Modify the SQL query to get the actual records, including InstituteID filter
            sql = @"
            SELECT 
                s.student_id AS StudentID,
                s.Admission_Number AS AdmissionNumber,
                s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name AS StudentName,
                CONCAT(c.class_name, '-', sec.section_name) AS ClassSection,
                --ss.SMSDate AS DateTime,  -- SMSDate is the equivalent of ScheduleDate
                FORMAT(ss.EmailDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS DateTime, 
                ss.EmailSubject AS EmailSubject,  -- SMSMessage is the equivalent of Message
                sts.EmailStatusName AS Status,  -- Join with tblSMSStatus to get the status name
	            e.First_Name + ' ' + e.Last_Name AS SentBy  -- Adding SentByName from tbl_EmployeeProfileMaster 
            FROM tblEmailStudent ss
            INNER JOIN tbl_StudentMaster s ON ss.StudentID = s.student_id
            --INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.GroupID = ss.GroupID
            INNER JOIN tbl_Class c ON s.class_id = c.class_id
            INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
            INNER JOIN tblEmailStatus sts ON ss.EmailStatusID = sts.EmailStatusID
            LEFT JOIN tbl_EmployeeProfileMaster e ON ss.SentBy = e.Employee_id
            WHERE ss.EmailDate BETWEEN @StartDate AND @EndDate
            AND (s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name) LIKE '%' + @Search + '%'
            AND s.Institute_id = @InstituteID
            ORDER BY ss.EmailDate
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";  // Added InstituteID filter

            // Calculate the offset for pagination
            int offset = (request.PageNumber - 1) * request.PageSize;

            // Execute the query and map results to SMSReportsResponse class
            var result = await _connection.QueryAsync<EmailStudentReportsResponse>(sql, new { StartDate = startDate, EndDate = endDate, Search = request.Search ?? "", InstituteID = request.InstituteID, Offset = offset, PageSize = request.PageSize });

            // Map the result from SMSReport to SMSReportsResponse
            var mappedResult = result.Select(report => new EmailStudentReportsResponse
            {
                StudentID = report.StudentID,
                AdmissionNumber = report.AdmissionNumber,
                StudentName = report.StudentName,
                ClassSection = report.ClassSection,
                DateTime = report.DateTime,
                EmailSubject = report.EmailSubject,
                Status = report.Status, // Assuming you want a string for status
                SentBy = report.SentBy
            }).ToList();

            // Return the response with totalCount
            if (mappedResult.Any())
            {
                return new ServiceResponse<List<EmailStudentReportsResponse>>(true, "Email Student Report Found", mappedResult, 200, totalCount);
            }
            else
            {
                return new ServiceResponse<List<EmailStudentReportsResponse>>(false, "No records found", null, 404);
            }
        }


        public async Task<List<EmailStudentReportExportResponse>> GetEmailStudentReportData(EmailStudentReportExportRequest request)
        {
            // Parse StartDate and EndDate from string to DateTime
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            string sql = @"
            SELECT 
                s.student_id AS StudentID,
                s.Admission_Number AS AdmissionNumber,
                s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name AS StudentName,
                CONCAT(c.class_name, '-', sec.section_name) AS ClassSection,
                --ss.SMSDate AS DateTime,  -- SMSDate is the equivalent of ScheduleDate
                FORMAT(ss.EmailDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS DateTime, 
                ss.EmailSubject AS EmailSubject,  -- SMSMessage is the equivalent of Message
                sts.EmailStatusName AS Status,  -- Join with tblSMSStatus to get the status name
	            e.First_Name + ' ' + e.Last_Name AS SentBy  -- Adding SentByName from tbl_EmployeeProfileMaster 
            FROM tblEmailStudent ss
            INNER JOIN tbl_StudentMaster s ON ss.StudentID = s.student_id
            --INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.GroupID = ss.GroupID
            INNER JOIN tbl_Class c ON s.class_id = c.class_id
            INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
            INNER JOIN tblEmailStatus sts ON ss.EmailStatusID = sts.EmailStatusID
            LEFT JOIN tbl_EmployeeProfileMaster e ON ss.SentBy = e.Employee_id
            WHERE ss.EmailDate BETWEEN @StartDate AND @EndDate
            AND (s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name) LIKE '%' + @Search + '%'
            AND s.Institute_id = @InstituteID
            ORDER BY ss.EmailDate;";

            return (await _connection.QueryAsync<EmailStudentReportExportResponse>(sql, new
            {
                StartDate = startDate,
                EndDate = endDate,
                Search = request.Search,
                InstituteID = request.InstituteID
            })).AsList();
        }


        public async Task<ServiceResponse<List<EmailEmployeeReportsResponse>>> GetEmailEmployeeReport(GetEmailEmployeeReportRequest request)
        {
            // Parse StartDate and EndDate from string to DateTime
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            // SQL query to get the total count of records based on the search, date filters, and InstituteID
            string countSql = @"
             SELECT COUNT(*) 
            FROM tblEmailEmployee se
            INNER JOIN tbl_EmployeeProfileMaster e ON se.EmployeeID = e.Employee_id
            --INNER JOIN tblGroupEmployeeMapping gem ON gem.GroupID = se.GroupID
            INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
            INNER JOIN tbl_Designation de ON e.Designation_id = de.Designation_id
            INNER JOIN tblEmailStatus sts ON se.EmailStatusID = sts.EmailStatusID
             WHERE se.EmailDate BETWEEN @StartDate AND @EndDate
             AND (e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name) LIKE '%' + @Search + '%'
             AND e.Institute_id = @InstituteID;";  // Added InstituteID filter

            // Get the total count
            int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { StartDate = startDate, EndDate = endDate, Search = request.Search ?? "", InstituteID = request.InstituteID });

            // Modify the SQL query to get the actual records, including InstituteID filter
            string sql = @"
            SELECT
                e.Employee_id AS EmployeeID, 
                e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name AS EmployeeName,
                CONCAT(d.DepartmentName, '-', de.DesignationName) AS DepartmentDesignation,
                --se.SMSDate AS DateTime,  -- SMSDate is the equivalent of ScheduleDate
                FORMAT(se.EmailDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS DateTime,  
                se.EmailSubject AS EmailSubject,  -- SMSMessage is the equivalent of Message
                e.EmailID AS EmailID,
                sts.EmailStatusName AS Status,  -- Join with tblSMSStatus to get the status name
                ee.First_Name + ' ' + ee.Last_Name AS SentBy
            FROM tblEmailEmployee se
            INNER JOIN tbl_EmployeeProfileMaster e ON se.EmployeeID = e.Employee_id
            --INNER JOIN tblGroupEmployeeMapping gem ON gem.GroupID = se.GroupID
            INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
            INNER JOIN tbl_Designation de ON e.Designation_id = de.Designation_id
            INNER JOIN tblEmailStatus sts ON se.EmailStatusID = sts.EmailStatusID
            LEFT JOIN tbl_EmployeeProfileMaster ee ON se.SentBy = ee.Employee_id
             WHERE se.EmailDate BETWEEN @StartDate AND @EndDate
             AND (e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name) LIKE '%' + @Search + '%'
             AND e.Institute_id = @InstituteID
             ORDER BY se.EmailDate
             OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";  // Added InstituteID filter

            // Calculate the offset for pagination
            int offset = (request.PageNumber - 1) * request.PageSize;

            // Execute the query and map results to SMSEmployeeReportsResponse class
            var result = await _connection.QueryAsync<EmailEmployeeReportsResponse>(sql, new
            {
                StartDate = startDate,
                EndDate = endDate,
                Search = request.Search ?? "",
                InstituteID = request.InstituteID,
                Offset = offset,
                PageSize = request.PageSize
            });

            // Map the result from SMSEmployeeReportsResponse to SMSEmployeeReportsResponse with formatted DateTime
            var mappedResult = result.Select(report => new EmailEmployeeReportsResponse
            {
                EmployeeID = report.EmployeeID,
                EmployeeName = report.EmployeeName,
                DepartmentDesignation = report.DepartmentDesignation,
                DateTime = report.DateTime.ToString(),  // Format the DateTime as '15 Dec 2024, 05:00 PM'
                EmailSubject = report.EmailSubject,
                EmailID = report.EmailID,
                Status = report.Status,
                SentBy = report.SentBy

            }).ToList();

            // Return the response with totalCount
            if (mappedResult.Any())
            {
                return new ServiceResponse<List<EmailEmployeeReportsResponse>>(true, "Email Employee Report Found", mappedResult, 200, totalCount);
            }
            else
            {
                return new ServiceResponse<List<EmailEmployeeReportsResponse>>(false, "No records found", null, 404);
            }
        }

         
        public async Task<ServiceResponse<string>> GetEmailEmployeeReportExport(EmailEmployeeReportExportRequest request)
        {
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            string sql = @"
            SELECT
                e.Employee_id AS EmployeeID, 
                e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name AS EmployeeName,
                CONCAT(d.DepartmentName, '-', de.DesignationName) AS DepartmentDesignation,
                --se.SMSDate AS DateTime,  -- SMSDate is the equivalent of ScheduleDate
                FORMAT(se.EmailDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS DateTime,  
                se.EmailSubject AS EmailSubject,  -- SMSMessage is the equivalent of Message
                e.EmailID AS EmailID,
                sts.EmailStatusName AS Status,  -- Join with tblSMSStatus to get the status name
                ee.First_Name + ' ' + ee.Last_Name AS SentBy
            FROM tblEmailEmployee se
            INNER JOIN tbl_EmployeeProfileMaster e ON se.EmployeeID = e.Employee_id
            --INNER JOIN tblGroupEmployeeMapping gem ON gem.GroupID = se.GroupID
            INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
            INNER JOIN tbl_Designation de ON e.Designation_id = de.Designation_id
            INNER JOIN tblEmailStatus sts ON se.EmailStatusID = sts.EmailStatusID
            LEFT JOIN tbl_EmployeeProfileMaster ee ON se.SentBy = ee.Employee_id
            WHERE se.EmailDate BETWEEN @StartDate AND @EndDate
            AND (e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name) LIKE '%' + @Search + '%'
            AND e.Institute_id = @InstituteID
            ORDER BY se.EmailDate;";

            // Execute the query and get the result
            var result = await _connection.QueryAsync<EmailEmployeeReportExportResponse>(
                sql, new { StartDate = startDate, EndDate = endDate, Search = request.Search, InstituteID = request.InstituteID });

            // Generate the file based on ExportType
            string filePath = "";
            if (request.ExportType == 1)
            {
                filePath = await GenerateExcelReport(result);
            }
            else if (request.ExportType == 2)
            {
                filePath = await GenerateCsvReport(result);
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return new ServiceResponse<string>(false, "Failed to generate report", null, 400);
            }

            return new ServiceResponse<string>(true, "Excel file generated", filePath, 200);

        }

        private async Task<string> GenerateExcelReport(IEnumerable<EmailEmployeeReportExportResponse> data)
        {
            // Use the current directory of the application for saving the file
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailReports");

            // Ensure the directory exists
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create a dynamic file name based on the current date and time
            string fileName = $"EmailEmployeeReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";  // e.g., SMSEmployeeReport_20241215_123000.xlsx

            // Combine the directory and file name to form the complete file path
            string filePath = Path.Combine(directory, fileName);

            // Generate the Excel file using EPPlus
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.Add("Email Employee Report");

                // Add headers
                worksheet.Cells[1, 1].Value = "Employee Name";
                worksheet.Cells[1, 2].Value = "Department Designation";
                worksheet.Cells[1, 3].Value = "DateTime";
                worksheet.Cells[1, 4].Value = "Email Subject";
                worksheet.Cells[1, 5].Value = "Email ID";
                worksheet.Cells[1, 6].Value = "Status";
                worksheet.Cells[1, 7].Value = "Sent By";

                // Add data rows
                int row = 2;
                foreach (var record in data)
                {
                    worksheet.Cells[row, 1].Value = record.EmployeeName;
                    worksheet.Cells[row, 2].Value = record.DepartmentDesignation;
                    worksheet.Cells[row, 3].Value = record.DateTime;
                    worksheet.Cells[row, 4].Value = record.EmailSubject;
                    worksheet.Cells[row, 5].Value = record.EmailID;
                    worksheet.Cells[row, 6].Value = record.Status; 
                    worksheet.Cells[row, 7].Value = record.SentBy;
                    row++;
                }

                // Save the file
                await package.SaveAsync();
            }

            return filePath;
        }

        private async Task<string> GenerateCsvReport(IEnumerable<EmailEmployeeReportExportResponse> data)
        {
            // Use the current directory of the application for saving the file
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailReports");

            // Ensure the directory exists
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create a dynamic file name based on the current date and time
            string fileName = $"EmailEmployeeReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";  // e.g., SMSEmployeeReport_20241215_123000.csv

            // Combine the directory and file name to form the complete file path
            string filePath = Path.Combine(directory, fileName);

            // Generate the CSV file
            using (var writer = new StreamWriter(filePath))
            {
                // Write headers
                writer.WriteLine("Employee Name,Department Designation,DateTime,Email Subject, Email ID,Status, Sent By");

                // Write data rows
                foreach (var record in data)
                {
                    writer.WriteLine($"{record.EmployeeName},{record.DepartmentDesignation},{record.DateTime.Replace(",","")},{record.EmailSubject}, {record.EmailID},{record.Status},{record.SentBy}");
                }
            }

            return filePath;
        }

    }
}
