using Communication_API.DTOs.Requests.Email;
using Communication_API.DTOs.Responses.Email;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Email;
using Communication_API.Repository.Interfaces.Email;
using Dapper;
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
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
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
            var query = @"INSERT INTO [tblEmailMaster] (EmailSubject, EmailBody, UserTypeID, GroupID, Status, ScheduleNow, ScheduleDate, ScheduleTime, AcademicYearCode, InstituteID) 
          VALUES (@EmailSubject, @EmailBody, @UserTypeID, @GroupID, @Status, @ScheduleNow, @ScheduleDate, @ScheduleTime, @AcademicYearCode, @InstituteID);
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
                request.InstituteID
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

                return new ServiceResponse<string>(true, "Email added successfully", "Email added/updated successfully", 201);
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

        public async Task InsertEmailForStudent(int groupID, int instituteID, int studentID, string emailSubject, string emailBody, DateTime emailDate, int emailStatusID)
        {
            string sql = @"
                INSERT INTO tblEmailStudent (GroupID, InstituteID, StudentID, EmailSubject, EmailBody, EmailDate, EmailStatusID)
                VALUES (@GroupID, @InstituteID, @StudentID, @EmailSubject, @EmailBody, @EmailDate, @EmailStatusID)";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                StudentID = studentID,
                EmailSubject = emailSubject,
                EmailBody = emailBody,
                EmailDate = emailDate,
                EmailStatusID = emailStatusID
            });
        }
        public async Task InsertEmailForEmployee(int groupID, int instituteID, int employeeID, string emailSubject, string emailBody, DateTime emailDate, int emailStatusID)
        {
            string sql = @"
                INSERT INTO tblEmailEmployee (GroupID, InstituteID, EmployeeID, EmailSubject, EmailBody, EmailDate, EmailStatusID)
                VALUES (@GroupID, @InstituteID, @EmployeeID, @EmailSubject, @EmailBody, @EmailDate, @EmailStatusID)";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                EmployeeID = employeeID,
                EmailSubject = emailSubject,
                EmailBody = emailBody,
                EmailDate = emailDate,
                EmailStatusID = emailStatusID
            });
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

    }
}
