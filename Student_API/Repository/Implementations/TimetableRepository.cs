using Dapper;
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Models;
using Student_API.Repository.Interfaces;
using System.Data;

namespace Student_API.Repository.Implementations
{
    public class TimetableRepository : ITimetableRepository
    {
        private readonly IDbConnection _connection;

        public TimetableRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<int>> AddUpdateTimeTableGroup(TimeTableGroupDTO timeTableGroupDTO)
        {
            try
            {
                string query;
                int result;


                bool timetableGroupSuccess = false;
                bool periodsSuccess = true;
                bool periodBreaksSuccess = true;

                if (timeTableGroupDTO.TimetableGroup_id == 0)
                {
                    // Add new TimetableGroup
                    query = @"
                INSERT INTO [dbo].[tbl_TimetableGroup] (GroupName, StartTime, EndTime) 
                VALUES (@GroupName, @StartTime, @EndTime);
                SELECT CAST(SCOPE_IDENTITY() as int)";
                    result = await _connection.ExecuteScalarAsync<int>(query, timeTableGroupDTO);
                    timeTableGroupDTO.TimetableGroup_id = result;
                    timetableGroupSuccess = true;
                }
                else
                {
                    // Update existing TimetableGroup
                    query = @"
                UPDATE [dbo].[tbl_TimetableGroup] 
                SET GroupName = @GroupName, StartTime = @StartTime, EndTime = @EndTime 
                WHERE TimetableGroup_id = @TimetableGroupId";
                    result = await _connection.ExecuteAsync(query, timeTableGroupDTO);
                    timetableGroupSuccess = result > 0;
                }

                // Handle Periods
                foreach (var periodDTO in timeTableGroupDTO.periodDTOs)
                {
                    periodDTO.TimetableGroup_id = timeTableGroupDTO.TimetableGroup_id;
                    if (periodDTO.Period_id == 0)
                    {
                        // Add new Period
                        query = @"
                    INSERT INTO [dbo].[tbl_Period] (TimetableGroup_id, PeriodName, StartTime, EndTime) 
                    VALUES (@TimetableGroupId, @PeriodName, @StartTime, @EndTime)";
                    }
                    else
                    {
                        // Update existing Period
                        query = @"
                    UPDATE [dbo].[tbl_Period] 
                    SET PeriodName = @PeriodName, StartTime = @StartTime, EndTime = @EndTime 
                    WHERE Period_id = @PeriodId";
                    }
                    result = await _connection.ExecuteAsync(query, periodDTO);
                    periodsSuccess &= result > 0;
                }

                // Handle PeriodBreaks
                foreach (var periodBreakDTO in timeTableGroupDTO.periodBreakDTOs)
                {
                    periodBreakDTO.TimetableGroup_id = timeTableGroupDTO.TimetableGroup_id;
                    if (periodBreakDTO.PeriodBreak_id == 0)
                    {
                        // Add new PeriodBreak
                        query = @"
                    INSERT INTO [dbo].[tbl_PeriodBreak] (TimetableGroup_id, BreakName, StartTime, EndTime) 
                    VALUES (@TimetableGroupId, @BreakName, @StartTime, @EndTime)";
                    }
                    else
                    {
                        // Update existing PeriodBreak
                        query = @"
                    UPDATE [dbo].[tbl_PeriodBreak] 
                    SET BreakName = @BreakName, StartTime = @StartTime, EndTime = @EndTime 
                    WHERE PeriodBreak_id = @PeriodBreakId";
                    }
                    result = await _connection.ExecuteAsync(query, periodBreakDTO);
                    periodBreaksSuccess &= result > 0;
                }

                bool success = timetableGroupSuccess && periodsSuccess && periodBreaksSuccess;
                int statusCode = success ? 200 : 500;
                string message = success ? "Operation successful" : "Some operations failed";
                return new ServiceResponse<int>(success, message, timeTableGroupDTO.TimetableGroup_id, statusCode);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<List<ResponseTimeTableGroupDTO>>> GetAllTimeTableGroups()
        {
            try
            {
                string query = @"
            SELECT tg.TimetableGroup_id AS TimetableGroupId,
                   tg.GroupName,
                   tg.StartTime,
                   tg.EndTime,
                   COUNT(p.Period_id) AS NumberOfPeriods,
                   COUNT(pb.PeriodBreak_id) AS NumberOfBreaks
            FROM [dbo].[tbl_TimetableGroup] tg
            LEFT JOIN [dbo].[tbl_Period] p ON tg.TimetableGroup_id = p.TimetableGroup_id
            LEFT JOIN [dbo].[tbl_PeriodBreak] pb ON tg.TimetableGroup_id = pb.TimetableGroup_id
            GROUP BY tg.TimetableGroup_id, tg.GroupName, tg.StartTime, tg.EndTime";

                var timetableGroups = await _connection.QueryAsync<ResponseTimeTableGroupDTO>(query);

                return new ServiceResponse<List<ResponseTimeTableGroupDTO>>(true, "Operation successful", timetableGroups.ToList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ResponseTimeTableGroupDTO>>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<TimeTableGroupDTO>> GetTimeTableGroupById(int timetableGroupId)
        {
            try
            {
                string query = "SELECT * FROM [dbo].[tbl_TimetableGroup] WHERE TimetableGroup_id = @TimetableGroupId";
                var timetableGroup = await _connection.QueryFirstOrDefaultAsync<TimeTableGroupDTO>(query, new { TimetableGroupId = timetableGroupId });

                if (timetableGroup != null)
                {
                    timetableGroup.periodDTOs = await GetPeriodsForTimeTableGroup(timetableGroupId);
                    timetableGroup.periodBreakDTOs = await GetPeriodBreaksForTimeTableGroup(timetableGroupId);
                    return new ServiceResponse<TimeTableGroupDTO>(true, "Operation successful", timetableGroup, 200);
                }
                else
                {
                    return new ServiceResponse<TimeTableGroupDTO>(false, "Timetable group not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<TimeTableGroupDTO>(false, ex.Message, null, 500);
            }
        }

        private async Task<List<PeriodDTO>> GetPeriodsForTimeTableGroup(int timetableGroupId)
        {
            string query = "SELECT * FROM [dbo].[tbl_Period] WHERE TimetableGroup_id = @TimetableGroupId";
            return (await _connection.QueryAsync<PeriodDTO>(query, new { TimetableGroupId = timetableGroupId })).ToList();
        }

        private async Task<List<PeriodBreakDTO>> GetPeriodBreaksForTimeTableGroup(int timetableGroupId)
        {
            string query = "SELECT * FROM [dbo].[tbl_PeriodBreak] WHERE TimetableGroup_id = @TimetableGroupId";
            return (await _connection.QueryAsync<PeriodBreakDTO>(query, new { TimetableGroupId = timetableGroupId })).ToList();
        }

        public async Task<ServiceResponse<int>> AddTimeTableDaysPlan(DaysSetupDTO daysSetupDTO)
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                     _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                      
                        string addDaysSetupQuery = @"
                    INSERT INTO [dbo].[tbl_DaysSetup] (PlanName, WorkingDays) 
                    VALUES (@PlanName, @WorkingDays);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                        int daysSetupId = await _connection.ExecuteScalarAsync<int>(addDaysSetupQuery, daysSetupDTO, transaction);

                    
                        if (daysSetupDTO.TimetableGroupIds != null && daysSetupDTO.TimetableGroupIds.Any())
                        {
                            string addDaysGroupMappingQuery = @"
                        INSERT INTO [dbo].[tbl_DaysGroupMapping] (DaysSetup_id, TimetableGroup_id) 
                        VALUES (@DaysSetupId, @TimetableGroupId)";
                            foreach (var timetableGroupId in daysSetupDTO.TimetableGroupIds)
                            {
                                await _connection.ExecuteAsync(addDaysGroupMappingQuery, new { DaysSetupId = daysSetupId, TimetableGroupId = timetableGroupId }, transaction);
                            }
                        }

                 
                        transaction.Commit();

                        return new ServiceResponse<int>(true, "Days plan added successfully", daysSetupId, 200);
                    }
                    catch (Exception ex)
                    {
                
                        transaction.Rollback();
                        return new ServiceResponse<int>(false, ex.Message, 0, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<List<TimeTableDaysPlanDTO>>> GetTimeTableDaysPlan()
        {
            try
            {
                string query = @"
            SELECT ds.DaysSetup_id AS DaysSetupId,
                   ds.PlanName,
                   tg.GroupName AS TimetableGroupName
            FROM [dbo].[tbl_DaysSetup] ds
            LEFT JOIN [dbo].[tbl_DaysGroupMapping] dgm ON ds.DaysSetup_id = dgm.DaysSetup_id
            LEFT JOIN [dbo].[tbl_TimetableGroup] tg ON dgm.TimetableGroup_id = tg.TimetableGroup_id";

                var result = await _connection.QueryAsync<TimeTableDaysPlanDTO>(query);

                var groupedData = result.GroupBy(r => new { r.DaysSetupId, r.PlanName })
                                        .Select(g => new TimeTableDaysPlanDTO
                                        {
                                            DaysSetupId = g.Key.DaysSetupId,
                                            PlanName = g.Key.PlanName,
                                            TimetableGroupNames = g.Select(x => x.TimetableGroupName).ToList()
                                        }).ToList();

                return new ServiceResponse<List<TimeTableDaysPlanDTO>>(true, "Operation successful", groupedData, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<TimeTableDaysPlanDTO>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddOrUpdateTimetable(Timetable timetable)
        {
            try
            {
                string addOrUpdateQuery = @"
            IF @Timetable_id = 0
            BEGIN
                INSERT INTO [dbo].[tbl_Timetable] (TimetableGroup_id, PeriodBreak_id, Period_id, Subject_id, Employee_id, IsBreak, AcademicYear)
                VALUES (@TimetableGroup_id, @PeriodBreak_id, @Period_id, @Subject_id, @Employee_id, @IsBreak, @AcademicYear);
                SELECT CAST(SCOPE_IDENTITY() as int);
            END
            ELSE
            BEGIN
                UPDATE [dbo].[tbl_Timetable]
                SET TimetableGroup_id = @TimetableGroup_id,
                    PeriodBreak_id = @PeriodBreak_id,
                    Period_id = @Period_id,
                    Subject_id = @Subject_id,
                    Employee_id = @Employee_id,
                    IsBreak = @IsBreak,
                    AcademicYear = @AcademicYear
                WHERE Timetable_id = @Timetable_id;
                SELECT @Timetable_id;
            END";

                int insertedOrUpdatedId = await _connection.ExecuteScalarAsync<int>(addOrUpdateQuery, timetable);
                return new ServiceResponse<int>(true, "Operation successful", insertedOrUpdatedId, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<List<Timetable>>> GetTimetablesByTimetableGroupId(int timetableGroupId)
        {
            try
            {
                string query = "SELECT * FROM [dbo].[tbl_Timetable] WHERE TimetableGroup_id = @TimetableGroupId";
                var timetables = await _connection.QueryAsync<Timetable>(query, new { TimetableGroupId = timetableGroupId });

                return new ServiceResponse<List<Timetable>>(true, "Operation successful", timetables.ToList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Timetable>>(false, ex.Message, null, 500);
            }
        }



    }
}
