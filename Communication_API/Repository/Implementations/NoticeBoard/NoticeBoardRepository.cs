using Communication_API.DTOs.Requests.NoticeBoard;
using Communication_API.DTOs.Responses.NoticeBoard;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.NoticeBoard;
using Communication_API.Repository.Interfaces.NoticeBoard;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Communication_API.Repository.Implementations.NoticeBoard
{
    public class NoticeBoardRepository : INoticeBoardRepository
    {
        private readonly IDbConnection _connection;

        public NoticeBoardRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateNotice(AddUpdateNoticeRequest request)
        {
            // Convert string dates to DateTime objects
            DateTime? startDate = null;
            DateTime? endDate = null;
            DateTime? scheduleDate = null;
            DateTime? scheduleTime = null;

            if (!string.IsNullOrEmpty(request.StartDate))
            {
                startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(request.EndDate))
            {
                endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(request.ScheduleDate))
            {
                scheduleDate = DateTime.ParseExact(request.ScheduleDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(request.ScheduleTime))
            {
                scheduleTime = DateTime.ParseExact(request.ScheduleTime, "h:mm tt", CultureInfo.InvariantCulture);
            }

            // Determine if it's an insert or update operation
            bool isInsert = request.NoticeID == 0;

            // SQL query for insert or update
            var query = isInsert
                ? @"INSERT INTO [tblNotice] 
              (InstituteID, Title, Description, Attachments, StartDate, EndDate, IsStudent, IsEmployee, ScheduleNow, ScheduleDate, ScheduleTime, CreatedBy, CreatedOn) 
            VALUES 
              (@InstituteID, @Title, @Description, @Attachments, @StartDate, @EndDate, @IsStudent, @IsEmployee, @ScheduleNow, @ScheduleDate, @ScheduleTime, @CreatedBy, GETDATE());
            SELECT CAST(SCOPE_IDENTITY() as int);"
                : @"UPDATE [tblNotice] 
            SET InstituteID = @InstituteID, Title = @Title, Description = @Description, Attachments = @Attachments, StartDate = @StartDate, EndDate = @EndDate, 
                IsStudent = @IsStudent, IsEmployee = @IsEmployee, ScheduleNow = @ScheduleNow, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime 
            WHERE NoticeID = @NoticeID;
            SELECT @NoticeID;";

            // Define the parameters for the query
            var parameters = new
            {
                request.InstituteID,
                request.NoticeID,
                request.Title,
                request.Description,
                request.Attachments,
                StartDate = startDate,
                EndDate = endDate,
                request.IsStudent,
                request.IsEmployee,
                request.ScheduleNow,
                ScheduleDate = scheduleDate,
                ScheduleTime = scheduleTime?.TimeOfDay,
                request.CreatedBy
            };

            // Execute the query and get the NoticeID
            var noticeID = await _connection.ExecuteScalarAsync<int>(query, parameters);

            if (noticeID > 0)
            {
                    if (request.IsStudent && request.StudentMappings != null && request.StudentMappings.Count > 0)
                    {
                        // Delete existing student mappings if updating
                        if (request.NoticeID != 0)
                        {
                            string deleteStudentMappingSql = "DELETE FROM tblNoticeStudentMapping WHERE NoticeID = @NoticeID";
                            await _connection.ExecuteAsync(deleteStudentMappingSql, new { NoticeID = noticeID });
                        }

                        // Insert new student mappings
                        string insertStudentMappingSql = "INSERT INTO tblNoticeStudentMapping (NoticeID, StudentID, ClassID, SectionID) VALUES (@NoticeID, @StudentID, @ClassID, @SectionID)";
                        foreach (var studentMapping in request.StudentMappings)
                        {
                            await _connection.ExecuteAsync(insertStudentMappingSql, new { NoticeID = noticeID, studentMapping.StudentID, studentMapping.ClassID, studentMapping.SectionID });
                        }
                    }

                // Step 3: Insert into tblNoticeEmployeeMapping if IsEmployee is true
                if (request.IsEmployee && request.EmployeeMappings != null && request.EmployeeMappings.Count > 0)
                {
                    // Delete existing employee mappings if updating
                    if (request.NoticeID != 0)
                    {
                        string deleteEmployeeMappingSql = "DELETE FROM tblNoticeEmployeeMapping WHERE NoticeID = @NoticeID";
                        await _connection.ExecuteAsync(deleteEmployeeMappingSql, new { NoticeID = noticeID });
                    }

                    // Insert new employee mappings
                    string insertEmployeeMappingSql = "INSERT INTO tblNoticeEmployeeMapping (NoticeID, EmployeeID, DepartmentID, DesignationID) VALUES (@NoticeID, @EmployeeID, @DepartmentID, @DesignationID)";
                    foreach (var employeeMapping in request.EmployeeMappings)
                    {
                        await _connection.ExecuteAsync(insertEmployeeMappingSql, new { NoticeID = noticeID, employeeMapping.EmployeeID, employeeMapping.DepartmentID, employeeMapping.DesignationID });
                    }
                }

                return new ServiceResponse<string>(true, "Operation Successful", "Notice added/updated successfully", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating notice", 400);
            }
        }


        public async Task<ServiceResponse<List<NoticeResponse>>> GetAllNotice(GetAllNoticeRequest request)
        {
            // Convert StartDate and EndDate to DateTime if not null, otherwise use null
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (!string.IsNullOrEmpty(request.StartDate))
            {
                startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(request.EndDate))
            {
                endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            // Construct the base query for counting the records
            var countSql = "SELECT COUNT(*) FROM [tblNotice] WHERE (@InstituteID IS NULL OR InstituteID = @InstituteID)";

            // Add condition for Search parameter (filter by Title)
            if (!string.IsNullOrEmpty(request.Search))
            {
                countSql += " AND (Title LIKE @Search)";
            }

            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID, Search = "%" + request.Search + "%" });

            // Construct the main query to retrieve the notices
            var sql = @"
        SELECT 
            n.NoticeID,
            n.Title,
            n.Description,
            FORMAT(n.StartDate, 'dd-MM-yyyy') AS StartDate,   -- Format StartDate
            FORMAT(n.EndDate, 'dd-MM-yyyy') AS EndDate,       -- Format EndDate
            FORMAT(n.CreatedOn, 'dd-MM-yyyy') AS CreatedOn,   -- Format CreatedOn
            ISNULL(CONCAT(emp.First_Name, ' ', emp.Middle_Name, ' ', emp.Last_Name), 'Unknown') AS CreatedBy, -- Full name
            CASE 
                WHEN n.IsStudent = 1 THEN (
                    SELECT STRING_AGG(CONCAT(c.class_name, '-', sec.section_name), ', ') 
                    FROM tblNoticeStudentMapping ns
                    INNER JOIN tbl_Class c ON ns.ClassID = c.class_id
                    INNER JOIN tbl_Section sec ON ns.SectionID = sec.section_id
                    WHERE ns.NoticeID = n.NoticeID
                )
                WHEN n.IsEmployee = 1 THEN (
                    SELECT STRING_AGG(CONCAT(d.DepartmentName, '-', des.DesignationName), ', ') 
                    FROM tblNoticeEmployeeMapping ne
                    INNER JOIN tbl_Department d ON ne.DepartmentID = d.Department_id
                    INNER JOIN tbl_Designation des ON ne.DesignationID = des.Designation_id
                    WHERE ne.NoticeID = n.NoticeID
                )
            END AS Recipients
        FROM tblNotice n
        LEFT JOIN tbl_EmployeeProfileMaster emp ON n.CreatedBy = emp.Employee_id -- Join to get full name
        WHERE (@InstituteID IS NULL OR n.InstituteID = @InstituteID)
          AND (@StartDate IS NULL OR n.StartDate >= @StartDate)
          AND (@EndDate IS NULL OR n.EndDate <= @EndDate)
          AND n.IsActive = 1";

            // Add condition for Search parameter (filter by Title)
            if (!string.IsNullOrEmpty(request.Search))
            {
                sql += " AND (n.Title LIKE @Search)";
            }

            sql += " ORDER BY n.NoticeID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            // Define parameters for the query
            var parameters = new
            {
                InstituteID = request.InstituteID,
                StartDate = startDate,
                EndDate = endDate,
                Search = "%" + request.Search + "%",
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            // Execute the query to fetch the notices
            var notices = await _connection.QueryAsync<NoticeResponse>(sql, parameters);

            return new ServiceResponse<List<NoticeResponse>>(true, "Records Found", notices.ToList(), 200, totalCount);
        }

        public async Task<ServiceResponse<string>> AddUpdateCircular(AddUpdateCircularRequest request)
        {
            // Convert string dates to DateTime
            DateTime? circularDate = null;
            DateTime? publishedDate = null;
            DateTime? scheduleDate = null;
            TimeSpan? scheduleTime = null;

            if (!string.IsNullOrEmpty(request.CircularDate))
            {
                circularDate = DateTime.ParseExact(request.CircularDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(request.PublishedDate))
            {
                publishedDate = DateTime.ParseExact(request.PublishedDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(request.ScheduleDate))
            {
                scheduleDate = DateTime.ParseExact(request.ScheduleDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(request.ScheduleTime))
            {
                scheduleTime = DateTime.ParseExact(request.ScheduleTime, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
            }

            // Step 1: Insert or update the Circular
            var query = request.CircularID == 0
                ? @"INSERT INTO [tblCircular] 
            (AcademicYear, Title, Message, Attachment, CircularNo, CircularDate, PublishedDate, IsStudent, IsEmployee, ScheduleNow, ScheduleDate, ScheduleTime, InstituteID, CreatedBy, CreatedOn) 
          VALUES 
            (@AcademicYear, @Title, @Message, @Attachment, @CircularNo, @CircularDate, @PublishedDate, @IsStudent, @IsEmployee, @ScheduleNow, @ScheduleDate, @ScheduleTime, @InstituteID, @CreatedBy, GETDATE());
          SELECT CAST(SCOPE_IDENTITY() as int);"
                : @"UPDATE [tblCircular] 
          SET AcademicYear = @AcademicYear, Title = @Title, Message = @Message, Attachment = @Attachment, CircularNo = @CircularNo, CircularDate = @CircularDate, PublishedDate = @PublishedDate,
              IsStudent = @IsStudent, IsEmployee = @IsEmployee, ScheduleNow = @ScheduleNow, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime, 
              InstituteID = @InstituteID, CreatedBy = @CreatedBy
          WHERE CircularID = @CircularID;
          SELECT @CircularID;";

            // Define parameters
            var parameters = new
            {
                request.CircularID,
                request.AcademicYear,
                request.Title,
                request.Message,
                request.Attachment,
                request.CircularNo,
                CircularDate = circularDate,
                PublishedDate = publishedDate,
                request.IsStudent,
                request.IsEmployee,
                request.ScheduleNow,
                ScheduleDate = scheduleDate,
                ScheduleTime = scheduleTime, // Converted to TimeSpan
                request.InstituteID,
                request.CreatedBy
            };

            // Execute the query
            var circularID = await _connection.ExecuteScalarAsync<int>(query, parameters);

            if (circularID > 0)
            {
                // Step 2: Insert into tblCircularStudentMapping if IsStudent is true
                if (request.IsStudent && request.StudentMappings != null && request.StudentMappings.Count > 0)
                {
                    if (request.CircularID != 0)
                    {
                        string deleteStudentMappingSql = "DELETE FROM tblCircularStudentMapping WHERE CircularID = @CircularID";
                        await _connection.ExecuteAsync(deleteStudentMappingSql, new { CircularID = circularID });
                    }

                    string insertStudentMappingSql = "INSERT INTO tblCircularStudentMapping (CircularID, StudentID, ClassID, SectionID) VALUES (@CircularID, @StudentID, @ClassID, @SectionID)";
                    foreach (var studentMapping in request.StudentMappings)
                    {
                        await _connection.ExecuteAsync(insertStudentMappingSql, new { CircularID = circularID, studentMapping.StudentID, studentMapping.ClassID, studentMapping.SectionID });
                    }
                }

                // Step 3: Insert into tblCircularEmployeeMapping if IsEmployee is true
                if (request.IsEmployee && request.EmployeeMappings != null && request.EmployeeMappings.Count > 0)
                {
                    if (request.CircularID != 0)
                    {
                        string deleteEmployeeMappingSql = "DELETE FROM tblCircularEmployeeMapping WHERE CircularID = @CircularID";
                        await _connection.ExecuteAsync(deleteEmployeeMappingSql, new { CircularID = circularID });
                    }

                    string insertEmployeeMappingSql = "INSERT INTO tblCircularEmployeeMapping (CircularID, EmployeeID, DepartmentID, DesignationID) VALUES (@CircularID, @EmployeeID, @DepartmentID, @DesignationID)";
                    foreach (var employeeMapping in request.EmployeeMappings)
                    {
                        await _connection.ExecuteAsync(insertEmployeeMappingSql, new { CircularID = circularID, employeeMapping.EmployeeID, employeeMapping.DepartmentID, employeeMapping.DesignationID });
                    }
                }

                return new ServiceResponse<string>(true, "Operation Successful", "Circular added/updated successfully", 201);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating circular", 400);
            }
        }

        public async Task<ServiceResponse<List<CircularResponse>>> GetAllCircular(GetAllCircularRequest request)
        {
            // Convert string dates to DateTime
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (!string.IsNullOrEmpty(request.StartDate))
            {
                startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(request.EndDate))
            {
                endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            var countSql = "SELECT COUNT(*) FROM [tblCircular] WHERE (@InstituteID IS NULL OR InstituteID = @InstituteID)";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

            var sql = @"
            SELECT 
                c.CircularID,
                c.AcademicYear,
                c.CircularNo,
                c.Title,
                FORMAT(c.CircularDate, 'dd-MM-yyyy') AS CircularDate,
                FORMAT(c.PublishedDate, 'dd-MM-yyyy') AS PublishedDate,
                FORMAT(c.CreatedOn, 'dd-MM-yyyy') AS CreatedOn,
                (e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name) AS CreatedBy, -- Fetch full employee name
                CASE 
                    WHEN c.IsStudent = 1 THEN (
                        SELECT STRING_AGG(CONCAT(cl.class_name, '-', sec.section_name), ', ') 
                        FROM tblCircularStudentMapping csm
                        INNER JOIN tbl_Class cl ON csm.ClassID = cl.class_id
                        INNER JOIN tbl_Section sec ON csm.SectionID = sec.section_id
                        WHERE csm.CircularID = c.CircularID
                    )
                    WHEN c.IsEmployee = 1 THEN (
                        SELECT STRING_AGG(CONCAT(d.DepartmentName, '-', des.DesignationName), ', ') 
                        FROM tblCircularEmployeeMapping cem
                        INNER JOIN tbl_Department d ON cem.DepartmentID = d.Department_id
                        INNER JOIN tbl_Designation des ON cem.DesignationID = des.Designation_id
                        WHERE cem.CircularID = c.CircularID
                    )
                    ELSE ''
                END AS Recipients
            FROM tblCircular c
            LEFT JOIN tbl_EmployeeProfileMaster e ON c.CreatedBy = e.Employee_id -- Join to fetch employee name
            WHERE (@StartDate IS NULL OR c.CircularDate >= @StartDate)
              AND (@EndDate IS NULL OR c.CircularDate <= @EndDate)
              AND (@InstituteID IS NULL OR c.InstituteID = @InstituteID)
              AND c.IsActive = 1
            ORDER BY c.CircularID 
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var parameters = new
            {
                StartDate = startDate,
                EndDate = endDate,
                InstituteID = request.InstituteID,
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var circulars = await _connection.QueryAsync<CircularResponse>(sql, parameters);

            return new ServiceResponse<List<CircularResponse>>(true, "Records Found", circulars.ToList(), 200, totalCount);
        }



        public async Task<ServiceResponse<string>> NoticeSetStudentView(NoticeSetStudentViewRequest request)
        {
            var query = @"
            UPDATE tblNoticeStudentMapping 
            SET IsViewed = 1, 
                ViewedOn = GETDATE()  -- Set current system date
            WHERE NoticeID = @NoticeID 
              AND StudentID = @StudentID";  

            var parameters = new
            {
                request.NoticeID,
                request.StudentID
            };

            var result = await _connection.ExecuteAsync(query, parameters);

            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Operation Successful", "Student view status updated successfully", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Unable to update student view status", 400);
            }
        }
        public async Task<ServiceResponse<string>> NoticeSetEmployeeView(NoticeSetEmployeeViewRequest request)
        {
            var query = @"
        UPDATE tblNoticeEmployeeMapping 
        SET IsViewed = 1, 
            ViewedOn = GETDATE()  -- Set current system date
        WHERE NoticeID = @NoticeID 
          AND EmployeeID = @EmployeeID";

            var parameters = new
            {
                request.NoticeID,
                request.EmployeeID
            };

            var result = await _connection.ExecuteAsync(query, parameters);

            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Operation Successful", "Employee view status updated successfully", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Unable to update employee view status", 400);
            }
        }

        public async Task<ServiceResponse<StudentNoticeStatisticsResponse>> GetStudentNoticeStatistics(GetStudentNoticeStatisticsRequest request)
        {
            // Query to get the total number of students, viewed count, and not viewed count
            var statisticsSql = @"
            SELECT 
                COUNT(*) AS TotalStudent,
                SUM(CASE WHEN ns.IsViewed = 1 THEN 1 ELSE 0 END) AS Viewed,
                SUM(CASE WHEN ns.IsViewed = 0 THEN 1 ELSE 0 END) AS NotViewed
            FROM tblNoticeStudentMapping ns
            INNER JOIN tbl_StudentMaster s ON ns.StudentID = s.student_id
            WHERE ns.NoticeID = @NoticeID AND s.Institute_id = @InstituteID";

            // Query to get the student list details
            var studentListSql = @"
            SELECT 
                CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name, ''), ' ', s.Last_Name) AS StudentName,
                CONCAT(c.class_name, '-', sec.section_name) AS ClassSection,
                s.Admission_Number AS AdmissionNumber,
                CASE 
                    WHEN ns.IsViewed = 1 THEN 
                        FORMAT(ns.ViewedOn, 'dd-MM-yyyy, hh:mm tt') 
                    ELSE NULL 
                END AS ViewedOn
            FROM tblNoticeStudentMapping ns
            INNER JOIN tbl_StudentMaster s ON ns.StudentID = s.student_id
            INNER JOIN tbl_Class c ON ns.ClassID = c.class_id
            INNER JOIN tbl_Section sec ON ns.SectionID = sec.section_id
            WHERE ns.NoticeID = @NoticeID AND s.Institute_id = @InstituteID";

            var parameters = new
            {
                request.InstituteID,
                request.NoticeID
            };

            // Execute both queries
            var statistics = await _connection.QueryFirstOrDefaultAsync<StudentNoticeStatisticsResponse>(statisticsSql, parameters);
            var studentList = await _connection.QueryAsync<StudentNoticeStatisticsResponse.StudentDetails>(studentListSql, parameters);

            if (statistics != null)
            {
                statistics.StudentList = studentList.ToList();
                return new ServiceResponse<StudentNoticeStatisticsResponse>(true, "Records Found", statistics, 200);
            }
            else
            {
                return new ServiceResponse<StudentNoticeStatisticsResponse>(false, "No records found", null, 404);
            }
        }



        public async Task<ServiceResponse<EmployeeNoticeStatisticsResponse>> GetEmployeeNoticeStatistics(GetEmployeeNoticeStatisticsRequest request)
        {
            // Query to get the total number of students, viewed count, and not viewed count
            var statisticsSql = @"
            SELECT 
                COUNT(*) AS TotalStudent,
                SUM(CASE WHEN ne.IsViewed = 1 THEN 1 ELSE 0 END) AS Viewed,
                SUM(CASE WHEN ne.IsViewed = 0 THEN 1 ELSE 0 END) AS NotViewed
            FROM tblNoticeEmployeeMapping ne
            INNER JOIN tbl_EmployeeProfileMaster e ON ne.EmployeeID = e.Employee_id
            WHERE ne.NoticeID = @NoticeID AND e.Institute_id = @InstituteID";

            // Query to get the student list details
            var studentListSql = @"


            SELECT 
                CONCAT(e.First_Name, ' ', ISNULL(e.Middle_Name, ''), ' ', e.Last_Name) AS EmployeeName,
                CONCAT(d.DepartmentName, '-', des.DesignationName) AS DepartmentDesignation,
                e.Employee_code_id AS EmployeeCode,
                CASE 
                    WHEN ne.IsViewed = 1 THEN 
                        FORMAT(ne.ViewedOn, 'dd-MM-yyyy, hh:mm tt') 
                    ELSE NULL 
                END AS ViewedOn
            FROM tblNoticeEmployeeMapping ne
            INNER JOIN tbl_EmployeeProfileMaster e ON ne.EmployeeID = e.Employee_id
            INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
            INNER JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
            WHERE ne.NoticeID = @NoticeID AND e.Institute_id = @InstituteID";

            var parameters = new
            {
                request.InstituteID,
                request.NoticeID
            };

            // Execute both queries
            var statistics = await _connection.QueryFirstOrDefaultAsync<EmployeeNoticeStatisticsResponse>(statisticsSql, parameters);
            var EmployeeList = await _connection.QueryAsync<EmployeeNoticeStatisticsResponse.EmployeeDetail>(studentListSql, parameters);

            if (statistics != null)
            {
                statistics.EmployeeList = EmployeeList.ToList();
                return new ServiceResponse<EmployeeNoticeStatisticsResponse>(true, "Records Found", statistics, 200);
            }
            else
            {
                return new ServiceResponse<EmployeeNoticeStatisticsResponse>(false, "No records found", null, 404);
            }
        }

        //public async Task<ServiceResponse<EmployeeNoticeStatisticsResponse>> GetEmployeeNoticeStatistics(GetEmployeeNoticeStatisticsRequest request)
        //{
        //    // SQL query to get employee statistics
        //    var sql = @"
        //    SELECT 
        //        COUNT(ne.EmployeeID) AS TotalEmployee,
        //        SUM(CASE WHEN ne.IsViewed = 1 THEN 1 ELSE 0 END) AS Viewed,
        //        SUM(CASE WHEN ne.IsViewed = 0 THEN 1 ELSE 0 END) AS NotViewed
        //    FROM tblNoticeEmployeeMapping ne
        //    WHERE ne.NoticeID = @NoticeID AND ne.EmployeeID IN (
        //        SELECT e.Employee_id
        //        FROM tbl_EmployeeMaster e
        //        WHERE e.Institute_id = @InstituteID
        //    );

        //    SELECT 
        //        CONCAT(e.First_Name, ' ', ISNULL(e.Middle_Name, ''), ' ', e.Last_Name) AS EmployeeName,
        //        CONCAT(d.DepartmentName, '-', des.DesignationName) AS DepartmentDesignation,
        //        e.Employee_code_id AS EmployeeCode,
        //        CASE 
        //            WHEN ne.IsViewed = 1 THEN 
        //                FORMAT(ne.ViewedOn, '[dd-MM-yyyy, hh:mm tt]') 
        //            ELSE NULL 
        //        END AS ViewedOn
        //    FROM tblNoticeEmployeeMapping ne
        //    INNER JOIN tbl_EmployeeMaster e ON ne.EmployeeID = e.Employee_id
        //    INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
        //    INNER JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
        //    WHERE ne.NoticeID = @NoticeID AND e.Institute_id = @InstituteID;";

        //    // Execute the queries
        //    using (var multi = await _connection.QueryMultipleAsync(sql, new { request.NoticeID, request.InstituteID }))
        //    {
        //        // First result: Get total, viewed, and not viewed counts
        //        var statistics = await multi.ReadFirstAsync<EmployeeNoticeStatisticsResponse>();

        //        // Second result: Get the list of employee details
        //        statistics.EmployeeList = (await multi.ReadAsync<EmployeeDetail>()).ToList();

        //        return new ServiceResponse<EmployeeNoticeStatisticsResponse>(true, "Statistics retrieved successfully", statistics, 200);
        //    }
        //}


        public async Task<ServiceResponse<string>> DeleteNotice(int InstituteID, int NoticeID)
        {
            string query = "UPDATE [tblNotice] SET IsActive = 0 WHERE InstituteID = @InstituteID AND NoticeID = @NoticeID";
            var result = await _connection.ExecuteAsync(query, new { InstituteID, NoticeID });

            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Notice has been deactivated successfully.", "Success", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Notice deactivation failed.", "Failure", 400);
            }
        }

        public async Task<ServiceResponse<string>> DeleteCircular(DeleteCircularRequest request)
        {
            string query = "UPDATE [tblCircular] SET IsActive = 0 WHERE CircularID = @CircularID AND InstituteID = @InstituteID";
            var result = await _connection.ExecuteAsync(query, new { request.CircularID, request.InstituteID });

            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Circular has been deactivated successfully.", "Success", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Circular deactivation failed.", "Failure", 400);
            }
        }

        public async Task<ServiceResponse<string>> CircularSetStudentView(CircularSetStudentViewRequest request)
        {
            var query = @"
                UPDATE tblCircularStudentMapping 
                SET IsViewed = 1, 
                    ViewedOn = GETDATE()  -- Set current system date/time
                WHERE CircularID = @CircularID 
                  AND StudentID = @StudentID";

            var parameters = new { request.CircularID, request.StudentID };
            var result = await _connection.ExecuteAsync(query, parameters);

            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Operation Successful", "Circular view status updated successfully", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Unable to update circular view status", 400);
            }
        }
        public async Task<ServiceResponse<string>> CircularSetEmployeeView(CircularSetEmployeeViewRequest request)
        {
            var query = @"
                UPDATE tblCircularEmployeeMapping 
                SET IsViewed = 1, 
                    ViewedOn = GETDATE()  -- Set current system date/time
                WHERE CircularID = @CircularID 
                  AND EmployeeID = @EmployeeID";

            var parameters = new { request.CircularID, request.EmployeeID };
            var result = await _connection.ExecuteAsync(query, parameters);

            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Operation Successful", "Circular employee view status updated successfully", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Unable to update circular employee view status", 400);
            }
        }

        public async Task<ServiceResponse<StudentCircularStatisticsResponse>> GetStudentCircularStatistics(GetStudentCircularStatisticsRequest request)
        {
            // Query to get the total number of students, viewed count, and not viewed count
            var statisticsSql = @"
                SELECT 
                    COUNT(*) AS TotalStudent,
                    SUM(CASE WHEN cs.IsViewed = 1 THEN 1 ELSE 0 END) AS Viewed,
                    SUM(CASE WHEN cs.IsViewed = 0 THEN 1 ELSE 0 END) AS NotViewed
                FROM tblCircularStudentMapping cs
                INNER JOIN tbl_StudentMaster s ON cs.StudentID = s.student_id
                WHERE cs.CircularID = @CircularID AND s.Institute_id = @InstituteID";

            // Query to get the student list details
            var studentListSql = @"
                SELECT 
                    CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name, ''), ' ', s.Last_Name) AS StudentName,
                    CONCAT(c.class_name, '-', sec.section_name) AS ClassSection,
                    s.Admission_Number AS AdmissionNumber,
                    CASE 
                        WHEN cs.IsViewed = 1 THEN FORMAT(cs.ViewedOn, 'dd-MM-yyyy, hh:mm tt')
                        ELSE NULL 
                    END AS ViewedOn
                FROM tblCircularStudentMapping cs
                INNER JOIN tbl_StudentMaster s ON cs.StudentID = s.student_id
                INNER JOIN tbl_Class c ON cs.ClassID = c.class_id
                INNER JOIN tbl_Section sec ON cs.SectionID = sec.section_id
                WHERE cs.CircularID = @CircularID AND s.Institute_id = @InstituteID";

            var parameters = new
            {
                request.InstituteID,
                request.CircularID
            };

            // Execute both queries
            var statistics = await _connection.QueryFirstOrDefaultAsync<StudentCircularStatisticsResponse>(statisticsSql, parameters);
            var studentList = await _connection.QueryAsync<StudentCircularStatisticsResponse.StudentDetails>(studentListSql, parameters);

            if (statistics != null)
            {
                statistics.StudentList = studentList.ToList();
                return new ServiceResponse<StudentCircularStatisticsResponse>(true, "Records Found", statistics, 200);
            }
            else
            {
                return new ServiceResponse<StudentCircularStatisticsResponse>(false, "No records found", null, 404);
            }
        }

        public async Task<ServiceResponse<EmployeeCircularStatisticsResponse>> GetEmployeeCircularStatistics(GetEmployeeCircularStatisticsRequest request)
        {
            // Query to get the overall statistics from tblCircularEmployeeMapping
            var statisticsSql = @"
                SELECT 
                    COUNT(*) AS TotalEmployee,
                    SUM(CASE WHEN ce.IsViewed = 1 THEN 1 ELSE 0 END) AS Viewed,
                    SUM(CASE WHEN ce.IsViewed = 0 THEN 1 ELSE 0 END) AS NotViewed
                FROM tblCircularEmployeeMapping ce
                INNER JOIN tbl_EmployeeProfileMaster e ON ce.EmployeeID = e.Employee_id
                WHERE ce.CircularID = @CircularID 
                  AND e.Institute_id = @InstituteID";

            // Query to get the employee list details
            var employeeListSql = @"
                SELECT 
                    CONCAT(e.First_Name, ' ', ISNULL(e.Middle_Name, ''), ' ', e.Last_Name) AS EmployeeName,
                    CONCAT(d.DepartmentName, '-', des.DesignationName) AS DepartmentDesignation,
                    e.Employee_code_id AS EmployeeCode,
                    CASE 
                        WHEN ce.IsViewed = 1 THEN FORMAT(ce.ViewedOn, 'dd-MM-yyyy, hh:mm tt')
                        ELSE NULL 
                    END AS ViewedOn
                FROM tblCircularEmployeeMapping ce
                INNER JOIN tbl_EmployeeProfileMaster e ON ce.EmployeeID = e.Employee_id
                INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
                INNER JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
                WHERE ce.CircularID = @CircularID 
                  AND e.Institute_id = @InstituteID";

            var parameters = new
            {
                request.InstituteID,
                request.CircularID
            };

            // Execute both queries
            var statistics = await _connection.QueryFirstOrDefaultAsync<EmployeeCircularStatisticsResponse>(statisticsSql, parameters);
            var employeeList = await _connection.QueryAsync<EmployeeCircularStatisticsResponse.EmployeeDetail>(employeeListSql, parameters);

            if (statistics != null)
            {
                statistics.EmployeeList = employeeList.ToList();
                return new ServiceResponse<EmployeeCircularStatisticsResponse>(true, "Records Found", statistics, 200);
            }
            else
            {
                return new ServiceResponse<EmployeeCircularStatisticsResponse>(false, "No records found", null, 404);
            }
        }
    }
}
