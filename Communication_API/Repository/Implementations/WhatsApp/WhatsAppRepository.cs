using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Requests.WhatsApp;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.Responses.WhatsApp;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.WhatsApp;
using Communication_API.Repository.Interfaces.WhatsApp;
using Dapper;
using OfficeOpenXml;
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
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 200 : 400);
        }

        public async Task<ServiceResponse<WhatsAppConfiguration>> GetBalance(int VendorID)
        {
            var query = "SELECT * FROM [tblWhatsAppConfiguration] WHERE ConfigurationID = @VendorID";
            var config = await _connection.QueryFirstOrDefaultAsync<WhatsAppConfiguration>(query, new { VendorID });
            return new ServiceResponse<WhatsAppConfiguration>(config != null, config != null ? "Operation Successful" : "No Record Found", config, config != null ? 200 : 404);
        }

        public async Task<ServiceResponse<string>> AddUpdateTemplate(AddUpdateWhatsAppTemplateRequest request)
        {
            var query = request.WhatsAppTemplateID == 0
                ? "INSERT INTO [tblWhatsAppTemplate] (TemplateCode, TemplateName, TemplateMessage, InstituteID) VALUES (@TemplateCode, @TemplateName, @TemplateMessage, @InstituteID)"
                : "UPDATE [tblWhatsAppTemplate] SET TemplateCode = @TemplateCode, TemplateName = @TemplateName, TemplateMessage = @TemplateMessage, InstituteID = @InstituteID WHERE WhatsAppTemplateID = @WhatsAppTemplateID";

            var parameters = new
            {
                request.WhatsAppTemplateID,
                request.TemplateCode,
                request.TemplateName,
                request.TemplateMessage,
                request.InstituteID  // Passing InstituteID to the query
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 200 : 400);
        }


        public async Task<ServiceResponse<List<WhatsAppTemplate>>> GetWhatsAppTemplate(GetWhatsAppTemplateRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblWhatsAppTemplate] WHERE InstituteID = @InstituteID";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

            var sql = @"SELECT * FROM [tblWhatsAppTemplate]
                WHERE InstituteID = @InstituteID
                ORDER BY WhatsAppTemplateID 
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                InstituteID = request.InstituteID,
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var templates = await _connection.QueryAsync<WhatsAppTemplate>(sql, parameters);
            return new ServiceResponse<List<WhatsAppTemplate>>(true, "Records Found", templates.ToList(), 200, totalCount);
        }

        public async Task<List<GetWhatsAppTemplateExportResponse>> GetWhatsAppTemplateExport(int instituteID)
        {
            var query = @"SELECT TemplateCode, TemplateName, TemplateMessage
                          FROM [tblWhatsAppTemplate]
                          WHERE (@InstituteID IS NULL OR InstituteID = @InstituteID)
                          ORDER BY WhatsAppTemplateID";

            var templates = await _connection.QueryAsync<GetWhatsAppTemplateExportResponse>(query, new { InstituteID = instituteID });
            return templates.AsList();
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
            (PredefinedTemplateID, WhatsAppMessage, UserTypeID, GroupID, Status, ScheduleNow, ScheduleDate, ScheduleTime, AcademicYearCode, InstituteID, SentBy) 
            VALUES (@PredefinedTemplateID, @WhatsAppMessage, @UserTypeID, @GroupID, @Status, @ScheduleNow, @ScheduleDate, @ScheduleTime, @AcademicYearCode, @InstituteID, @SentBy);
            SELECT CAST(SCOPE_IDENTITY() as int);"; // Get the newly inserted ID

            // Execute the query and get the WhatsAppMessageID
            var whatsAppMessageID = await _connection.ExecuteScalarAsync<int>(sql, new
            {
                request.PredefinedTemplateID,
                request.WhatsAppMessage,
                request.UserTypeID,
                request.GroupID,
                request.Status,
                request.ScheduleNow,
                ScheduleDate = scheduleDate,
                ScheduleTime = scheduleTime,
                request.AcademicYearCode,  // Passing the AcademicYearCode
                request.InstituteID,       // Passing the InstituteID
                request.SentBy             // Passing the SentBy
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

                return new ServiceResponse<string>(true, "WhatsApp message sent successfully", "WhatsApp message added/updated successfully", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating WhatsApp message", 400);
            }
        }

        //public async Task<ServiceResponse<string>> Send(SendWhatsAppRequest request)
        //{
        //    // Convert ScheduleDate and ScheduleTime from string to DateTime for insertion into the database
        //    DateTime? scheduleDate = null;
        //    DateTime? scheduleTime = null;

        //    // Parse ScheduleDate if provided
        //    if (!string.IsNullOrEmpty(request.ScheduleDate))
        //    {
        //        scheduleDate = DateTime.ParseExact(request.ScheduleDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //    }

        //    // Parse ScheduleTime if provided
        //    if (!string.IsNullOrEmpty(request.ScheduleTime))
        //    {
        //        scheduleTime = DateTime.ParseExact(request.ScheduleTime, "hh:mm tt", CultureInfo.InvariantCulture);
        //    }

        //    // Step 1: Insert the WhatsApp message into tblWhatsAppMessage
        //    string sql = @"
        //    INSERT INTO [tblWhatsAppMessage] 
        //    (PredefinedTemplateID, WhatsAppMessage, UserTypeID, GroupID, Status, ScheduleNow, ScheduleDate, ScheduleTime, AcademicYearCode, InstituteID) 
        //    VALUES (@PredefinedTemplateID, @WhatsAppMessage, @UserTypeID, @GroupID, @Status, @ScheduleNow, @ScheduleDate, @ScheduleTime, @AcademicYearCode, @InstituteID);
        //    SELECT CAST(SCOPE_IDENTITY() as int);"; // Get the newly inserted ID

        //    // Execute the query and get the WhatsAppMessageID
        //    var whatsAppMessageID = await _connection.ExecuteScalarAsync<int>(sql, new
        //    {
        //        request.PredefinedTemplateID,
        //        request.WhatsAppMessage,
        //        request.UserTypeID,
        //        request.GroupID,
        //        request.Status,
        //        request.ScheduleNow,
        //        ScheduleDate = scheduleDate,
        //        ScheduleTime = scheduleTime,
        //        request.AcademicYearCode,  // Passing the AcademicYearCode
        //        request.InstituteID        // Passing the InstituteID
        //    });

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

        //        return new ServiceResponse<string>(true, "WhatsApp message sent successfully", "WhatsApp message added/updated successfully", 200);
        //    }
        //    else
        //    {
        //        return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating WhatsApp message", 400);
        //    }
        //}


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

        public async Task InsertWhatsAppForStudent(int groupID, int instituteID, int studentID, string whatsAppMessage, DateTime whatsAppDate, int whatsAppStatusID, int SentBy)
        {
            string sql = @"
                INSERT INTO tblWhatsAppStudent (GroupID, InstituteID, StudentID, WhatsAppMessage, WhatsAppDate, WhatsAppStatusID, SentBy)
                VALUES (@GroupID, @InstituteID, @StudentID, @WhatsAppMessage, @WhatsAppDate, @WhatsAppStatusID, @SentBy)";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                StudentID = studentID,
                WhatsAppMessage = whatsAppMessage,
                WhatsAppDate = whatsAppDate,
                WhatsAppStatusID = whatsAppStatusID,
                SentBy = SentBy
            });
        }

        public async Task InsertWhatsAppForEmployee(int groupID, int instituteID, int employeeID, string whatsAppMessage, DateTime whatsAppDate, int whatsAppStatusID, int SentBy)
        {
            string sql = @"
                INSERT INTO tblWhatsAppEmployee (GroupID, InstituteID, EmployeeID, WhatsAppMessage, WhatsAppDate, WhatsAppStatusID, SentBy)
                VALUES (@GroupID, @InstituteID, @EmployeeID, @WhatsAppMessage, @WhatsAppDate, @WhatsAppStatusID, @SentBy)";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                EmployeeID = employeeID,
                WhatsAppMessage = whatsAppMessage,
                WhatsAppDate = whatsAppDate,
                WhatsAppStatusID = whatsAppStatusID,
                SentBy = SentBy
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

        public async Task<ServiceResponse<List<WhatsAppTemplateDDLResponse>>> GetWhatsAppTemplateDDL(int instituteID)
        {
            string sql = @"
                SELECT WhatsAppTemplateID, TemplateName
                FROM tblWhatsAppTemplate
                WHERE InstituteID = @InstituteID";

            var templates = await _connection.QueryAsync<WhatsAppTemplateDDLResponse>(sql, new { InstituteID = instituteID });

            return new ServiceResponse<List<WhatsAppTemplateDDLResponse>>(true, "Templates fetched successfully.", templates.ToList(), 200);
        }




        public async Task<ServiceResponse<List<WhatsAppStudentReportsResponse>>> GetWhatsAppStudentReport(GetWhatsAppStudentReportRequest request)
        {
            string sql = string.Empty;

            // Parse StartDate and EndDate from string to DateTime
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            // SQL query to get the total count of records based on the search, date filters, and InstituteID
            string countSql = @"
            SELECT COUNT(*) 
            FROM tblWhatsAppStudent ss
            INNER JOIN tbl_StudentMaster s ON ss.StudentID = s.student_id
            --INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.GroupID = ss.GroupID
            INNER JOIN tbl_Class c ON s.class_id = c.class_id
            INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
            INNER JOIN tblSMSStatus sts ON ss.WhatsAppStatusID = sts.SMSStatusID
            WHERE ss.WhatsAppDate BETWEEN @StartDate AND @EndDate
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
                FORMAT(ss.WhatsAppDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS DateTime, 
                ss.WhatsAppMessage AS Message,  -- SMSMessage is the equivalent of Message
                sts.WhatsAppStatusName AS Status, 
                e.First_Name + ' ' + e.Last_Name AS SentBy  -- Adding SentByName from tbl_EmployeeProfileMaster
            FROM tblWhatsAppStudent ss
            INNER JOIN tbl_StudentMaster s ON ss.StudentID = s.student_id
            --INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.GroupID = ss.GroupID
            INNER JOIN tbl_Class c ON s.class_id = c.class_id
            INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
            INNER JOIN tblWhatsAppStatus sts ON ss.WhatsAppStatusID = sts.WhatsAppStatusID
            LEFT JOIN tbl_EmployeeProfileMaster e ON ss.SentBy = e.Employee_id
            WHERE ss.WhatsAppDate BETWEEN @StartDate AND @EndDate
            AND (s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name) LIKE '%' + @Search + '%'
            AND s.Institute_id = @InstituteID
            ORDER BY ss.WhatsAppDate
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";  // Added InstituteID filter

            // Calculate the offset for pagination
            int offset = (request.PageNumber - 1) * request.PageSize;

            // Execute the query and map results to SMSReportsResponse class
            var result = await _connection.QueryAsync<WhatsAppStudentReportsResponse>(sql, new { StartDate = startDate, EndDate = endDate, Search = request.Search ?? "", InstituteID = request.InstituteID, Offset = offset, PageSize = request.PageSize });

            // Map the result from SMSReport to SMSReportsResponse
            var mappedResult = result.Select(report => new WhatsAppStudentReportsResponse
            {
                StudentID = report.StudentID,
                AdmissionNumber = report.AdmissionNumber,
                StudentName = report.StudentName,
                ClassSection = report.ClassSection,
                DateTime = report.DateTime,
                Message = report.Message,
                Status = report.Status, // Assuming you want a string for status
                SentBy = report.SentBy
            }).ToList();

            // Return the response with totalCount
            if (mappedResult.Any())
            {
                return new ServiceResponse<List<WhatsAppStudentReportsResponse>>(true, "SMS Student Report Found", mappedResult, 200, totalCount);
            }
            else
            {
                return new ServiceResponse<List<WhatsAppStudentReportsResponse>>(false, "No records found", null, 404);
            }
        }

        public async Task<List<WhatsAppStudentReportExportResponse>> GetWhatsAppStudentReportData(WhatsAppStudentReportExportRequest request)
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
                    FORMAT(ss.WhatsAppDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS DateTime, 
                    ss.WhatsAppMessage AS Message,  -- SMSMessage is the equivalent of Message
                    sts.WhatsAppStatusName AS Status, 
                    e.First_Name + ' ' + e.Last_Name AS SentBy  -- Adding SentByName from tbl_EmployeeProfileMaster
                FROM tblWhatsAppStudent ss
                INNER JOIN tbl_StudentMaster s ON ss.StudentID = s.student_id
                --INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.GroupID = ss.GroupID
                INNER JOIN tbl_Class c ON s.class_id = c.class_id
                INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
                INNER JOIN tblWhatsAppStatus sts ON ss.WhatsAppStatusID = sts.WhatsAppStatusID
                LEFT JOIN tbl_EmployeeProfileMaster e ON ss.SentBy = e.Employee_id
                WHERE ss.WhatsAppDate BETWEEN @StartDate AND @EndDate
                AND (s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name) LIKE '%' + @Search + '%'
                AND s.Institute_id = @InstituteID
                ORDER BY ss.WhatsAppDate;";

            return (await _connection.QueryAsync<WhatsAppStudentReportExportResponse>(sql, new
            {
                StartDate = startDate,
                EndDate = endDate,
                Search = request.Search,
                InstituteID = request.InstituteID
            })).AsList();
        }

        public async Task<ServiceResponse<List<WhatsAppEmployeeReportsResponse>>> GetWhatsAppEmployeeReport(GetWhatsAppEmployeeReportRequest request)
        {
            // Parse StartDate and EndDate from string to DateTime
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            // SQL query to get the total count of records based on the search, date filters, and InstituteID
            string countSql = @"
                   SELECT COUNT(*) 
            FROM tblWhatsAppEmployee se
            INNER JOIN tbl_EmployeeProfileMaster e ON se.EmployeeID = e.Employee_id
            --INNER JOIN tblGroupEmployeeMapping gem ON gem.GroupID = se.GroupID
            INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
            INNER JOIN tbl_Designation de ON e.Designation_id = de.Designation_id
            INNER JOIN tblWhatsAppStatus sts ON se.WhatsAppStatusID = sts.WhatsAppStatusID
            WHERE se.WhatsAppDate BETWEEN @StartDate AND @EndDate
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
                 FORMAT(se.WhatsAppDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS DateTime,  
                 se.WhatsAppMessage AS Message,  -- SMSMessage is the equivalent of Message
                 sts.WhatsAppStatusName AS Status,  -- Join with tblSMSStatus to get the status name
                ee.First_Name + ' ' + ee.Last_Name AS SentBy  -- Adding SentByName from tbl_EmployeeProfileMaster 
             FROM tblWhatsAppEmployee se
             INNER JOIN tbl_EmployeeProfileMaster e ON se.EmployeeID = e.Employee_id
             --INNER JOIN tblGroupEmployeeMapping gem ON gem.GroupID = se.GroupID
             INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
             INNER JOIN tbl_Designation de ON e.Designation_id = de.Designation_id
             INNER JOIN tblWhatsAppStatus sts ON se.WhatsAppStatusID = sts.WhatsAppStatusID
             LEFT JOIN tbl_EmployeeProfileMaster ee ON se.SentBy = ee.Employee_id 
               WHERE se.WhatsAppDate BETWEEN @StartDate AND @EndDate
               AND (e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name) LIKE '%' + @Search + '%'
               AND e.Institute_id = @InstituteID
               ORDER BY se.WhatsAppDate
               OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";  // Added InstituteID filter

            // Calculate the offset for pagination
            int offset = (request.PageNumber - 1) * request.PageSize;

            // Execute the query and map results to SMSEmployeeReportsResponse class
            var result = await _connection.QueryAsync<WhatsAppEmployeeReportsResponse>(sql, new
            {
                StartDate = startDate,
                EndDate = endDate,
                Search = request.Search ?? "",
                InstituteID = request.InstituteID,
                Offset = offset,
                PageSize = request.PageSize
            });

            // Map the result from SMSEmployeeReportsResponse to SMSEmployeeReportsResponse with formatted DateTime
            var mappedResult = result.Select(report => new WhatsAppEmployeeReportsResponse
            {
                EmployeeID = report.EmployeeID,
                EmployeeName = report.EmployeeName,
                DepartmentDesignation = report.DepartmentDesignation,
                DateTime = report.DateTime.ToString(),  // Format the DateTime as '15 Dec 2024, 05:00 PM'
                Message = report.Message,
                Status = report.Status,
                SentBy = report.SentBy
            }).ToList();

            // Return the response with totalCount
            if (mappedResult.Any())
            {
                return new ServiceResponse<List<WhatsAppEmployeeReportsResponse>>(true, "WhatsApp Employee Report Found", mappedResult, 200, totalCount);
            }
            else
            {
                return new ServiceResponse<List<WhatsAppEmployeeReportsResponse>>(false, "No records found", null, 404);
            }
        }


        public async Task<string> GetWhatsAppEmployeeReportExport(WhatsAppEmployeeReportExportRequest request)
        {
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            string sql = @"
             SELECT
                 e.Employee_id AS EmployeeID, 
                 e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name AS EmployeeName,
                 CONCAT(d.DepartmentName, '-', de.DesignationName) AS DepartmentDesignation,
                 --se.SMSDate AS DateTime,  -- SMSDate is the equivalent of ScheduleDate
                 FORMAT(se.WhatsAppDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS DateTime,  
                 se.WhatsAppMessage AS Message,  -- SMSMessage is the equivalent of Message
                 sts.WhatsAppStatusName AS Status,  -- Join with tblSMSStatus to get the status name
                ee.First_Name + ' ' + ee.Last_Name AS SentBy  -- Adding SentByName from tbl_EmployeeProfileMaster 
             FROM tblWhatsAppEmployee se
             INNER JOIN tbl_EmployeeProfileMaster e ON se.EmployeeID = e.Employee_id
             --INNER JOIN tblGroupEmployeeMapping gem ON gem.GroupID = se.GroupID
             INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
             INNER JOIN tbl_Designation de ON e.Designation_id = de.Designation_id
             INNER JOIN tblWhatsAppStatus sts ON se.WhatsAppStatusID = sts.WhatsAppStatusID
             LEFT JOIN tbl_EmployeeProfileMaster ee ON se.SentBy = ee.Employee_id 
            WHERE se.WhatsAppDate BETWEEN @StartDate AND @EndDate
            AND (e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name) LIKE '%' + @Search + '%'
            AND e.Institute_id = @InstituteID
            ORDER BY se.WhatsAppDate;";

            // Execute the query and get the result
            var result = await _connection.QueryAsync<WhatsAppEmployeeReportExportResponse>(
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

            return filePath;
        }


        private async Task<string> GenerateExcelReport(IEnumerable<WhatsAppEmployeeReportExportResponse> data)
        {
            // Use the current directory of the application for saving the file
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WhatsAppReports");

            // Ensure the directory exists
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create a dynamic file name based on the current date and time
            string fileName = $"WhatsAppEmployeeReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";  // e.g., SMSEmployeeReport_20241215_123000.xlsx

            // Combine the directory and file name to form the complete file path
            string filePath = Path.Combine(directory, fileName);

            // Generate the Excel file using EPPlus
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.Add("WhatsApp Employee Report");

                // Add headers
                worksheet.Cells[1, 1].Value = "Employee Name";
                worksheet.Cells[1, 2].Value = "Department Designation";
                worksheet.Cells[1, 3].Value = "DateTime";
                worksheet.Cells[1, 4].Value = "Message";
                worksheet.Cells[1, 5].Value = "Status";
                worksheet.Cells[1, 6].Value = "Sent By";


                // Add data rows
                int row = 2;
                foreach (var record in data)
                {
                    worksheet.Cells[row, 1].Value = record.EmployeeName;
                    worksheet.Cells[row, 2].Value = record.DepartmentDesignation;
                    worksheet.Cells[row, 3].Value = record.DateTime;
                    worksheet.Cells[row, 4].Value = record.Message;
                    worksheet.Cells[row, 5].Value = record.Status;
                    worksheet.Cells[row, 6].Value = record.SentBy;

                    row++;
                }

                // Save the file
                await package.SaveAsync();
            }

            return filePath;
        }

        private async Task<string> GenerateCsvReport(IEnumerable<WhatsAppEmployeeReportExportResponse> data)
        {
            // Use the current directory of the application for saving the file
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WhatsAppReports");

            // Ensure the directory exists
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create a dynamic file name based on the current date and time
            string fileName = $"WhatsAppEmployeeReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";  // e.g., SMSEmployeeReport_20241215_123000.csv

            // Combine the directory and file name to form the complete file path
            string filePath = Path.Combine(directory, fileName);

            // Generate the CSV file
            using (var writer = new StreamWriter(filePath))
            {
                // Write headers
                writer.WriteLine("Employee Name,Department Designation,DateTime,Message,Status, Sent By");

                // Write data rows
                foreach (var record in data)
                {
                    writer.WriteLine($"{record.EmployeeName},{record.DepartmentDesignation},{record.DateTime.Replace(",","")},{record.Message.Replace(",", "")},{record.Status},{record.SentBy}");
                }
            }

            return filePath;
        }

        public async Task<ServiceResponse<WhatsAppPlanResponse>> GetWhatsAppPlan(int WhatsAppVendorID)
        {
            var query = "SELECT RateID, CreditCount, CreditAmount FROM tblWhatsAppTopUpRate WHERE WhatsAppVendorID = @WhatsAppVendorID AND IsDeleted = 0";

            var plan = await _connection.QueryFirstOrDefaultAsync<WhatsAppPlanResponse>(query, new { WhatsAppVendorID });

            if (plan != null)
            {
                return new ServiceResponse<WhatsAppPlanResponse>(true, "Plan found", plan, 200);
            }
            else
            {
                return new ServiceResponse<WhatsAppPlanResponse>(false, "No plan found for this vendor", null, 404);
            }
        }


        public async Task<ServiceResponse<List<GetWhatsAppTopUpHistoryResponse>>> GetWhatsAppTopUpHistory(int instituteID)
        {
            var query = @"
            SELECT WO.WhatsAppOrderID, WT.CreditCount AS WhatsAppCredits, WO.TransactionAmount AS Amount, WO.TransactionDate
            FROM tblWhatsAppOrder WO
            LEFT JOIN tblWhatsAppTopUpRate WT ON WO.RateID = WT.RateID
            WHERE WO.InstituteID = @InstituteID";

            var result = await _connection.QueryAsync<dynamic>(query, new { InstituteID = instituteID });

            var responseList = result.Select(x => new GetWhatsAppTopUpHistoryResponse
            {
                WhatsAppOrderID = x.WhatsAppOrderID,
                WhatsAppCredits = x.WhatsAppCredits,
                Amount = x.Amount,
                TransactionDate = ((DateTime)x.TransactionDate).ToString("dd-MM-yyyy 'at' hh:mm tt") // Format the date
            }).ToList();

            if (responseList.Any())
            {
                return new ServiceResponse<List<GetWhatsAppTopUpHistoryResponse>>(true, "WhatsApp Top Up History Found", responseList, 200);
            }
            else
            {
                return new ServiceResponse<List<GetWhatsAppTopUpHistoryResponse>>(false, "No records found", null, 404);
            }
        }

        public async Task<List<GetWhatsAppTopUpHistoryExportResponse>> GetWhatsAppTopUpHistoryExport(int instituteID)
        {
            var query = @"
                SELECT WT.CreditCount AS WhatsAppCredits, 
                       WO.TransactionAmount AS Amount, 
                       WO.TransactionDate
                FROM tblWhatsAppOrder WO
                LEFT JOIN tblWhatsAppTopUpRate WT ON WO.RateID = WT.RateID
                WHERE WO.InstituteID = @InstituteID";

            var result = await _connection.QueryAsync<GetWhatsAppTopUpHistoryExportResponse>(query, new { InstituteID = instituteID });

            // Format the TransactionDate as 'dd-MM-yyyy at hh:mm tt'
            foreach (var record in result)
            {
                record.TransactionDate = DateTime.Parse(record.TransactionDate).ToString("dd-MM-yyyy 'at' hh:mm tt");
            }

            return result.ToList();
        }
    }
}
