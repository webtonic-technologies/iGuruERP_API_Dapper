﻿using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Responses;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.SMS;
using Communication_API.Repository.Interfaces.SMS;
using Dapper;
using OfficeOpenXml;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

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
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 200 : 400);
        }

        public async Task<ServiceResponse<SMSBalance>> GetSMSBalance(int VendorID)
        {
            var sql = "SELECT * FROM [tblSMSBalance] WHERE VendorID = @VendorID";
            var balance = await _connection.QueryFirstOrDefaultAsync<SMSBalance>(sql, new { VendorID });
            return new ServiceResponse<SMSBalance>(true, "Record Found", balance, 302);
        }

        public async Task<ServiceResponse<string>> CreateSMSTemplate(CreateSMSTemplateRequest request)
        {
            var query = "INSERT INTO [tblSMSTemplate] (TemplateName, TemplateMessage, InstituteID, TemplateCode) VALUES (@TemplateName, @TemplateMessage, @InstituteID, @TemplateCode)";  // Updated query

            var parameters = new
            {
                request.TemplateName,
                request.TemplateMessage,
                request.InstituteID,
                request.TemplateCode  // Add TemplateCode to the parameters
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 200 : 400);
        }



        public async Task<ServiceResponse<List<SMSTemplate>>> GetAllSMSTemplate(GetAllSMSTemplateRequest request)
        {
            // SQL to count the total number of records, applying the InstituteID filter if provided
            var countSql = "SELECT COUNT(*) FROM [tblSMSTemplate] WHERE (@InstituteID IS NULL OR InstituteID = @InstituteID)";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

            // SQL query to retrieve the templates, including TemplateCode
            var sql = @"SELECT TemplateID, TemplateName, TemplateMessage, TemplateCode 
                FROM [tblSMSTemplate]
                WHERE (@InstituteID IS NULL OR InstituteID = @InstituteID)
                ORDER BY TemplateID 
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            // Define parameters for the query, including InstituteID and pagination
            var parameters = new
            {
                InstituteID = request.InstituteID,
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            // Execute the query to fetch the templates
            var templates = await _connection.QueryAsync<SMSTemplate>(sql, parameters);

            // Return the response with the templates and total count
            return new ServiceResponse<List<SMSTemplate>>(true, "Records Found", templates.ToList(), 200, totalCount);
        }

        public async Task<List<SMSTemplateExportResponse>> GetAllSMSTemplateExport(int instituteID)
        {
            var query = @"SELECT TemplateCode, TemplateName, TemplateMessage
                          FROM [tblSMSTemplate]
                          WHERE (@InstituteID IS NULL OR InstituteID = @InstituteID)
                          ORDER BY TemplateID";

            var templates = await _connection.QueryAsync<SMSTemplateExportResponse>(query, new { InstituteID = instituteID });
            return templates.AsList();
        }

        public async Task<ServiceResponse<string>> SendNewSMS(SendNewSMSRequest request)
        {
            // Convert ScheduleDate and ScheduleTime from string to DateTime for insertion into database
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

            // Step 1: Insert or update the SMS message
            string sql;
            if (request.SMSID == 0)
            {
                sql = @"INSERT INTO [tblSMSMessage] (PredefinedTemplateID, SMSMessage, UserTypeID, GroupID, Status, ScheduleNow, ScheduleDate, ScheduleTime, AcademicYearCode, InstituteID, SentBy) 
        VALUES (@PredefinedTemplateID, @SMSMessage, @UserTypeID, @GroupID, @Status, @ScheduleNow, @ScheduleDate, @ScheduleTime, @AcademicYearCode, @InstituteID, @SentBy);
        SELECT CAST(SCOPE_IDENTITY() as int);";  // Get the newly inserted ID
            }
            else
            {
                sql = @"UPDATE [tblSMSMessage] 
        SET PredefinedTemplateID = @PredefinedTemplateID, SMSMessage = @SMSMessage, UserTypeID = @UserTypeID, GroupID = @GroupID, 
            Status = @Status, ScheduleNow = @ScheduleNow, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime, 
            AcademicYearCode = @AcademicYearCode, InstituteID = @InstituteID, SentBy = @SentBy 
        WHERE SMSID = @SMSID;
        SELECT @SMSID;";
            }

            // Execute the query and get the SMSID
            var smsID = await _connection.ExecuteScalarAsync<int>(sql, new
            {
                request.PredefinedTemplateID,
                request.SMSMessage,
                request.UserTypeID,
                request.GroupID,
                request.Status,
                request.ScheduleNow,
                ScheduleDate = scheduleDate,
                ScheduleTime = scheduleTime,
                request.AcademicYearCode,
                request.InstituteID,
                request.SMSID,
                request.SentBy // Include SentBy in the parameters
            });

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

        //public async Task<ServiceResponse<string>> SendNewSMS(SendNewSMSRequest request)
        //{
        //    // Convert ScheduleDate and ScheduleTime from string to DateTime for insertion into database
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

        //    // Step 1: Insert or update the SMS message
        //    string sql;
        //    if (request.SMSID == 0)
        //    {
        //        sql = @"INSERT INTO [tblSMSMessage] (PredefinedTemplateID, SMSMessage, UserTypeID, GroupID, Status, ScheduleNow, ScheduleDate, ScheduleTime, AcademicYearCode, InstituteID) 
        //        VALUES (@PredefinedTemplateID, @SMSMessage, @UserTypeID, @GroupID, @Status, @ScheduleNow, @ScheduleDate, @ScheduleTime, @AcademicYearCode, @InstituteID);
        //        SELECT CAST(SCOPE_IDENTITY() as int);";  // Get the newly inserted ID
        //            }
        //            else
        //            {
        //                sql = @"UPDATE [tblSMSMessage] 
        //        SET PredefinedTemplateID = @PredefinedTemplateID, SMSMessage = @SMSMessage, UserTypeID = @UserTypeID, GroupID = @GroupID, 
        //            Status = @Status, ScheduleNow = @ScheduleNow, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime, 
        //            AcademicYearCode = @AcademicYearCode, InstituteID = @InstituteID 
        //        WHERE SMSID = @SMSID;
        //        SELECT @SMSID;";
        //    }

        //    // Execute the query and get the SMSID
        //    var smsID = await _connection.ExecuteScalarAsync<int>(sql, new
        //    {
        //        request.PredefinedTemplateID,
        //        request.SMSMessage,
        //        request.UserTypeID,
        //        request.GroupID,
        //        request.Status,
        //        request.ScheduleNow,
        //        ScheduleDate = scheduleDate,
        //        ScheduleTime = scheduleTime,
        //        request.AcademicYearCode,
        //        request.InstituteID,
        //        request.SMSID
        //    });

        //    if (smsID > 0)
        //    {
        //        // Step 2: Handle the student or employee mappings
        //        if (request.UserTypeID == 1 && request.StudentIDs != null && request.StudentIDs.Count > 0)
        //        {
        //            // Delete existing mappings if updating
        //            if (request.SMSID != 0)
        //            {
        //                string deleteStudentMappingSql = "DELETE FROM tblSMSStudentMapping WHERE SMSID = @SMSID";
        //                await _connection.ExecuteAsync(deleteStudentMappingSql, new { SMSID = smsID });
        //            }

        //            // Insert into tblSMSStudentMapping
        //            string insertStudentMappingSql = "INSERT INTO tblSMSStudentMapping (SMSID, StudentID) VALUES (@SMSID, @StudentID)";
        //            foreach (var studentID in request.StudentIDs)
        //            {
        //                await _connection.ExecuteAsync(insertStudentMappingSql, new { SMSID = smsID, StudentID = studentID });
        //            }
        //        }
        //        else if (request.UserTypeID == 2 && request.EmployeeIDs != null && request.EmployeeIDs.Count > 0)
        //        {
        //            // Delete existing mappings if updating
        //            if (request.SMSID != 0)
        //            {
        //                string deleteEmployeeMappingSql = "DELETE FROM tblSMSEmployeeMapping WHERE SMSID = @SMSID";
        //                await _connection.ExecuteAsync(deleteEmployeeMappingSql, new { SMSID = smsID });
        //            }

        //            // Insert into tblSMSEmployeeMapping
        //            string insertEmployeeMappingSql = "INSERT INTO tblSMSEmployeeMapping (SMSID, EmployeeID) VALUES (@SMSID, @EmployeeID)";
        //            foreach (var employeeID in request.EmployeeIDs)
        //            {
        //                await _connection.ExecuteAsync(insertEmployeeMappingSql, new { SMSID = smsID, EmployeeID = employeeID });
        //            }
        //        }

        //        return new ServiceResponse<string>(true, "Operation Successful", "SMS added/updated successfully", StatusCodes.Status200OK);
        //    }
        //    else
        //    {
        //        return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating SMS", StatusCodes.Status400BadRequest);
        //    }
        //}

        public async Task<ServiceResponse<List<SMSStudentReportsResponse>>> GetSMSStudentReport(GetSMSStudentReportRequest request)
        {
            string sql = string.Empty;

            // Parse StartDate and EndDate from string to DateTime
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            // SQL query to get the total count of records based on the search, date filters, and InstituteID
            string countSql = @"
            SELECT COUNT(*) 
            FROM tblSMSStudent ss
            INNER JOIN tbl_StudentMaster s ON ss.StudentID = s.student_id
            --INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.GroupID = ss.GroupID
            INNER JOIN tbl_Class c ON s.class_id = c.class_id
            INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
            INNER JOIN tblSMSStatus sts ON ss.SMSStatusID = sts.SMSStatusID
            WHERE ss.SMSDate BETWEEN @StartDate AND @EndDate
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
                FORMAT(ss.SMSDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS DateTime, 
                ss.SMSMessage AS Message,
                sts.SMSStatusName AS Status, 
                e.First_Name + ' ' + e.Last_Name AS SentByName  -- Adding SentByName from tbl_EmployeeProfileMaster
            FROM tblSMSStudent ss
            INNER JOIN tbl_StudentMaster s ON ss.StudentID = s.student_id
            INNER JOIN tbl_Class c ON s.class_id = c.class_id
            INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
            INNER JOIN tblSMSStatus sts ON ss.SMSStatusID = sts.SMSStatusID
            LEFT JOIN tbl_EmployeeProfileMaster e ON ss.SentBy = e.Employee_id
            WHERE ss.SMSDate BETWEEN @StartDate AND @EndDate
            AND (s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name) LIKE '%' + @Search + '%'
            AND s.Institute_id = @InstituteID
            ORDER BY ss.SMSDate
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";  // Added InstituteID filter

            // Calculate the offset for pagination
            int offset = (request.PageNumber - 1) * request.PageSize;

            // Execute the query and map results to SMSReportsResponse class
            var result = await _connection.QueryAsync<SMSStudentReportsResponse>(sql, new { StartDate = startDate, EndDate = endDate, Search = request.Search ?? "", InstituteID = request.InstituteID, Offset = offset, PageSize = request.PageSize });

            // Map the result from SMSReport to SMSReportsResponse
            var mappedResult = result.Select(report => new SMSStudentReportsResponse
            {
                StudentID = report.StudentID,
                AdmissionNumber = report.AdmissionNumber,
                StudentName = report.StudentName,
                ClassSection = report.ClassSection,
                DateTime = report.DateTime,
                Message = report.Message,
                Status = report.Status,
                SentByName = report.SentByName// Assuming you want a string for status
            }).ToList();

            // Return the response with totalCount
            if (mappedResult.Any())
            {
                return new ServiceResponse<List<SMSStudentReportsResponse>>(true, "SMS Student Report Found", mappedResult, 200, totalCount);
            }
            else
            {
                return new ServiceResponse<List<SMSStudentReportsResponse>>(false, "No records found", null, 404);
            }
        }

        public async Task<List<SMSStudentReportExportResponse>> GetSMSStudentReportData(SMSStudentReportExportRequest request)
        {
            // Parse StartDate and EndDate from string to DateTime
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            string sql = @"
            SELECT  
                s.Admission_Number AS AdmissionNumber,
                s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name AS StudentName,
                CONCAT(c.class_name, '-', sec.section_name) AS ClassSection,
                FORMAT(ss.SMSDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS DateTime, 
                ss.SMSMessage AS Message,
                sts.SMSStatusName AS Status, 
                e.First_Name + ' ' + e.Last_Name AS SentBy  -- Adding SentByName from tbl_EmployeeProfileMaster
            FROM tblSMSStudent ss
            INNER JOIN tbl_StudentMaster s ON ss.StudentID = s.student_id
            INNER JOIN tbl_Class c ON s.class_id = c.class_id
            INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
            INNER JOIN tblSMSStatus sts ON ss.SMSStatusID = sts.SMSStatusID
            LEFT JOIN tbl_EmployeeProfileMaster e ON ss.SentBy = e.Employee_id
            WHERE ss.SMSDate BETWEEN @StartDate AND @EndDate
            AND (s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name) LIKE '%' + @Search + '%'
            AND s.Institute_id = @InstituteID
            ORDER BY ss.SMSDate;";



            //string sql = @"
            //SELECT 
            //    s.Admission_Number AS AdmissionNumber,
            //    s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name AS StudentName,
            //    CONCAT(c.class_name, '-', sec.section_name) AS ClassSection,
            //    --ss.SMSDate AS DateTime,  -- SMSDate is the equivalent of ScheduleDate
            //    FORMAT(ss.SMSDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS DateTime,  
            //    ss.SMSMessage AS Message,  -- SMSMessage is the equivalent of Message
            //    sts.SMSStatusName AS Status  -- Join with tblSMSStatus to get the status name
            //FROM tblSMSStudent ss
            //INNER JOIN tbl_StudentMaster s ON ss.StudentID = s.student_id
            //--INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.GroupID = ss.GroupID
            //INNER JOIN tbl_Class c ON s.class_id = c.class_id
            //INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
            //INNER JOIN tblSMSStatus sts ON ss.SMSStatusID = sts.SMSStatusID
            //WHERE ss.SMSDate BETWEEN @StartDate AND @EndDate
            //AND (s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name) LIKE '%' + @Search + '%'
            //AND s.Institute_id = @InstituteID
            //ORDER BY ss.SMSDate;";

            return (await _connection.QueryAsync<SMSStudentReportExportResponse>(sql, new
            {
                StartDate = startDate,
                EndDate = endDate,
                Search = request.Search,
                InstituteID = request.InstituteID
            })).AsList();
        }

        public async Task<ServiceResponse<List<SMSEmployeeReportsResponse>>> GetSMSEmployeeReport(GetSMSEmployeeReportRequest request)
        {
            // Parse StartDate and EndDate from string to DateTime
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            // SQL query to get the total count of records based on the search, date filters, and InstituteID
            string countSql = @"
            SELECT COUNT(*) 
            FROM tblSMSEmployee se
            INNER JOIN tbl_EmployeeProfileMaster e ON se.EmployeeID = e.Employee_id
            --INNER JOIN tblGroupEmployeeMapping gem ON gem.GroupID = se.GroupID
            INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
            INNER JOIN tbl_Designation de ON e.Designation_id = de.Designation_id
            INNER JOIN tblSMSStatus sts ON se.SMSStatusID = sts.SMSStatusID
            WHERE se.SMSDate BETWEEN @StartDate AND @EndDate
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
                FORMAT(se.SMSDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS DateTime,  
                se.SMSMessage AS Message,  -- SMSMessage is the equivalent of Message 
                sts.SMSStatusName AS Status,  -- Join with tblSMSStatus to get the status name
	            ee.First_Name + ' ' + ee.Last_Name AS SentByName  -- Adding SentByName from tbl_EmployeeProfileMaster  
            FROM tblSMSEmployee se
            INNER JOIN tbl_EmployeeProfileMaster e ON se.EmployeeID = e.Employee_id
            --INNER JOIN tblGroupEmployeeMapping gem ON gem.GroupID = se.GroupID
            INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
            INNER JOIN tbl_Designation de ON e.Designation_id = de.Designation_id
            INNER JOIN tblSMSStatus sts ON se.SMSStatusID = sts.SMSStatusID
            LEFT JOIN tbl_EmployeeProfileMaster ee ON se.SentBy = ee.Employee_id 
            WHERE se.SMSDate BETWEEN @StartDate AND @EndDate
            AND (e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name) LIKE '%' + @Search + '%'
            AND e.Institute_id = @InstituteID
            ORDER BY se.SMSDate
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";  // Added InstituteID filter

            // Calculate the offset for pagination
            int offset = (request.PageNumber - 1) * request.PageSize;

            // Execute the query and map results to SMSEmployeeReportsResponse class
            var result = await _connection.QueryAsync<SMSEmployeeReportsResponse>(sql, new
            {
                StartDate = startDate,
                EndDate = endDate,
                Search = request.Search ?? "",
                InstituteID = request.InstituteID,
                Offset = offset,
                PageSize = request.PageSize
            });

            // Map the result from SMSEmployeeReportsResponse to SMSEmployeeReportsResponse with formatted DateTime
            var mappedResult = result.Select(report => new SMSEmployeeReportsResponse
            {
                EmployeeID = report.EmployeeID,
                EmployeeName = report.EmployeeName,
                DepartmentDesignation = report.DepartmentDesignation,
                DateTime = report.DateTime.ToString(),  // Format the DateTime as '15 Dec 2024, 05:00 PM'
                Message = report.Message,
                Status = report.Status,
                SentByName = report.SentByName
            }).ToList();

            // Return the response with totalCount
            if (mappedResult.Any())
            {
                return new ServiceResponse<List<SMSEmployeeReportsResponse>>(true, "SMS Employee Report Found", mappedResult, 200, totalCount);
            }
            else
            {
                return new ServiceResponse<List<SMSEmployeeReportsResponse>>(false, "No records found", null, 404);
            }
        }

        public async Task<string> GetSMSEmployeeReportExport(SMSEmployeeReportExportRequest request)
        {
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
             
            string sql = @"
            SELECT
                e.Employee_id AS EmployeeID, 
                e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name AS EmployeeName,
                CONCAT(d.DepartmentName, '-', de.DesignationName) AS DepartmentDesignation,
                --se.SMSDate AS DateTime,  
                FORMAT(se.SMSDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS DateTime,  
                se.SMSMessage AS Message,  
                sts.SMSStatusName AS Status  
            FROM tblSMSEmployee se
            INNER JOIN tbl_EmployeeProfileMaster e ON se.EmployeeID = e.Employee_id
            --INNER JOIN tblGroupEmployeeMapping gem ON gem.GroupID = se.GroupID
            INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
            INNER JOIN tbl_Designation de ON e.Designation_id = de.Designation_id
            INNER JOIN tblSMSStatus sts ON se.SMSStatusID = sts.SMSStatusID
            WHERE se.SMSDate BETWEEN @StartDate AND @EndDate
            AND (e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name) LIKE '%' + @Search + '%'
            AND e.Institute_id = @InstituteID
            ORDER BY se.SMSDate;";

            // Execute the query and get the result
            var result = await _connection.QueryAsync<SMSEmployeeReportExportResponse>(
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

        private async Task<string> GenerateExcelReport(IEnumerable<SMSEmployeeReportExportResponse> data)
        {
            // Use the current directory of the application for saving the file
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SMSReports");

            // Ensure the directory exists
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create a dynamic file name based on the current date and time
            string fileName = $"SMSEmployeeReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";  // e.g., SMSEmployeeReport_20241215_123000.xlsx

            // Combine the directory and file name to form the complete file path
            string filePath = Path.Combine(directory, fileName);

            // Generate the Excel file using EPPlus
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.Add("SMS Employee Report");

                // Add headers
                worksheet.Cells[1, 1].Value = "Employee Name";
                worksheet.Cells[1, 2].Value = "Department Designation";
                worksheet.Cells[1, 3].Value = "DateTime";
                worksheet.Cells[1, 4].Value = "Message";
                worksheet.Cells[1, 5].Value = "Status";

                // Add data rows
                int row = 2;
                foreach (var record in data)
                {
                    worksheet.Cells[row, 1].Value = record.EmployeeName;
                    worksheet.Cells[row, 2].Value = record.DepartmentDesignation;
                    worksheet.Cells[row, 3].Value = record.DateTime;
                    worksheet.Cells[row, 4].Value = record.Message;
                    worksheet.Cells[row, 5].Value = record.Status;
                    row++;
                }

                // Save the file
                await package.SaveAsync();
            }

            return filePath;
        }

        private async Task<string> GenerateCsvReport(IEnumerable<SMSEmployeeReportExportResponse> data)
        {
            // Use the current directory of the application for saving the file
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SMSReports");

            // Ensure the directory exists
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create a dynamic file name based on the current date and time
            string fileName = $"SMSEmployeeReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";  // e.g., SMSEmployeeReport_20241215_123000.csv

            // Combine the directory and file name to form the complete file path
            string filePath = Path.Combine(directory, fileName);

            // Generate the CSV file
            using (var writer = new StreamWriter(filePath))
            {
                // Write headers
                writer.WriteLine("Employee Name,Department Designation,DateTime,Message,Status");

                // Write data rows
                foreach (var record in data)
                {
                    writer.WriteLine($"{record.EmployeeName},{record.DepartmentDesignation},{record.DateTime.Replace(",","")},{record.Message.Replace(",", "")},{record.Status}");
                }
            }

            return filePath;
        }



        //public async Task<ServiceResponse<List<NotificationReport>>> GetSMSReport(GetSMSReportRequest request)
        //{
        //    string sql = string.Empty;

        //    // Query for students
        //    if (request.UserTypeID == 1)
        //    {
        //        sql = @"
        //SELECT 
        //    s.student_id AS StudentID,
        //    s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name AS StudentName,
        //    CONCAT(c.class_name, '-', sec.section_name) AS ClassSection,
        //    sm.ScheduleDate AS DateTime,
        //    st.TemplateMessage AS Message,
        //    sm.Status
        //FROM tblSMSMessage sm
        //INNER JOIN tblSMSStudentMapping ssm ON sm.SMSID = ssm.SMSID
        //INNER JOIN tbl_StudentMaster s ON ssm.StudentID = s.student_id
        //INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.GroupID = sm.GroupID
        //INNER JOIN tbl_Class c ON gcsm.ClassID = c.class_id
        //INNER JOIN tbl_Section sec ON gcsm.SectionID = sec.section_id
        //INNER JOIN tblSMSTemplate st ON sm.PredefinedTemplateID = st.TemplateID
        //WHERE sm.ScheduleDate BETWEEN @StartDate AND @EndDate
        //ORDER BY sm.ScheduleDate
        //OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
        //    }
        //    // Query for employees
        //    else if (request.UserTypeID == 2)
        //    {
        //        sql = @"
        //SELECT 
        //    e.Employee_id AS EmployeeID,
        //    e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name AS EmployeeName,
        //    CONCAT(d.DepartmentName, '-', des.DesignationName) AS DepartmentDesignation,
        //    sm.ScheduleDate AS DateTime,
        //    st.TemplateMessage AS Message,
        //    sm.Status
        //FROM tblSMSMessage sm
        //INNER JOIN tblSMSEmployeeMapping sem ON sm.SMSID = sem.SMSID
        //INNER JOIN tbl_EmployeeMaster e ON sem.EmployeeID = e.Employee_id
        //INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
        //INNER JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
        //INNER JOIN tblSMSTemplate st ON sm.PredefinedTemplateID = st.TemplateID
        //WHERE sm.ScheduleDate BETWEEN @StartDate AND @EndDate
        //ORDER BY sm.ScheduleDate
        //OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
        //    }

        //    // Calculate the offset for pagination
        //    int offset = (request.PageNumber - 1) * request.PageSize;

        //    // Execute the query and map results to NotificationReport class
        //    var result = await _connection.QueryAsync<NotificationReport>(sql, new { request.StartDate, request.EndDate, Offset = offset, PageSize = request.PageSize });

        //    // Return the response
        //    if (result != null && result.Any())
        //    {
        //        return new ServiceResponse<List<NotificationReport>>(true, "SMS Report Found", result.ToList(), 200);
        //    }
        //    else
        //    {
        //        return new ServiceResponse<List<NotificationReport>>(false, "No records found", null, 404);
        //    }
        //}

        public async Task InsertSMSForStudent(int groupID, int instituteID, int studentID, string smsMessage, DateTime smsDate, int smsStatusID, int SentBy)
        {
            string sql = @"
                INSERT INTO tblSMSStudent (GroupID, InstituteID, StudentID, SMSMessage, SMSDate, SMSStatusID, SentBy)
                VALUES (@GroupID, @InstituteID, @StudentID, @SMSMessage, @SMSDate, @SMSStatusID, @SentBy)";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                StudentID = studentID,
                SMSMessage = smsMessage,
                SMSDate = smsDate,
                SMSStatusID = smsStatusID,
                SentBy = SentBy
            });
        }

        public async Task InsertSMSForEmployee(int groupID, int instituteID, int employeeID, string smsMessage, DateTime smsDate, int smsStatusID, int SentBy)
        {
            string sql = @"
                INSERT INTO tblSMSEmployee (GroupID, InstituteID, EmployeeID, SMSMessage, SMSDate, SMSStatusID, SentBy)
                VALUES (@GroupID, @InstituteID, @EmployeeID, @SMSMessage, @SMSDate, @SMSStatusID, @SentBy)";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                EmployeeID = employeeID,
                SMSMessage = smsMessage,
                SMSDate = smsDate,
                SMSStatusID = smsStatusID, 
                SentBy = SentBy
            });
        }

        public async Task UpdateSMSStudentStatus(int groupID, int instituteID, int studentID, int smsStatusID)
        {
            string sql = @"
                UPDATE tblSMSStudent
                SET SMSStatusID = @SMSStatusID
                WHERE GroupID = @GroupID
                  AND InstituteID = @InstituteID
                  AND StudentID = @StudentID";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                StudentID = studentID,
                SMSStatusID = smsStatusID
            });
        }

        public async Task UpdateSMSEmployeeStatus(int groupID, int instituteID, int employeeID, int smsStatusID)
        {
            string sql = @"
                UPDATE tblSMSEmployee
                SET SMSStatusID = @SMSStatusID
                WHERE GroupID = @GroupID
                  AND InstituteID = @InstituteID
                  AND EmployeeID = @EmployeeID";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                EmployeeID = employeeID,
                SMSStatusID = smsStatusID
            });
        }

        public async Task<ServiceResponse<List<SMSTemplateDDLResponse>>> GetSMSTemplateDDL(int instituteID)
        {
            string sql = @"
                SELECT TemplateID, TemplateName
                FROM tblSMSTemplate
                WHERE InstituteID = @InstituteID";

            var templates = await _connection.QueryAsync<SMSTemplateDDLResponse>(sql, new { InstituteID = instituteID });

            return new ServiceResponse<List<SMSTemplateDDLResponse>>(true, "Templates fetched successfully.", templates.ToList(), 200);
        }

        public async Task<ServiceResponse<SMSPlanResponse>> GetSMSPlan(int SMSVendorID)
        {
            var query = "SELECT RateID, CreditCount, CreditAmount FROM tblSMSTopUpRate WHERE SMSVendorID = @SMSVendorID AND IsDeleted = 0";

            var plan = await _connection.QueryFirstOrDefaultAsync<SMSPlanResponse>(query, new { SMSVendorID });

            if (plan != null)
            {
                return new ServiceResponse<SMSPlanResponse>(true, "Plan found", plan, 200);
            }
            else
            {
                return new ServiceResponse<SMSPlanResponse>(false, "No plan found for this vendor", null, 404);
            }
        }

        public async Task<ServiceResponse<List<GetSMSTopUpHistoryResponse>>> GetSMSTopUpHistory(int instituteID)
        {
            var query = @"
            SELECT SO.SMSOrderID, ST.CreditCount AS SMSCredits, SO.TransactionAmount AS Amount, SO.TransactionDate
            FROM tblSMSOrder SO
            LEFT JOIN tblSMSTopUpRate ST ON SO.RateID = ST.RateID
            WHERE SO.InstituteID = @InstituteID";

            var result = await _connection.QueryAsync<dynamic>(query, new { InstituteID = instituteID });

            // Format the TransactionDate and return the response
            var responseList = result.Select(x => new GetSMSTopUpHistoryResponse
            {
                SMSOrderID = x.SMSOrderID,
                SMSCredits = x.SMSCredits,
                Amount = x.Amount,
                // Format the TransactionDate to the required format
                TransactionDate = ((DateTime)x.TransactionDate).ToString("dd-MM-yyyy 'at' hh:mm tt")
            }).ToList();

            if (responseList.Any())
            {
                return new ServiceResponse<List<GetSMSTopUpHistoryResponse>>(true, "SMS Top Up History Found", responseList, 200);
            }
            else
            {
                return new ServiceResponse<List<GetSMSTopUpHistoryResponse>>(false, "No records found", null, 404);
            }
        }


        public async Task<List<GetSMSTopUpHistoryExportResponse>> GetSMSTopUpHistoryExport(int instituteID)
        {
            var query = @"
                SELECT ST.CreditCount AS SMSCredits, 
                       SO.TransactionAmount AS Amount, 
                       SO.TransactionDate
                FROM tblSMSOrder SO
                LEFT JOIN tblSMSTopUpRate ST ON SO.RateID = ST.RateID
                WHERE SO.InstituteID = @InstituteID";

            var result = await _connection.QueryAsync<GetSMSTopUpHistoryExportResponse>(query, new { InstituteID = instituteID });

            // Format the TransactionDate as 'dd-MM-yyyy at hh:mm tt'
            foreach (var record in result)
            {
                record.TransactionDate = DateTime.Parse(record.TransactionDate).ToString("dd-MM-yyyy 'at' hh:mm tt");
            }

            return result.ToList();
        }

    }
}
