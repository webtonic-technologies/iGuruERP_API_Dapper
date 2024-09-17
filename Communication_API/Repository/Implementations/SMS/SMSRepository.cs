using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.SMS;
using Communication_API.Repository.Interfaces.SMS;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Communication_API.Repository.Implementations.SMS
{
    public class SMSRepository : ISMSRepository
    {
        private readonly IDbConnection _connection;

        public SMSRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> SetupSMSConfiguration(SetupSMSConfigurationRequest request)
        {
            var query = "INSERT INTO [tblSMSConfiguration] (APIkey, UserID, SenderID, Status) VALUES (@APIkey, @UserID, @SenderID, @Status)";

            var parameters = new
            {
                request.APIkey,
                request.UserID,
                request.SenderID,
                request.Status
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<SMSBalance>> GetSMSBalance(int VendorID)
        {
            var sql = "SELECT * FROM [tblSMSBalance] WHERE VendorID = @VendorID";
            var balance = await _connection.QueryFirstOrDefaultAsync<SMSBalance>(sql, new { VendorID });
            return new ServiceResponse<SMSBalance>(true, "Record Found", balance, 302);
        }

        public async Task<ServiceResponse<string>> CreateSMSTemplate(CreateSMSTemplateRequest request)
        {
            var query = "INSERT INTO [tblSMSTemplate] (TemplateName, TemplateMessage) VALUES (@TemplateName, @TemplateMessage)";

            var parameters = new
            {
                request.TemplateName,
                request.TemplateMessage
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<List<SMSTemplate>>> GetAllSMSTemplate(GetAllSMSTemplateRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblSMSTemplate]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblSMSTemplate]
                        ORDER BY TemplateID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var templates = await _connection.QueryAsync<SMSTemplate>(sql, parameters);
            return new ServiceResponse<List<SMSTemplate>>(true, "Records Found", templates.ToList(), 302, totalCount);
        }

        public async Task<ServiceResponse<string>> SendNewSMS(SendNewSMSRequest request)
        {
            // Step 1: Insert or update the SMS message
            string sql;
            if (request.SMSID == 0)
            {
                sql = @"INSERT INTO [tblSMSMessage] (PredefinedTemplateID, SMSMessage, UserTypeID, GroupID, Status, ScheduleNow, ScheduleDate, ScheduleTime) 
                VALUES (@PredefinedTemplateID, @SMSMessage, @UserTypeID, @GroupID, @Status, @ScheduleNow, @ScheduleDate, @ScheduleTime);
                SELECT CAST(SCOPE_IDENTITY() as int);";  // Get the newly inserted ID
            }
            else
            {
                sql = @"UPDATE [tblSMSMessage] 
                SET PredefinedTemplateID = @PredefinedTemplateID, SMSMessage = @SMSMessage, UserTypeID = @UserTypeID, GroupID = @GroupID, 
                    Status = @Status, ScheduleNow = @ScheduleNow, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime 
                WHERE SMSID = @SMSID;
                SELECT @SMSID;";
            }

            // Execute the query and get the SMSID
            var smsID = await _connection.ExecuteScalarAsync<int>(sql, request);

            if (smsID > 0)
            {
                // Step 2: Handle the student or employee mappings
                if (request.UserTypeID == 1 && request.StudentIDs != null && request.StudentIDs.Count > 0)
                {
                    // Delete existing mappings if updating
                    if (request.SMSID != 0)
                    {
                        string deleteStudentMappingSql = "DELETE FROM tblSMSStudentMapping WHERE SMSID = @SMSID";
                        await _connection.ExecuteAsync(deleteStudentMappingSql, new { SMSID = smsID });
                    }

                    // Insert into tblSMSStudentMapping
                    string insertStudentMappingSql = "INSERT INTO tblSMSStudentMapping (SMSID, StudentID) VALUES (@SMSID, @StudentID)";
                    foreach (var studentID in request.StudentIDs)
                    {
                        await _connection.ExecuteAsync(insertStudentMappingSql, new { SMSID = smsID, StudentID = studentID });
                    }
                }
                else if (request.UserTypeID == 2 && request.EmployeeIDs != null && request.EmployeeIDs.Count > 0)
                {
                    // Delete existing mappings if updating
                    if (request.SMSID != 0)
                    {
                        string deleteEmployeeMappingSql = "DELETE FROM tblSMSEmployeeMapping WHERE SMSID = @SMSID";
                        await _connection.ExecuteAsync(deleteEmployeeMappingSql, new { SMSID = smsID });
                    }

                    // Insert into tblSMSEmployeeMapping
                    string insertEmployeeMappingSql = "INSERT INTO tblSMSEmployeeMapping (SMSID, EmployeeID) VALUES (@SMSID, @EmployeeID)";
                    foreach (var employeeID in request.EmployeeIDs)
                    {
                        await _connection.ExecuteAsync(insertEmployeeMappingSql, new { SMSID = smsID, EmployeeID = employeeID });
                    }
                }

                return new ServiceResponse<string>(true, "Operation Successful", "SMS added/updated successfully", StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating SMS", StatusCodes.Status400BadRequest);
            }
        }

        public async Task<ServiceResponse<List<NotificationReport>>> GetSMSReport(GetSMSReportRequest request)
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
            sm.ScheduleDate AS DateTime,
            st.TemplateMessage AS Message,
            sm.Status
        FROM tblSMSMessage sm
        INNER JOIN tblSMSStudentMapping ssm ON sm.SMSID = ssm.SMSID
        INNER JOIN tbl_StudentMaster s ON ssm.StudentID = s.student_id
        INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.GroupID = sm.GroupID
        INNER JOIN tbl_Class c ON gcsm.ClassID = c.class_id
        INNER JOIN tbl_Section sec ON gcsm.SectionID = sec.section_id
        INNER JOIN tblSMSTemplate st ON sm.PredefinedTemplateID = st.TemplateID
        WHERE sm.ScheduleDate BETWEEN @StartDate AND @EndDate
        ORDER BY sm.ScheduleDate
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
            sm.ScheduleDate AS DateTime,
            st.TemplateMessage AS Message,
            sm.Status
        FROM tblSMSMessage sm
        INNER JOIN tblSMSEmployeeMapping sem ON sm.SMSID = sem.SMSID
        INNER JOIN tbl_EmployeeMaster e ON sem.EmployeeID = e.Employee_id
        INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
        INNER JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
        INNER JOIN tblSMSTemplate st ON sm.PredefinedTemplateID = st.TemplateID
        WHERE sm.ScheduleDate BETWEEN @StartDate AND @EndDate
        ORDER BY sm.ScheduleDate
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
            }

            // Calculate the offset for pagination
            int offset = (request.PageNumber - 1) * request.PageSize;

            // Execute the query and map results to NotificationReport class
            var result = await _connection.QueryAsync<NotificationReport>(sql, new { request.StartDate, request.EndDate, Offset = offset, PageSize = request.PageSize });

            // Return the response
            if (result != null && result.Any())
            {
                return new ServiceResponse<List<NotificationReport>>(true, "SMS Report Found", result.ToList(), 200);
            }
            else
            {
                return new ServiceResponse<List<NotificationReport>>(false, "No records found", null, 404);
            }
        }

    }
}
