using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.Repository.Interfaces; 

namespace TimeTable_API.Repository.Implementations
{
    public class DaySetupRepository : IDaySetupRepository
    {
        private readonly IDbConnection _connection;

        public DaySetupRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<DaySetupResponse>>> GetAllDaySetups(GetAllDaySetupsRequest request)
        {
            try
            {
                // Query to get all day setups based on InstituteID and pagination
                string sql = @"
                SELECT PlanID, PlanName 
                FROM tblTimeTableDaySetup
                WHERE InstituteID = @InstituteID AND IsActive = 1
                ORDER BY PlanID
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var daySetups = (await _connection.QueryAsync<DaySetupResponse>(sql, new
                {
                    InstituteID = request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                })).ToList();

                // Fetch groups mapped to each day setup
                foreach (var daySetup in daySetups)
                {
                    string mappedGroupsSql = @"
                    SELECT 
                        g.GroupID,
                        g.GroupName
                    FROM tblTimeTableDayGroupsMapping dgm
                    JOIN tblTimeTableGroups g ON dgm.GroupID = g.GroupID
                    WHERE dgm.PlanID = @PlanID";

                    var mappedGroups = await _connection.QueryAsync<GroupMappingResponse>(mappedGroupsSql, new
                    {
                        PlanID = daySetup.PlanID
                    });

                    daySetup.MappedTo = mappedGroups.ToList();
                }

                return new ServiceResponse<List<DaySetupResponse>>(true, "Day setups retrieved successfully", daySetups, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<DaySetupResponse>>(false, ex.Message, new List<DaySetupResponse>(), 500);
            }
        }

        public async Task<ServiceResponse<DaySetupResponse>> GetDaySetupById(int planId)
        {
            try
            {
                // Query to get the plan based on PlanID
                string sql = @"
                SELECT PlanID, PlanName 
                FROM tblTimeTableDaySetup
                WHERE PlanID = @PlanID";

                var daySetup = await _connection.QueryFirstOrDefaultAsync<DaySetupResponse>(sql, new
                {
                    PlanID = planId
                });

                if (daySetup == null)
                {
                    return new ServiceResponse<DaySetupResponse>(false, "Day setup not found", null, 404);
                }

                // Fetch groups mapped to the plan
                string mappedGroupsSql = @"
                SELECT 
                    g.GroupID,
                    g.GroupName
                FROM tblTimeTableDayGroupsMapping dgm
                JOIN tblTimeTableGroups g ON dgm.GroupID = g.GroupID
                WHERE dgm.PlanID = @PlanID";

                var mappedGroups = await _connection.QueryAsync<GroupMappingResponse>(mappedGroupsSql, new
                {
                    PlanID = daySetup.PlanID
                });

                daySetup.MappedTo = mappedGroups.ToList();

                return new ServiceResponse<DaySetupResponse>(true, "Day setup retrieved successfully", daySetup, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<DaySetupResponse>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddUpdatePlan(AddUpdatePlanRequest request)
        {
            // Open the connection if it is not already open
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Step 1: Add or update Plan
                    if (request.PlanID == 0)
                    {
                        // Insert new Plan
                        string insertSql = @"
                INSERT INTO tblTimeTableDaySetup (PlanName, DayIDs, InstituteID, IsActive)
                VALUES (@PlanName, @DayIDs, @InstituteID, 1);
                SELECT CAST(SCOPE_IDENTITY() as int);";

                        request.PlanID = await _connection.ExecuteScalarAsync<int>(insertSql, new
                        {
                            request.PlanName,
                            request.DayIDs,
                            request.InstituteID
                        }, transaction);
                    }
                    else
                    {
                        // Update existing Plan
                        string updateSql = @"
                UPDATE tblTimeTableDaySetup 
                SET PlanName = @PlanName, DayIDs = @DayIDs, InstituteID = @InstituteID
                WHERE PlanID = @PlanID";

                        await _connection.ExecuteAsync(updateSql, new
                        {
                            request.PlanID,
                            request.PlanName,
                            request.DayIDs,
                            request.InstituteID
                        }, transaction);
                    }

                    // Step 2: Remove any existing group mappings that are not in the request
                    string deleteGroupMappingsSql = @"
            DELETE FROM tblTimeTableDayGroupsMapping
            WHERE PlanID = @PlanID AND GroupID NOT IN @GroupIDs";

                    var groupIdsToKeep = request.Groups.Select(g => g.GroupID).ToList();
                    await _connection.ExecuteAsync(deleteGroupMappingsSql, new { request.PlanID, GroupIDs = groupIdsToKeep }, transaction);

                    // Step 3: Add or update Groups in tblTimeTableDayGroupsMapping
                    foreach (var group in request.Groups)
                    {
                        // Ensure group.PlanID is set to the current PlanID
                        group.PlanID = request.PlanID;

                        string insertGroupSql = @"
                IF NOT EXISTS (SELECT 1 FROM tblTimeTableDayGroupsMapping WHERE GroupID = @GroupID AND PlanID = @PlanID)
                BEGIN
                    INSERT INTO tblTimeTableDayGroupsMapping (GroupID, PlanID)
                    VALUES (@GroupID, @PlanID)
                END";

                        await _connection.ExecuteAsync(insertGroupSql, group, transaction);
                    }

                    transaction.Commit();
                    return new ServiceResponse<int>(true, "Plan and Groups added/updated successfully", request.PlanID, 200);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<int>(false, ex.Message, 0, 500);
                }
                finally
                {
                    // Ensure the connection is closed properly after the transaction
                    if (_connection.State == ConnectionState.Open)
                        _connection.Close();
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteDaySetup(int planId)
        {
            string sql = "UPDATE tblTimeTableDaySetup SET IsActive = 0 WHERE PlanID = @PlanID";
            await _connection.ExecuteAsync(sql, new { PlanID = planId });
            return new ServiceResponse<bool>(true, "Day Setup deleted successfully", true, 200);
        }
    }
}
