using Communication_API.DTOs.ServiceResponse;

using Communication_API.DTOs.Requests.DiscussionBoard;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.DiscussionBoard;
using Communication_API.Repository.Interfaces.DiscussionBoard;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Communication_API.DTOs.Responses.DiscussionBoard;

namespace Communication_API.Repository.Implementations.DiscussionBoard
{
    public class DiscussionBoardRepository : IDiscussionBoardRepository
    {
        private readonly IDbConnection _connection;

        public DiscussionBoardRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateDiscussion(AddUpdateDiscussionRequest request)
        {
            // Convert string dates to DateTime objects
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

            // Determine if this is an insert (DiscussionBoardID == 0) or update
            bool isInsert = request.DiscussionBoardID == 0;
            string query = isInsert
                ? @"INSERT INTO tblDiscussionBoard 
                    (InstituteID, DiscussionHeading, Description, Attachments, StartDate, EndDate, IsStudent, IsEmployee, CreatedBy, CreatedOn)
                   VALUES 
                    (@InstituteID, @DiscussionHeading, @Description, @Attachments, @StartDate, @EndDate, @IsStudent, @IsEmployee, @CreatedBy, GETDATE());
                   SELECT CAST(SCOPE_IDENTITY() as int);"
                : @"UPDATE tblDiscussionBoard 
                    SET InstituteID = @InstituteID, DiscussionHeading = @DiscussionHeading, Description = @Description, Attachments = @Attachments, 
                        StartDate = @StartDate, EndDate = @EndDate, IsStudent = @IsStudent, IsEmployee = @IsEmployee, CreatedBy = @CreatedBy
                    WHERE DiscussionBoardID = @DiscussionBoardID;
                    SELECT @DiscussionBoardID;";

            var parameters = new
            {
                request.InstituteID,
                request.DiscussionBoardID,
                request.DiscussionHeading,
                request.Description,
                request.Attachments,
                StartDate = startDate,
                EndDate = endDate,
                request.IsStudent,
                request.IsEmployee,
                request.CreatedBy
            };

            int discussionBoardID;
            try
            {
                discussionBoardID = await _connection.ExecuteScalarAsync<int>(query, parameters);
            }
            catch (Exception ex)
            {
                // Optionally, log the exception here
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating discussion board", 400);
            }

            if (discussionBoardID > 0)
            {
                // Process Student Mappings
                if (request.IsStudent && request.StudentMappings != null && request.StudentMappings.Any())
                {
                    // If updating, delete existing mappings
                    if (!isInsert)
                    {
                        string deleteStudentMappingSql = "DELETE FROM tblDiscussionBoardStudentMapping WHERE DiscussionBoardID = @DiscussionBoardID";
                        await _connection.ExecuteAsync(deleteStudentMappingSql, new { DiscussionBoardID = discussionBoardID });
                    }

                    // Insert new student mappings
                    string insertStudentMappingSql = "INSERT INTO tblDiscussionBoardStudentMapping (DiscussionBoardID, ClassID, SectionID, StudentID) VALUES (@DiscussionBoardID, @ClassID, @SectionID, @StudentID)";
                    foreach (var studentMapping in request.StudentMappings)
                    {
                        await _connection.ExecuteAsync(insertStudentMappingSql, new
                        {
                            DiscussionBoardID = discussionBoardID,
                            studentMapping.ClassID,
                            studentMapping.SectionID,
                            studentMapping.StudentID
                        });
                    }
                }

                // Process Employee Mappings
                if (request.IsEmployee && request.EmployeeMappings != null && request.EmployeeMappings.Any())
                {
                    // If updating, delete existing mappings
                    if (!isInsert)
                    {
                        string deleteEmployeeMappingSql = "DELETE FROM tblDiscussionBoardEmployeeMapping WHERE DiscussionBoardID = @DiscussionBoardID";
                        await _connection.ExecuteAsync(deleteEmployeeMappingSql, new { DiscussionBoardID = discussionBoardID });
                    }

                    // Insert new employee mappings
                    string insertEmployeeMappingSql = "INSERT INTO tblDiscussionBoardEmployeeMapping (DiscussionBoardID, DepartmentID, DesignationID, EmployeeID) VALUES (@DiscussionBoardID, @DepartmentID, @DesignationID, @EmployeeID)";
                    foreach (var employeeMapping in request.EmployeeMappings)
                    {
                        await _connection.ExecuteAsync(insertEmployeeMappingSql, new
                        {
                            DiscussionBoardID = discussionBoardID,
                            employeeMapping.DepartmentID,
                            employeeMapping.DesignationID,
                            employeeMapping.EmployeeID
                        });
                    }
                }

                return new ServiceResponse<string>(true, "Operation Successful", "Discussion board added/updated successfully", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating discussion board", 400);
            }
        }


        public async Task<ServiceResponse<List<GetAllDiscussionResponse>>> GetAllDiscussion(GetAllDiscussionRequest request)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;

            // Convert incoming date strings to DateTime (if provided)
            if (!string.IsNullOrEmpty(request.StartDate))
            {
                startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(request.EndDate))
            {
                endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            // Build the SQL query.
            // Here we filter discussions by the InstituteID and (optionally) by the CreatedOn date range.
            string query = @"SELECT DiscussionBoardID, DiscussionHeading, CreatedOn 
                             FROM tblDiscussionBoard 
                             WHERE InstituteID = @InstituteID";
            if (startDate.HasValue && endDate.HasValue)
            {
                query += " AND CreatedOn BETWEEN @startDate AND @endDate";
            }

            // Execute the query using Dapper.
            var discussionList = await _connection.QueryAsync<DiscussionTemp>(
                query,
                new { InstituteID = request.InstituteID, startDate, endDate }
            );

            // Map results and format the CreatedOn date.
            var responseData = discussionList.Select(d => new GetAllDiscussionResponse
            {
                DiscussionBoardID = d.DiscussionBoardID,
                DiscussionHeading = d.DiscussionHeading,
                CreatedOn = FormatCreatedOn(d.CreatedOn)
            }).ToList();

            return new ServiceResponse<List<GetAllDiscussionResponse>>(true, "Operation Successful", responseData, 200);
        }

        // A temporary class to help with mapping the database results.
        private class DiscussionTemp
        {
            public int DiscussionBoardID { get; set; }
            public string DiscussionHeading { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        // Formats the CreatedOn date as "26th Apr 2024, 07:00 PM"
        private string FormatCreatedOn(DateTime createdOn)
        {
            int day = createdOn.Day;
            string ordinal = GetOrdinal(day);
            return $"{day}{ordinal} {createdOn.ToString("MMM yyyy, hh:mm tt")}";
        }

        // Returns the ordinal suffix for a given day.
        private string GetOrdinal(int day)
        {
            if (day % 100 is 11 or 12 or 13)
            {
                return "th";
            }
            return (day % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            };
        }

        public async Task<ServiceResponse<string>> DeleteDiscussion(int DiscussionBoardID)
        {
            var query = "DELETE FROM [tblDiscussionBoard] WHERE DiscussionBoardID = @DiscussionBoardID";
            var result = await _connection.ExecuteAsync(query, new { DiscussionBoardID });
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 200 : 400);
        }

        public async Task<ServiceResponse<GetDiscussionBoardDetailsResponse>> GetDiscussionBoardDetails(GetDiscussionBoardDetailsRequest request)
        {
            // 1. Retrieve the main discussion board record
            string mainQuery = @"
                SELECT DiscussionBoardID, StartDate, EndDate, CreatedOn, CreatedBy 
                FROM tblDiscussionBoard 
                WHERE DiscussionBoardID = @DiscussionBoardID AND InstituteID = @InstituteID";
            var discussion = await _connection.QuerySingleOrDefaultAsync<DiscussionBoardMain>(
                mainQuery,
                new { request.DiscussionBoardID, request.InstituteID }
            );
            if (discussion == null)
            {
                return new ServiceResponse<GetDiscussionBoardDetailsResponse>(
                    false, "No discussion board found", null, 404
                );
            }

            // Format the date range from StartDate to EndDate (e.g., "26-04-2024 to 28-04-2024")
            string dateRange = $"{discussion.StartDate.ToString("dd-MM-yyyy")} to {discussion.EndDate.ToString("dd-MM-yyyy")}";

            // 2. Retrieve student mappings and join with tbl_Class and tbl_Section
            string studentQuery = @"
                SELECT c.class_name, s.section_name 
                FROM tblDiscussionBoardStudentMapping sbsm
                JOIN tbl_Class c ON sbsm.ClassID = c.class_id
                JOIN tbl_Section s ON sbsm.SectionID = s.section_id
                WHERE sbsm.DiscussionBoardID = @DiscussionBoardID";
            var studentMappings = await _connection.QueryAsync<StudentMappingTemp>(
                studentQuery,
                new { request.DiscussionBoardID }
            );
            string studentFormatted = string.Join(", ", studentMappings.Select(sm => $"{sm.class_name}-{sm.section_name}"));

            // 3. Retrieve employee mappings and join with tbl_Department and tbl_Designation
            string employeeQuery = @"
                SELECT d.DepartmentName, ds.DesignationName 
                FROM tblDiscussionBoardEmployeeMapping sbe
                JOIN tbl_Department d ON sbe.DepartmentID = d.Department_id
                JOIN tbl_Designation ds ON sbe.DesignationID = ds.Designation_id
                WHERE sbe.DiscussionBoardID = @DiscussionBoardID";
            var employeeMappings = await _connection.QueryAsync<EmployeeMappingTemp>(
                employeeQuery,
                new { request.DiscussionBoardID }
            );
            string employeeFormatted = string.Join(", ", employeeMappings.Select(em => $"{em.DepartmentName}-{em.DesignationName}"));

            // 4. Retrieve the creator's name from tbl_EmployeeProfileMaster using the CreatedBy value
            string creatorQuery = @"
                SELECT First_Name, Middle_Name, Last_Name 
                FROM tbl_EmployeeProfileMaster 
                WHERE Employee_id = @CreatedBy";
            var creator = await _connection.QuerySingleOrDefaultAsync<EmployeeNameTemp>(
                creatorQuery,
                new { CreatedBy = discussion.CreatedBy }
            );
            string createdByName = creator != null
                ? $"{creator.First_Name} {creator.Middle_Name} {creator.Last_Name}".Replace("  ", " ").Trim()
                : "";

            // 5. Format the CreatedOn date as "26th Apr 2024, 07:00 PM"
            string createdOnFormatted = FormatCreatedOn(discussion.CreatedOn);

            // Build the response DTO
            var responseData = new GetDiscussionBoardDetailsResponse
            {
                Date = dateRange,
                Student = studentFormatted,
                Employee = employeeFormatted,
                CreatedBy = createdByName,
                CreatedOn = createdOnFormatted
            };

            return new ServiceResponse<GetDiscussionBoardDetailsResponse>(
                true, "Operation Successful", responseData, 200
            );
        }

        // Helper method to format the CreatedOn date
 
        // Temporary classes for mapping query results
        private class DiscussionBoardMain
        {
            public int DiscussionBoardID { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime CreatedOn { get; set; }
            public int CreatedBy { get; set; }
        }

        private class StudentMappingTemp
        {
            public string class_name { get; set; }
            public string section_name { get; set; }
        }

        private class EmployeeMappingTemp
        {
            public string DepartmentName { get; set; }
            public string DesignationName { get; set; }
        }

        private class EmployeeNameTemp
        {
            public string First_Name { get; set; }
            public string Middle_Name { get; set; }
            public string Last_Name { get; set; }
        }

        public async Task<ServiceResponse<string>> CreateDiscussionThread(CreateDiscussionThreadRequest request)
        {
            var query = "INSERT INTO [tblDiscussionBoardComment] (DiscussionBoardID, UserID, Comments, CommentDate) VALUES (@DiscussionBoardID, @UserID, @Comments, @CommentDate)";

            var parameters = new
            {
                request.DiscussionBoardID,
                request.UserID,
                request.Comments,
                CommentDate = DateTime.Now
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<List<DiscussionThread>>> GetDiscussionThread(int DiscussionBoardID)
        {
            var query = "SELECT * FROM [tblDiscussionBoardComment] WHERE DiscussionBoardID = @DiscussionBoardID";
            var comments = await _connection.QueryAsync<DiscussionThread>(query, new { DiscussionBoardID });
            return new ServiceResponse<List<DiscussionThread>>(true, "Records Found", comments.ToList(), 302);
        }

        public async Task<ServiceResponse<string>> AddDiscussionBoardComment(AddDiscussionBoardCommentRequest request)
        {
            // SQL query to insert a new comment. 
            // It assumes that CreatedOn is auto-populated using GETDATE() on insert.
            var query = @"
                INSERT INTO tblDiscussionBoardComment (DiscussionBoardID, UserID, UserTypeID, Comments, CreatedOn)
                VALUES (@DiscussionBoardID, @UserID, @UserTypeID, @Comments, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var parameters = new
            {
                request.DiscussionBoardID,
                request.UserID,
                request.UserTypeID,
                request.Comments
            };

            try
            {
                // Execute the query and retrieve the new CommentID
                int commentID = await _connection.ExecuteScalarAsync<int>(query, parameters);

                if (commentID > 0)
                {
                    return new ServiceResponse<string>(
                        true,
                        "Operation Successful",
                        "Discussion board comment added successfully",
                        200
                    );
                }
                else
                {
                    return new ServiceResponse<string>(
                        false,
                        "Operation Failed",
                        "Error adding discussion board comment",
                        400
                    );
                }
            }
            catch (Exception ex)
            {
                // Optionally, log the exception here
                return new ServiceResponse<string>(
                    false,
                    "Operation Failed",
                    "Error adding discussion board comment",
                    400
                );
            }
        }

        public async Task<ServiceResponse<string>> AddDiscussionBoardReaction(AddDiscussionBoardReactionRequest request)
        {
            // Step 1: Remove any existing reaction for the same DiscussionBoardID, UserID, and UserTypeID
            var deleteQuery = @"
        DELETE FROM tblDiscussionBoardReaction
        WHERE DiscussionBoardID = @DiscussionBoardID 
          AND UserID = @UserID 
          AND UserTypeID = @UserTypeID;
    ";

            var deleteParameters = new
            {
                request.DiscussionBoardID,
                request.UserID,
                request.UserTypeID
            };

            await _connection.ExecuteAsync(deleteQuery, deleteParameters);

            // Step 2: Insert the new reaction
            var query = @"
        INSERT INTO tblDiscussionBoardReaction (DiscussionBoardID, Reaction, UserID, UserTypeID)
        VALUES (@DiscussionBoardID, @Reaction, @UserID, @UserTypeID);
        SELECT CAST(SCOPE_IDENTITY() as int);
    ";

            var parameters = new
            {
                request.DiscussionBoardID,
                request.Reaction,
                request.UserID,
                request.UserTypeID
            };

            try
            {
                int reactionID = await _connection.ExecuteScalarAsync<int>(query, parameters);

                if (reactionID > 0)
                {
                    return new ServiceResponse<string>(true, "Operation Successful", "Reaction added successfully", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation Failed", "Error adding reaction", 400);
                }
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding reaction", 400);
            }
        }

        public async Task<ServiceResponse<List<GetDiscussionBoardCommentsResponse>>> GetDiscussionBoardComments(GetDiscussionBoardCommentsRequest request)
        {
            // The query returns comments from employees (UserTypeID = 1) and from students (UserTypeID = 2)
            string query = @"
                SELECT 
                    c.UserTypeID,
                    c.UserID,
                    CONCAT(e.First_Name, ' ', ISNULL(e.Middle_Name, ''), ' ', e.Last_Name) AS UserName,
                    CONCAT(d.DepartmentName, '-', ds.DesignationName) AS Category,
                    c.Comments AS Comment
                FROM tblDiscussionBoardComment c
                JOIN tbl_EmployeeProfileMaster e ON c.UserID = e.Employee_id
                JOIN tbl_Department d ON e.Department_id = d.Department_id
                JOIN tbl_Designation ds ON e.Designation_id = ds.Designation_id
                WHERE c.DiscussionBoardID = @DiscussionBoardID AND c.UserTypeID = 1

                UNION ALL

                SELECT 
                    c.UserTypeID,
                    c.UserID,
                    CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name, ''), ' ', s.Last_Name) AS UserName,
                    CONCAT(cl.class_name, '-', sec.section_name) AS Category,
                    c.Comments AS Comment
                FROM tblDiscussionBoardComment c
                JOIN tbl_StudentMaster s ON c.UserID = s.student_id
                JOIN tbl_Class cl ON s.class_id = cl.class_id
                JOIN tbl_Section sec ON s.section_id = sec.section_id
                WHERE c.DiscussionBoardID = @DiscussionBoardID AND c.UserTypeID = 2;
            ";

            var parameters = new { DiscussionBoardID = request.DiscussionBoardID };

            try
            {
                var comments = (await _connection.QueryAsync<GetDiscussionBoardCommentsResponse>(query, parameters)).ToList();
                return new ServiceResponse<List<GetDiscussionBoardCommentsResponse>>(
                    true, "Operation Successful", comments, 200
                );
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return new ServiceResponse<List<GetDiscussionBoardCommentsResponse>>(
                    false, "Operation Failed", null, 400
                );
            }
        }

        public async Task<ServiceResponse<GetDiscussionBoardReactionsResponse>> GetDiscussionBoardReactions(GetDiscussionBoardReactionsRequest request)
        {
            // This query returns the like count, dislike count, and comments count for the specified DiscussionBoardID.
            string query = @"
                SELECT 
                    (SELECT COUNT(*) 
                     FROM tblDiscussionBoardReaction 
                     WHERE DiscussionBoardID = @DiscussionBoardID AND Reaction = 1) AS LikesCount,
                    (SELECT COUNT(*) 
                     FROM tblDiscussionBoardReaction 
                     WHERE DiscussionBoardID = @DiscussionBoardID AND Reaction = 0) AS DisLikesCount,
                    (SELECT COUNT(*) 
                     FROM tblDiscussionBoardComment 
                     WHERE DiscussionBoardID = @DiscussionBoardID) AS CommentsCounts
            ";

            var parameters = new { DiscussionBoardID = request.DiscussionBoardID };

            try
            {
                var result = await _connection.QuerySingleOrDefaultAsync<GetDiscussionBoardReactionsResponse>(query, parameters);

                if (result != null)
                {
                    return new ServiceResponse<GetDiscussionBoardReactionsResponse>(
                        true,
                        "Operation Successful",
                        result,
                        200
                    );
                }
                else
                {
                    // If no records found, return zero counts.
                    var emptyResponse = new GetDiscussionBoardReactionsResponse
                    {
                        LikesCount = 0,
                        DisLikesCount = 0,
                        CommentsCounts = 0
                    };

                    return new ServiceResponse<GetDiscussionBoardReactionsResponse>(
                        true,
                        "No data found",
                        emptyResponse,
                        200
                    );
                }
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return new ServiceResponse<GetDiscussionBoardReactionsResponse>(
                    false,
                    "Operation Failed",
                    null,
                    400
                );
            }
        }

    }
}
