using Dapper;
using System.Data;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.Models;
using TimeTable_API.Repository.Interfaces;

namespace TimeTable_API.Repository.Implementations
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IDbConnection _connection;

        public GroupRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<string>> AddUpdateGroup(AddUpdateGroupRequest request)
        {
            // Open the connection explicitly
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Step 1: Add or update Group
                    int groupId = request.GroupID ?? 0;
                    if (groupId == 0)
                    {
                        // Insert new group
                        string query = @"INSERT INTO tblTimeTableGroups (GroupName, StartTime, EndTime, InstituteID, IsActive) 
                                 VALUES (@GroupName, @StartTime, @EndTime, @InstituteID, @IsActive);
                                 SELECT CAST(SCOPE_IDENTITY() as int);";
                        groupId = await _connection.ExecuteScalarAsync<int>(query, new
                        {
                            request.GroupName,
                            request.StartTime,
                            request.EndTime,
                            request.InstituteID,
                            request.IsActive
                        }, transaction);
                    }
                    else
                    {
                        // Update existing group
                        string query = @"UPDATE tblTimeTableGroups 
                                 SET GroupName = @GroupName, StartTime = @StartTime, EndTime = @EndTime, InstituteID = @InstituteID, IsActive = @IsActive 
                                 WHERE GroupID = @GroupID";
                        await _connection.ExecuteAsync(query, new
                        {
                            request.GroupID,
                            request.GroupName,
                            request.StartTime,
                            request.EndTime,
                            request.InstituteID,
                            request.IsActive
                        }, transaction);
                    }

                    // Step 2: Add or update Sessions
                    foreach (var session in request.Sessions)
                    {
                        if (session.SessionID == null || session.SessionID == 0)
                        {
                            // Insert new session
                            string sessionQuery = @"INSERT INTO tblTimeTableSessions (GroupID, SessionName, StartTime, EndTime, IsActive) 
                                            VALUES (@GroupID, @SessionName, @StartTime, @EndTime, 1)";
                            await _connection.ExecuteAsync(sessionQuery, new
                            {
                                GroupID = groupId,
                                session.SessionName,
                                session.StartTime,
                                session.EndTime
                            }, transaction);
                        }
                        else
                        {
                            // Update existing session
                            string sessionQuery = @"UPDATE tblTimeTableSessions 
                                            SET SessionName = @SessionName, StartTime = @StartTime, EndTime = @EndTime 
                                            WHERE SessionID = @SessionID AND GroupID = @GroupID";
                            await _connection.ExecuteAsync(sessionQuery, new
                            {
                                session.SessionID,
                                GroupID = groupId,
                                session.SessionName,
                                session.StartTime,
                                session.EndTime
                            }, transaction);
                        }
                    }

                    // Step 3: Add or update Breaks
                    foreach (var breaks in request.Breaks)
                    {
                        if (breaks.BreaksID == null || breaks.BreaksID == 0)
                        {
                            // Insert new break
                            string breakQuery = @"INSERT INTO tblTimeTableBreaks (GroupID, BreaksName, StartTime, EndTime, IsActive) 
                                          VALUES (@GroupID, @BreakName, @StartTime, @EndTime, 1)";
                            await _connection.ExecuteAsync(breakQuery, new
                            {
                                GroupID = groupId,
                                BreakName = breaks.BreakName,
                                breaks.StartTime,
                                breaks.EndTime
                            }, transaction);
                        }
                        else
                        {
                            // Update existing break
                            string breakQuery = @"UPDATE tblTimeTableBreaks 
                                          SET BreaksName = @BreakName, StartTime = @StartTime, EndTime = @EndTime 
                                          WHERE BreaksID = @BreaksID AND GroupID = @GroupID";
                            await _connection.ExecuteAsync(breakQuery, new
                            {
                                breaks.BreaksID,
                                GroupID = groupId,
                                BreakName = breaks.BreakName,
                                breaks.StartTime,
                                breaks.EndTime
                            }, transaction);
                        }
                    }

                    // Step 4: Add Class Sections
                    foreach (var classSection in request.ClassSections)
                    {
                        string classSectionQuery = @"INSERT INTO tblTimeTableClassSession (GroupID, ClassID, SectionID, IsActive) 
                                             VALUES (@GroupID, @ClassID, @SectionID, 1)";
                        await _connection.ExecuteAsync(classSectionQuery, new
                        {
                            GroupID = groupId,
                            classSection.ClassID,
                            classSection.SectionID
                        }, transaction);
                    }

                    transaction.Commit();

                    return new ServiceResponse<string>(true, "Group and related data added/updated successfully", "Success", StatusCodes.Status200OK);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<string>(false, ex.Message, string.Empty, StatusCodes.Status500InternalServerError);
                }
            }
        }
        public async Task<ServiceResponse<List<GroupResponse>>> GetAllGroups(GetAllGroupsRequest request)
        {
            try
            {
                // Query to get the total count of groups for pagination
                string countSql = @"
            SELECT COUNT(*) 
            FROM tblTimeTableGroups 
            WHERE IsActive = 1 AND InstituteID = @InstituteID";

                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                // Updated SQL Query with InstituteID filter
                string sql = @"
            SELECT 
                g.GroupID,
                g.GroupName,
                g.StartTime,
                g.EndTime,
                (SELECT COUNT(*) FROM tblTimeTableSessions s WHERE s.GroupID = g.GroupID AND s.IsActive = 1) AS NumberOfSessions,
                (SELECT COUNT(*) FROM tblTimeTableBreaks b WHERE b.GroupID = g.GroupID AND b.IsActive = 1) AS NumberOfBreaks
            FROM tblTimeTableGroups g
            WHERE g.IsActive = 1 AND g.InstituteID = @InstituteID
            ORDER BY g.GroupID
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                // Execute query to get group data with pagination and filtering
                var groupData = (await _connection.QueryAsync<GroupResponse>(sql, new
                {
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize,
                    request.InstituteID
                })).ToList();

                // Format StartEndTime correctly
                foreach (var group in groupData)
                {
                    // Convert TimeSpan to DateTime for AM/PM formatting
                    if (group.StartTime.HasValue && group.EndTime.HasValue)
                    {
                        DateTime startDateTime = DateTime.Today.Add(group.StartTime.Value);
                        DateTime endDateTime = DateTime.Today.Add(group.EndTime.Value);
                        group.StartEndTime = $"{startDateTime:hh:mm tt} - {endDateTime:hh:mm tt}";
                    }
                    else
                    {
                        group.StartEndTime = "N/A";
                    }

                    // Fetch ClassSection data for each group
                    string classSectionSql = @"
                SELECT 
                    cs.ClassID,
                    cs.SectionID,
                    c.class_name AS ClassName,
                    s.section_name AS SectionName
                FROM tblTimeTableClassSession cs
                JOIN tbl_Class c ON cs.ClassID = c.class_id
                JOIN tbl_Section s ON cs.SectionID = s.section_id
                WHERE cs.GroupID = @GroupID AND cs.IsActive = 1";

                    var classSections = await _connection.QueryAsync<ClassSectionResponse>(classSectionSql, new
                    {
                        GroupID = group.GroupID
                    });

                    group.ClassSections = classSections.ToList();
                }

                // Pass totalCount in ServiceResponse
                return new ServiceResponse<List<GroupResponse>>(true, "Groups retrieved successfully", groupData, StatusCodes.Status200OK, totalCount);
            }
            catch (Exception ex)
            {
                // Return error response if any exception occurs
                return new ServiceResponse<List<GroupResponse>>(false, ex.Message, new List<GroupResponse>(), StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ServiceResponse<GroupResponse>> GetGroupById(int groupId)
        {
            try
            {
                // Fetch group data by GroupID
                string sql = @"
                SELECT 
                    g.GroupID,
                    g.GroupName,
                    g.StartTime,
                    g.EndTime,
                    (SELECT COUNT(*) FROM tblTimeTableSessions s WHERE s.GroupID = g.GroupID AND s.IsActive = 1) AS NumberOfSessions,
                    (SELECT COUNT(*) FROM tblTimeTableBreaks b WHERE b.GroupID = g.GroupID AND b.IsActive = 1) AS NumberOfBreaks
                FROM tblTimeTableGroups g
                WHERE g.IsActive = 1 AND g.GroupID = @GroupID";

                // Execute query to get the group
                var groupData = await _connection.QuerySingleOrDefaultAsync<GroupResponse>(sql, new { GroupID = groupId });

                if (groupData != null)
                {
                    // Format StartEndTime
                    if (groupData.StartTime.HasValue && groupData.EndTime.HasValue)
                    {
                        DateTime startDateTime = DateTime.Today.Add(groupData.StartTime.Value);
                        DateTime endDateTime = DateTime.Today.Add(groupData.EndTime.Value);
                        groupData.StartEndTime = $"{startDateTime:hh:mm tt} - {endDateTime:hh:mm tt}";
                    }

                    // Fetch ClassSection data for the group
                    string classSectionSql = @"
                    SELECT 
                        cs.ClassID,
                        cs.SectionID,
                        c.class_name AS ClassName,
                        s.section_name AS SectionName
                    FROM tblTimeTableClassSession cs
                    JOIN tbl_Class c ON cs.ClassID = c.class_id
                    JOIN tbl_Section s ON cs.SectionID = s.section_id
                    WHERE cs.GroupID = @GroupID AND cs.IsActive = 1";

                    var classSections = await _connection.QueryAsync<ClassSectionResponse>(classSectionSql, new { GroupID = groupId });
                    groupData.ClassSections = classSections.ToList();
                }

                return new ServiceResponse<GroupResponse>(true, "Group retrieved successfully", groupData, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GroupResponse>(false, ex.Message, null, StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<ServiceResponse<bool>> DeleteGroup(int groupId)
        {
            try
            {
                string sql = @"UPDATE tblTimeTableGroups SET IsActive = 0 WHERE GroupID = @GroupID";
                int rowsAffected = await _connection.ExecuteAsync(sql, new { GroupID = groupId });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Group deleted successfully", true, StatusCodes.Status200OK);
                }
                return new ServiceResponse<bool>(false, "Operation failed", false, StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
    }
}
