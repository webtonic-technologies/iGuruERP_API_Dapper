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
                scheduleTime = DateTime.ParseExact(request.ScheduleTime, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            // Step 1: Insert or update the Notice
            var query = request.NoticeID == 0
                ? @"INSERT INTO [tblNotice] 
                  (InstituteID, Title, Description, Attachments, StartDate, EndDate, IsStudent, IsEmployee, ScheduleNow, ScheduleDate, ScheduleTime) 
               VALUES 
                  (@InstituteID, @Title, @Description, @Attachments, @StartDate, @EndDate, @IsStudent, @IsEmployee, @ScheduleNow, @ScheduleDate, @ScheduleTime);
               SELECT CAST(SCOPE_IDENTITY() as int);"  // Return the newly inserted NoticeID
                                : @"UPDATE [tblNotice] 
               SET InstituteID = @InstituteID, Title = @Title, Description = @Description, Attachments = @Attachments, StartDate = @StartDate, EndDate = @EndDate, 
                   IsStudent = @IsStudent, IsEmployee = @IsEmployee, ScheduleNow = @ScheduleNow, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime 
               WHERE NoticeID = @NoticeID;
               SELECT @NoticeID;"; // Return the existing NoticeID in case of update

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
                ScheduleTime = scheduleTime
            };

            // Execute the query and get the NoticeID
            var noticeID = await _connection.ExecuteScalarAsync<int>(query, parameters);

            if (noticeID > 0)
            {
                // Step 2: Insert into tblNoticeStudentMapping if IsStudent is true
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


        //public async Task<ServiceResponse<string>> AddUpdateNotice(AddUpdateNoticeRequest request)
        //{
        //    // Step 1: Insert or update the Notice
        //    var query = request.NoticeID == 0
        //        ? @"INSERT INTO [tblNotice] 
        //          (InstituteID, Title, Description, Attachments, StartDate, EndDate, IsStudent, IsEmployee, ScheduleNow, ScheduleDate, ScheduleTime) 
        //       VALUES 
        //          (@InstituteID, @Title, @Description, @Attachments, @StartDate, @EndDate, @IsStudent, @IsEmployee, @ScheduleNow, @ScheduleDate, @ScheduleTime);
        //       SELECT CAST(SCOPE_IDENTITY() as int);"  // Return the newly inserted NoticeID
        //                : @"UPDATE [tblNotice] 
        //       SET InstituteID = @InstituteID, Title = @Title, Description = @Description, Attachments = @Attachments, StartDate = @StartDate, EndDate = @EndDate, 
        //           IsStudent = @IsStudent, IsEmployee = @IsEmployee, ScheduleNow = @ScheduleNow, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime 
        //       WHERE NoticeID = @NoticeID;
        //       SELECT @NoticeID;"; // Return the existing NoticeID in case of update

        //    // Define the parameters for the query
        //    var parameters = new
        //    {
        //        request.InstituteID,
        //        request.NoticeID,
        //        request.Title,
        //        request.Description,
        //        request.Attachments,
        //        request.StartDate,
        //        request.EndDate,
        //        request.IsStudent,
        //        request.IsEmployee,
        //        request.ScheduleNow,
        //        request.ScheduleDate,
        //        request.ScheduleTime
        //    };

        //    // Execute the query and get the NoticeID
        //    var noticeID = await _connection.ExecuteScalarAsync<int>(query, parameters);

        //    if (noticeID > 0)
        //    {
        //        // Step 2: Insert into tblNoticeStudentMapping if IsStudent is true
        //        if (request.IsStudent && request.StudentMappings != null && request.StudentMappings.Count > 0)
        //        {
        //            // Delete existing student mappings if updating
        //            if (request.NoticeID != 0)
        //            {
        //                string deleteStudentMappingSql = "DELETE FROM tblNoticeStudentMapping WHERE NoticeID = @NoticeID";
        //                await _connection.ExecuteAsync(deleteStudentMappingSql, new { NoticeID = noticeID });
        //            }

        //            // Insert new student mappings
        //            string insertStudentMappingSql = "INSERT INTO tblNoticeStudentMapping (NoticeID, StudentID, ClassID, SectionID) VALUES (@NoticeID, @StudentID, @ClassID, @SectionID)";
        //            foreach (var studentMapping in request.StudentMappings)
        //            {
        //                await _connection.ExecuteAsync(insertStudentMappingSql, new { NoticeID = noticeID, studentMapping.StudentID, studentMapping.ClassID, studentMapping.SectionID });
        //            }
        //        }

        //        // Step 3: Insert into tblNoticeEmployeeMapping if IsEmployee is true
        //        if (request.IsEmployee && request.EmployeeMappings != null && request.EmployeeMappings.Count > 0)
        //        {
        //            // Delete existing employee mappings if updating
        //            if (request.NoticeID != 0)
        //            {
        //                string deleteEmployeeMappingSql = "DELETE FROM tblNoticeEmployeeMapping WHERE NoticeID = @NoticeID";
        //                await _connection.ExecuteAsync(deleteEmployeeMappingSql, new { NoticeID = noticeID });
        //            }

        //            // Insert new employee mappings
        //            string insertEmployeeMappingSql = "INSERT INTO tblNoticeEmployeeMapping (NoticeID, EmployeeID, DepartmentID, DesignationID) VALUES (@NoticeID, @EmployeeID, @DepartmentID, @DesignationID)";
        //            foreach (var employeeMapping in request.EmployeeMappings)
        //            {
        //                await _connection.ExecuteAsync(insertEmployeeMappingSql, new { NoticeID = noticeID, employeeMapping.EmployeeID, employeeMapping.DepartmentID, employeeMapping.DesignationID });
        //            }
        //        }

        //        return new ServiceResponse<string>(true, "Operation Successful", "Notice added/updated successfully", 200);
        //    }
        //    else
        //    {
        //        return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating notice", 400);
        //    }
        //}


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
                            n.StartDate,
                            n.EndDate,
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
                        WHERE (@InstituteID IS NULL OR n.InstituteID = @InstituteID)
                          AND (@StartDate IS NULL OR n.StartDate >= @StartDate)
                          AND (@EndDate IS NULL OR n.EndDate <= @EndDate)";

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
            // Step 1: Insert or update the Circular
            var query = request.CircularID == 0
                ? @"INSERT INTO [tblCircular] 
            (Title, Message, Attachment, CircularNo, CircularDate, PublishedDate, IsStudent, IsEmployee, ScheduleNow, ScheduleDate, ScheduleTime, InstituteID) 
            VALUES 
            (@Title, @Message, @Attachment, @CircularNo, @CircularDate, @PublishedDate, @IsStudent, @IsEmployee, @ScheduleNow, @ScheduleDate, @ScheduleTime, @InstituteID);
            SELECT CAST(SCOPE_IDENTITY() as int);"  // Return the newly inserted CircularID
                : @"UPDATE [tblCircular] 
            SET Title = @Title, Message = @Message, Attachment = @Attachment, CircularNo = @CircularNo, CircularDate = @CircularDate, PublishedDate = @PublishedDate,
            IsStudent = @IsStudent, IsEmployee = @IsEmployee, ScheduleNow = @ScheduleNow, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime, InstituteID = @InstituteID
            WHERE CircularID = @CircularID;
            SELECT @CircularID;"; // Return the existing CircularID in case of update

            // Define the parameters for the query
            var parameters = new
            {
                request.CircularID,
                request.Title,
                request.Message,
                request.Attachment,
                request.CircularNo,
                request.CircularDate,
                request.PublishedDate,
                request.IsStudent,
                request.IsEmployee,
                request.ScheduleNow,
                request.ScheduleDate,
                request.ScheduleTime,
                request.InstituteID
            };

            // Execute the query and get the CircularID
            var circularID = await _connection.ExecuteScalarAsync<int>(query, parameters);

            if (circularID > 0)
            {
                // Step 2: Insert into tblCircularStudentMapping if IsStudent is true
                if (request.IsStudent && request.StudentMappings != null && request.StudentMappings.Count > 0)
                {
                    // Delete existing student mappings if updating
                    if (request.CircularID != 0)
                    {
                        string deleteStudentMappingSql = "DELETE FROM tblCircularStudentMapping WHERE CircularID = @CircularID";
                        await _connection.ExecuteAsync(deleteStudentMappingSql, new { CircularID = circularID });
                    }

                    // Insert new student mappings
                    string insertStudentMappingSql = "INSERT INTO tblCircularStudentMapping (CircularID, StudentID, ClassID, SectionID) VALUES (@CircularID, @StudentID, @ClassID, @SectionID)";
                    foreach (var studentMapping in request.StudentMappings)
                    {
                        await _connection.ExecuteAsync(insertStudentMappingSql, new { CircularID = circularID, studentMapping.StudentID, studentMapping.ClassID, studentMapping.SectionID });
                    }
                }

                // Step 3: Insert into tblCircularEmployeeMapping if IsEmployee is true
                if (request.IsEmployee && request.EmployeeMappings != null && request.EmployeeMappings.Count > 0)
                {
                    // Delete existing employee mappings if updating
                    if (request.CircularID != 0)
                    {
                        string deleteEmployeeMappingSql = "DELETE FROM tblCircularEmployeeMapping WHERE CircularID = @CircularID";
                        await _connection.ExecuteAsync(deleteEmployeeMappingSql, new { CircularID = circularID });
                    }

                    // Insert new employee mappings
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
            var countSql = "SELECT COUNT(*) FROM [tblCircular] WHERE (@InstituteID IS NULL OR InstituteID = @InstituteID)";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

            var sql = @"
            SELECT 
                c.CircularID,
                c.AcademicYear,
                c.CircularNo,
                c.Title,
                c.CircularDate,
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
            WHERE (@StartDate IS NULL OR c.CircularDate >= @StartDate)
              AND (@EndDate IS NULL OR c.CircularDate <= @EndDate)
              AND (@InstituteID IS NULL OR c.InstituteID = @InstituteID)
            ORDER BY c.CircularID 
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var parameters = new
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                InstituteID = request.InstituteID,
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var circulars = await _connection.QueryAsync<CircularResponse>(sql, parameters);

            return new ServiceResponse<List<CircularResponse>>(true, "Records Found", circulars.ToList(), 302, totalCount);
        }


        public async Task<ServiceResponse<string>> NoticeSetStudentView(NoticeSetStudentViewRequest request)
        {
            var query = "UPDATE tblNoticeStudentMapping SET IsViewed = 1 WHERE NoticeID = @NoticeID AND StudentID = @StudentID";

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
            var query = "UPDATE tblNoticeEmployeeMapping SET IsViewed = 1 WHERE NoticeID = @NoticeID AND EmployeeID = @EmployeeID";

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
                s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name AS StudentName,
                CONCAT(c.class_name, '-', sec.section_name) AS ClassSection,
                s.Admission_Number as AdmissionNumber,
                CASE WHEN ns.IsViewed = 1 THEN GETDATE() ELSE NULL END AS ViewedOn
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
            // SQL query to get employee statistics
            var sql = @"
            SELECT 
                COUNT(ne.EmployeeID) AS TotalEmployee,
                SUM(CASE WHEN ne.IsViewed = 1 THEN 1 ELSE 0 END) AS Viewed,
                SUM(CASE WHEN ne.IsViewed = 0 THEN 1 ELSE 0 END) AS NotViewed
            FROM tblNoticeEmployeeMapping ne
            WHERE ne.NoticeID = @NoticeID AND ne.EmployeeID IN (
                SELECT e.Employee_id
                FROM tbl_EmployeeMaster e
                WHERE e.Institute_id = @InstituteID
            );

            SELECT 
                e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name AS EmployeeName,
                CONCAT(d.DepartmentName, '-', des.DesignationName) AS DepartmentDesignation,
                e.Employee_code_id AS EmployeeCode,
                null as ViewedOn
            FROM tblNoticeEmployeeMapping ne
            INNER JOIN tbl_EmployeeMaster e ON ne.EmployeeID = e.Employee_id
            INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
            INNER JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
            WHERE ne.NoticeID = @NoticeID AND e.Institute_id = @InstituteID;";

            // Execute the queries
            using (var multi = await _connection.QueryMultipleAsync(sql, new { request.NoticeID, request.InstituteID }))
            {
                // First result: Get total, viewed, and not viewed counts
                var statistics = await multi.ReadFirstAsync<EmployeeNoticeStatisticsResponse>();

                // Second result: Get the list of employee details
                statistics.EmployeeList = (await multi.ReadAsync<EmployeeDetail>()).ToList();

                return new ServiceResponse<EmployeeNoticeStatisticsResponse>(true, "Statistics retrieved successfully", statistics, 200);
            }
        }

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


    }
}
