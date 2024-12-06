using Communication_API.DTOs.Requests.WhatsApp;
using Communication_API.DTOs.Responses.WhatsApp;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.WhatsApp;
using Communication_API.Repository.Interfaces.WhatsApp;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Communication_API.Repository.Implementations.WhatsApp
{
    public class WhatsAppRepository : IWhatsAppRepository
    {
        private readonly IDbConnection _connection;

        public WhatsAppRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> Setup(SetupWhatsAppRequest request)
        {
            var query = "INSERT INTO [tblWhatsAppConfiguration] (EmailID, PhoneNumber, Address) VALUES (@EmailID, @PhoneNumber, @Address)";

            var parameters = new
            {
                request.EmailID,
                request.PhoneNumber,
                request.Address
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<WhatsAppConfiguration>> GetBalance(int VendorID)
        {
            var query = "SELECT * FROM [tblWhatsAppConfiguration] WHERE ConfigurationID = @VendorID";
            var config = await _connection.QueryFirstOrDefaultAsync<WhatsAppConfiguration>(query, new { VendorID });
            return new ServiceResponse<WhatsAppConfiguration>(config != null, config != null ? "Operation Successful" : "No Record Found", config, config != null ? 200 : 404);
        }

        public async Task<ServiceResponse<string>> AddUpdateTemplate(AddUpdateTemplateRequest request)
        {
            var query = request.TemplateID == 0
                ? "INSERT INTO [tblWhatsAppTemplate] (TemplateTypeID, TemplateName, TemplateMessage) VALUES (@TemplateTypeID, @TemplateName, @TemplateMessage)"
                : "UPDATE [tblWhatsAppTemplate] SET TemplateTypeID = @TemplateTypeID, TemplateName = @TemplateName, TemplateMessage = @TemplateMessage WHERE TemplateID = @TemplateID";

            var parameters = new
            {
                request.TemplateID,
                request.TemplateTypeID,
                request.TemplateName,
                request.TemplateMessage
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<List<WhatsAppTemplate>>> GetWhatsAppTemplate(GetWhatsAppTemplateRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblWhatsAppTemplate]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblWhatsAppTemplate]
                        ORDER BY WhatsAppTemplateID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var templates = await _connection.QueryAsync<WhatsAppTemplate>(sql, parameters);
            return new ServiceResponse<List<WhatsAppTemplate>>(true, "Records Found", templates.ToList(), 302, totalCount);
        }

        //public async Task<ServiceResponse<string>> Send(SendWhatsAppRequest request)
        //{
        //    // Step 1: Insert the WhatsApp message into tblWhatsAppMessage
        //    string sql = @"
        //        INSERT INTO [tblWhatsAppMessage] 
        //        (PredefinedTemplateID, WhatsAppMessage, UserTypeID, GroupID, ScheduleNow, ScheduleDate, ScheduleTime) 
        //        VALUES (@PredefinedTemplateID, @WhatsAppMessage, @UserTypeID, @GroupID, @ScheduleNow, @ScheduleDate, @ScheduleTime);
        //        SELECT CAST(SCOPE_IDENTITY() as int);"; // Get the newly inserted ID

        //    // Execute the query and get the WhatsAppMessageID
        //    var whatsAppMessageID = await _connection.ExecuteScalarAsync<int>(sql, request);

        //    // Step 2: Proceed with student or employee mappings
        //    if (whatsAppMessageID > 0)
        //    {
        //        // Handle student mapping
        //        if (request.UserTypeID == 1 && request.StudentIDs != null && request.StudentIDs.Count > 0)
        //        {
        //            // Insert into tblWhatsAppStudentMapping
        //            string insertStudentMappingSql = "INSERT INTO tblWhatsAppStudentMapping (WhatsAppMessageID, StudentID) VALUES (@WhatsAppMessageID, @StudentID)";
        //            foreach (var studentID in request.StudentIDs)
        //            {
        //                await _connection.ExecuteAsync(insertStudentMappingSql, new { WhatsAppMessageID = whatsAppMessageID, StudentID = studentID });
        //            }
        //        }
        //        // Handle employee mapping
        //        else if (request.UserTypeID == 2 && request.EmployeeIDs != null && request.EmployeeIDs.Count > 0)
        //        {
        //            // Insert into tblWhatsAppEmployeeMapping
        //            string insertEmployeeMappingSql = "INSERT INTO tblWhatsAppEmployeeMapping (WhatsAppMessageID, EmployeeID) VALUES (@WhatsAppMessageID, @EmployeeID)";
        //            foreach (var employeeID in request.EmployeeIDs)
        //            {
        //                await _connection.ExecuteAsync(insertEmployeeMappingSql, new { WhatsAppMessageID = whatsAppMessageID, EmployeeID = employeeID });
        //            }
        //        }

        //        return new ServiceResponse<string>(true, "WhatsApp message sent successfully", "WhatsApp message added/updated successfully", 201);
        //    }
        //    else
        //    {
        //        return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating WhatsApp message", 400);
        //    }
        //}


        public async Task<ServiceResponse<string>> Send(SendWhatsAppRequest request)
        {
            // Convert ScheduleDate and ScheduleTime from string to DateTime for insertion into the database
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

            // Step 1: Insert the WhatsApp message into tblWhatsAppMessage
            string sql = @"
        INSERT INTO [tblWhatsAppMessage] 
        (PredefinedTemplateID, WhatsAppMessage, UserTypeID, GroupID, ScheduleNow, ScheduleDate, ScheduleTime, AcademicYearCode, InstituteID) 
        VALUES (@PredefinedTemplateID, @WhatsAppMessage, @UserTypeID, @GroupID, @ScheduleNow, @ScheduleDate, @ScheduleTime, @AcademicYearCode, @InstituteID);
        SELECT CAST(SCOPE_IDENTITY() as int);"; // Get the newly inserted ID

            // Execute the query and get the WhatsAppMessageID
            var whatsAppMessageID = await _connection.ExecuteScalarAsync<int>(sql, new
            {
                request.PredefinedTemplateID,
                request.WhatsAppMessage,
                request.UserTypeID,
                request.GroupID,
                request.ScheduleNow,
                ScheduleDate = scheduleDate,
                ScheduleTime = scheduleTime,
                request.AcademicYearCode,  // Passing the AcademicYearCode
                request.InstituteID        // Passing the InstituteID
            });

            // Step 2: Proceed with student or employee mappings
            if (whatsAppMessageID > 0)
            {
                // Handle student mapping
                if (request.UserTypeID == 1 && request.StudentIDs != null && request.StudentIDs.Count > 0)
                {
                    // Insert into tblWhatsAppStudentMapping
                    string insertStudentMappingSql = "INSERT INTO tblWhatsAppStudentMapping (WhatsAppMessageID, StudentID) VALUES (@WhatsAppMessageID, @StudentID)";
                    foreach (var studentID in request.StudentIDs)
                    {
                        await _connection.ExecuteAsync(insertStudentMappingSql, new { WhatsAppMessageID = whatsAppMessageID, StudentID = studentID });
                    }
                }
                // Handle employee mapping
                else if (request.UserTypeID == 2 && request.EmployeeIDs != null && request.EmployeeIDs.Count > 0)
                {
                    // Insert into tblWhatsAppEmployeeMapping
                    string insertEmployeeMappingSql = "INSERT INTO tblWhatsAppEmployeeMapping (WhatsAppMessageID, EmployeeID) VALUES (@WhatsAppMessageID, @EmployeeID)";
                    foreach (var employeeID in request.EmployeeIDs)
                    {
                        await _connection.ExecuteAsync(insertEmployeeMappingSql, new { WhatsAppMessageID = whatsAppMessageID, EmployeeID = employeeID });
                    }
                }

                return new ServiceResponse<string>(true, "WhatsApp message sent successfully", "WhatsApp message added/updated successfully", 201);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating WhatsApp message", 400);
            }
        }


        public async Task<ServiceResponse<List<WhatsAppReportResponse>>> GetWhatsAppReport(GetWhatsAppReportRequest request)
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
            wm.ScheduleDate AS DateTime,
            wm.WhatsAppMessage,
            wm.Status
        FROM tblWhatsAppMessage wm
        INNER JOIN tblWhatsAppStudentMapping wsm ON wm.WhatsAppMessageID = wsm.WhatsAppMessageID
        INNER JOIN tbl_StudentMaster s ON wsm.StudentID = s.student_id
        INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.GroupID = wm.GroupID
        INNER JOIN tbl_Class c ON gcsm.ClassID = c.class_id
        INNER JOIN tbl_Section sec ON gcsm.SectionID = sec.section_id
        WHERE wm.ScheduleDate BETWEEN @StartDate AND @EndDate
        ORDER BY wm.ScheduleDate
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
            wm.ScheduleDate AS DateTime,
            wm.WhatsAppMessage,
            wm.Status
        FROM tblWhatsAppMessage wm
        INNER JOIN tblWhatsAppEmployeeMapping wem ON wm.WhatsAppMessageID = wem.WhatsAppMessageID
        INNER JOIN tbl_EmployeeMaster e ON wem.EmployeeID = e.Employee_id
        INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
        INNER JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
        WHERE wm.ScheduleDate BETWEEN @StartDate AND @EndDate
        ORDER BY wm.ScheduleDate
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
            }

            // Calculate the offset for pagination
            int offset = (request.PageNumber - 1) * request.PageSize;

            // Execute the query and map results to WhatsAppReportResponse class
            var result = await _connection.QueryAsync<WhatsAppReportResponse>(sql, new { request.StartDate, request.EndDate, Offset = offset, PageSize = request.PageSize });

            // Return the response
            if (result != null && result.Any())
            {
                return new ServiceResponse<List<WhatsAppReportResponse>>(true, "WhatsApp Reports Found", result.ToList(), 200);
            }
            else
            {
                return new ServiceResponse<List<WhatsAppReportResponse>>(false, "No records found", null, 404);
            }
        }

        public async Task InsertWhatsAppForStudent(int groupID, int instituteID, int studentID, string whatsAppMessage, DateTime whatsAppDate, int whatsAppStatusID)
        {
            string sql = @"
                INSERT INTO tblWhatsAppStudent (GroupID, InstituteID, StudentID, WhatsAppMessage, WhatsAppDate, WhatsAppStatusID)
                VALUES (@GroupID, @InstituteID, @StudentID, @WhatsAppMessage, @WhatsAppDate, @WhatsAppStatusID)";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                StudentID = studentID,
                WhatsAppMessage = whatsAppMessage,
                WhatsAppDate = whatsAppDate,
                WhatsAppStatusID = whatsAppStatusID
            });
        }

        public async Task InsertWhatsAppForEmployee(int groupID, int instituteID, int employeeID, string whatsAppMessage, DateTime whatsAppDate, int whatsAppStatusID)
        {
            string sql = @"
                INSERT INTO tblWhatsAppEmployee (GroupID, InstituteID, EmployeeID, WhatsAppMessage, WhatsAppDate, WhatsAppStatusID)
                VALUES (@GroupID, @InstituteID, @EmployeeID, @WhatsAppMessage, @WhatsAppDate, @WhatsAppStatusID)";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                EmployeeID = employeeID,
                WhatsAppMessage = whatsAppMessage,
                WhatsAppDate = whatsAppDate,
                WhatsAppStatusID = whatsAppStatusID
            });
        }

        public async Task UpdateWhatsAppStatusForStudent(int groupID, int instituteID, int studentID, int whatsAppStatusID)
        {
            string sql = @"
                UPDATE tblWhatsAppStudent
                SET WhatsAppStatusID = @WhatsAppStatusID
                WHERE GroupID = @GroupID
                  AND InstituteID = @InstituteID
                  AND StudentID = @StudentID";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                StudentID = studentID,
                WhatsAppStatusID = whatsAppStatusID
            });
        }

        public async Task UpdateWhatsAppStatusForEmployee(int groupID, int instituteID, int employeeID, int whatsAppStatusID)
        {
            string sql = @"
                UPDATE tblWhatsAppEmployee
                SET WhatsAppStatusID = @WhatsAppStatusID
                WHERE GroupID = @GroupID
                  AND InstituteID = @InstituteID
                  AND EmployeeID = @EmployeeID";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                EmployeeID = employeeID,
                WhatsAppStatusID = whatsAppStatusID
            });
        }
    }
}
