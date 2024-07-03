﻿using Dapper;
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
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    string query;
                    int result;

                    bool timetableGroupSuccess = false;
                    bool periodsSuccess = true;
                    bool periodBreaksSuccess = true;
                    bool timetableClassMappingsSuccess = false;

                    if (timeTableGroupDTO.TimetableGroup_id == 0)
                    {
                        // Add new TimetableGroup
                        query = @"
                    INSERT INTO [dbo].[tbl_TimetableGroup] (GroupName, StartTime, EndTime) 
                    VALUES (@GroupName, @StartTime, @EndTime);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                        result = await _connection.ExecuteScalarAsync<int>(query, timeTableGroupDTO, transaction);
                        timeTableGroupDTO.TimetableGroup_id = result;
                        timetableGroupSuccess = true;
                    }
                    else
                    {
                        // Update existing TimetableGroup
                        query = @"
                    UPDATE [dbo].[tbl_TimetableGroup] 
                    SET GroupName = @GroupName, StartTime = @StartTime, EndTime = @EndTime 
                    WHERE TimetableGroup_id = @TimetableGroup_id";
                        result = await _connection.ExecuteAsync(query, timeTableGroupDTO, transaction);
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
                        VALUES (@TimetableGroup_id, @PeriodName, @StartTime, @EndTime)";
                        }
                        else
                        {
                            // Update existing Period
                            query = @"
                        UPDATE [dbo].[tbl_Period] 
                        SET PeriodName = @PeriodName, StartTime = @StartTime, EndTime = @EndTime 
                        WHERE Period_id = @PeriodId";
                        }
                        result = await _connection.ExecuteAsync(query, periodDTO, transaction);
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
                        VALUES (@TimetableGroup_id, @BreakName, @StartTime, @EndTime)";
                        }
                        else
                        {
                            // Update existing PeriodBreak
                            query = @"
                        UPDATE [dbo].[tbl_PeriodBreak] 
                        SET BreakName = @BreakName, StartTime = @StartTime, EndTime = @EndTime 
                        WHERE PeriodBreak_id = @PeriodBreakId";
                        }
                        result = await _connection.ExecuteAsync(query, periodBreakDTO, transaction);
                        periodBreaksSuccess &= result > 0;
                    }

                    // Handle TimetableClassMappings
                    foreach (var timetableClassMapping in timeTableGroupDTO.timetableClassMappings)
                    {
                        timetableClassMapping.TimetableGroup_id = timeTableGroupDTO.TimetableGroup_id;
                        if (timetableClassMapping.TimetableClassMapping_id == 0)
                        {
                            // Add new TimetableClassMapping
                            query = @"
                        INSERT INTO [dbo].[tbl_TimetableClassMapping] (TimetableGroup_id, Class_id, Section_id) 
                        VALUES (@TimetableGroup_id, @Class_id, @Section_id)";
                        }
                        else
                        {
                            // Update existing TimetableClassMapping
                            query = @"
                        UPDATE [dbo].[tbl_TimetableClassMapping] 
                        SET Class_id = @Class_id, Section_id = @Section_id 
                        WHERE TimetableClassMapping_id = @TimetableClassMappingId";
                        }
                        result = await _connection.ExecuteAsync(query, timetableClassMapping, transaction);
                        timetableClassMappingsSuccess = result > 0 ? true : false;
                    }

                    bool success = timetableGroupSuccess && periodsSuccess && periodBreaksSuccess && timetableClassMappingsSuccess;
                    if (success)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    int statusCode = success ? 200 : 500;
                    string message = success ? "Operation successful" : "Some operations failed";
                    return new ServiceResponse<int>(success, message, timeTableGroupDTO.TimetableGroup_id, statusCode);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<int>(false, ex.Message, 0, 500);
                }
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
                    timetableGroup.timetableClassMappings = await GetTimetableClassMappingsForTimeTableGroup(timetableGroupId);
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

        private async Task<List<TimetableClassMapping>> GetTimetableClassMappingsForTimeTableGroup(int timetableGroupId)
        {
            string query = "SELECT * FROM [dbo].[tbl_TimetableClassMapping] WHERE TimetableGroup_id = @TimetableGroupId";
            return (await _connection.QueryAsync<TimetableClassMapping>(query, new { TimetableGroupId = timetableGroupId })).ToList();
        }

        public async Task<ServiceResponse<bool>> DeleteTimetableGroup(int timetableGroupId)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Delete from tbl_Period
                    string query = "DELETE FROM [dbo].[tbl_Period] WHERE TimetableGroup_id = @TimetableGroupId";
                    await _connection.ExecuteAsync(query, new { TimetableGroupId = timetableGroupId }, transaction);

                    // Delete from tbl_PeriodBreak
                    query = "DELETE FROM [dbo].[tbl_PeriodBreak] WHERE TimetableGroup_id = @TimetableGroupId";
                    await _connection.ExecuteAsync(query, new { TimetableGroupId = timetableGroupId }, transaction);

                    // Delete from tbl_TimetableClassMapping
                    query = "DELETE FROM [dbo].[tbl_TimetableClassMapping] WHERE TimetableGroup_id = @TimetableGroupId";
                    await _connection.ExecuteAsync(query, new { TimetableGroupId = timetableGroupId }, transaction);

                    // Delete from TimetableGroup
                    query = "DELETE FROM [dbo].[tbl_TimetableGroup] WHERE TimetableGroup_id = @TimetableGroupId";
                    int result = await _connection.ExecuteAsync(query, new { TimetableGroupId = timetableGroupId }, transaction);

                    bool success = result > 0;
                    if (success)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    int statusCode = success ? 200 : 404; // Assuming 404 for not found
                    string message = success ? "TimetableGroup deleted successfully" : "TimetableGroup not found";
                    return new ServiceResponse<bool>(success, message, success, statusCode);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<bool>(false, ex.Message, false, 500);
                }
            }
        }

        public async Task<ServiceResponse<int>> AddOrUpdateTimeTableDaysPlan(DaysSetupDTO daysSetupDTO)
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        int daysSetupId;

                        if (daysSetupDTO.DaysSetupId > 0)
                        {
                            // Update existing record
                            daysSetupId = daysSetupDTO.DaysSetupId;

                            string updateDaysSetupQuery = @"
                        UPDATE [dbo].[tbl_DaysSetup]
                        SET PlanName = @PlanName, WorkingDays = @WorkingDays
                        WHERE DaysSetup_id = @DaysSetupId";

                            await _connection.ExecuteAsync(updateDaysSetupQuery, new
                            {
                                PlanName = daysSetupDTO.PlanName,
                                WorkingDays = daysSetupDTO.WorkingDays,
                                DaysSetupId = daysSetupId
                            }, transaction);

                            // Retrieve existing mappings
                            string selectExistingMappingsQuery = @"
                        SELECT TimetableGroup_id
                        FROM [dbo].[tbl_DaysGroupMapping]
                        WHERE DaysSetup_id = @DaysSetupId";

                            var existingMappings = await _connection.QueryAsync<int>(selectExistingMappingsQuery, new { DaysSetupId = daysSetupId }, transaction);

                            // Convert existing mappings to HashSet for quick lookup
                            var existingMappingsSet = new HashSet<int>(existingMappings);

                            // Determine new mappings to add
                            var newMappingsToAdd = daysSetupDTO.TimetableGroupIds.Except(existingMappingsSet);

                            // Insert new mappings
                            string addDaysGroupMappingQuery = @"
                        INSERT INTO [dbo].[tbl_DaysGroupMapping] (DaysSetup_id, TimetableGroup_id) 
                        VALUES (@DaysSetupId, @TimetableGroupId)";

                            foreach (var timetableGroupId in newMappingsToAdd)
                            {
                                await _connection.ExecuteAsync(addDaysGroupMappingQuery, new
                                {
                                    DaysSetupId = daysSetupId,
                                    TimetableGroupId = timetableGroupId
                                }, transaction);
                            }

                            // Determine mappings to remove (if any)
                            var mappingsToRemove = existingMappingsSet.Except(daysSetupDTO.TimetableGroupIds);

                            // Remove mappings that are no longer needed
                            if (mappingsToRemove.Any())
                            {
                                string deleteMappingsQuery = @"
                            DELETE FROM [dbo].[tbl_DaysGroupMapping]
                            WHERE DaysSetup_id = @DaysSetupId AND TimetableGroup_id IN @TimetableGroupIds";

                                await _connection.ExecuteAsync(deleteMappingsQuery, new
                                {
                                    DaysSetupId = daysSetupId,
                                    TimetableGroupIds = mappingsToRemove.ToList()
                                }, transaction);
                            }
                        }
                        else
                        {
                            // Insert new record
                            string addDaysSetupQuery = @"
                        INSERT INTO [dbo].[tbl_DaysSetup] (PlanName, WorkingDays) 
                        VALUES (@PlanName, @WorkingDays);
                        SELECT CAST(SCOPE_IDENTITY() as int)";

                            daysSetupId = await _connection.ExecuteScalarAsync<int>(addDaysSetupQuery, new
                            {
                                PlanName = daysSetupDTO.PlanName,
                                WorkingDays = daysSetupDTO.WorkingDays
                            }, transaction);

                            // Insert mappings for the new record
                            string addDaysGroupMappingQuery = @"
                        INSERT INTO [dbo].[tbl_DaysGroupMapping] (DaysSetup_id, TimetableGroup_id) 
                        VALUES (@DaysSetupId, @TimetableGroupId)";

                            foreach (var timetableGroupId in daysSetupDTO.TimetableGroupIds)
                            {
                                await _connection.ExecuteAsync(addDaysGroupMappingQuery, new
                                {
                                    DaysSetupId = daysSetupId,
                                    TimetableGroupId = timetableGroupId
                                }, transaction);
                            }
                        }

                        transaction.Commit();
                        return new ServiceResponse<int>(true, "Days plan added or updated successfully", daysSetupId, 200);
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
        public async Task<ServiceResponse<DaysSetupDTO>> GetDaysSetupById(int daysSetupId)
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                // Query to fetch DaysSetup details and WorkingDays
                string query = @"
            SELECT DaysSetup_id, PlanName, WorkingDays
            FROM [dbo].[tbl_DaysSetup]
            WHERE DaysSetup_id = @DaysSetupId;

            SELECT TimetableGroup_id
            FROM [dbo].[tbl_DaysGroupMapping]
            WHERE DaysSetup_id = @DaysSetupId";

                using (var multi = await _connection.QueryMultipleAsync(query, new { DaysSetupId = daysSetupId }))
                {
                    // Read DaysSetup details
                    var daysSetup = await multi.ReadSingleOrDefaultAsync<DaysSetupDTO>();

                    if (daysSetup == null)
                    {
                        return new ServiceResponse<DaysSetupDTO>(false, "Days setup not found", null, 404);
                    }

                    // Read TimetableGroup mappings
                    var timetableGroupIds = await multi.ReadAsync<int>();

                    //if (!string.IsNullOrEmpty(daysSetup.WorkingDays))
                    //{
                    //    var workingDaysArray = daysSetup.WorkingDays.Split(',').Select(int.Parse).ToList();
                    //    daysSetup.WorkingDaysList = workingDaysArray;
                    //}
                    //else
                    //{
                    //    daysSetup.WorkingDaysList = new List<int>();
                    //}


                    // Assign TimetableGroup_ids to the DTO
                    daysSetup.TimetableGroupIds = timetableGroupIds.ToList();

                    return new ServiceResponse<DaysSetupDTO>(true, "Days setup retrieved successfully", daysSetup, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<DaysSetupDTO>(false, ex.Message, null, 500);
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
                INSERT INTO [dbo].[tbl_Timetable] (TimetableGroup_id, PeriodBreak_id, Period_id, Subject_id, Employee_id, IsBreak, AcademicYear, Class_id,Section_id)
                VALUES (@TimetableGroup_id, @PeriodBreak_id, @Period_id, @Subject_id, @Employee_id, @IsBreak, @AcademicYear, @Class_id, @Section_id);
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
                    AcademicYear = @AcademicYear,
                    Class_id = @Class_id,
                    Section_id = @Section_id
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

        public async Task<ServiceResponse<List<Timetable>>> GetTimetablesByCriteria(string academicYear, int classId, int sectionId)
        {
            try
            {
                string query = @"
            SELECT * 
            FROM [dbo].[tbl_Timetable] 
            WHERE AcademicYear = @AcademicYear 
            AND Class_id = @ClassId 
            AND Section_id = @SectionId";

                var timetables = await _connection.QueryAsync<Timetable>(
                    query,
                    new { AcademicYear = academicYear, ClassId = classId, SectionId = sectionId }
                );

                return new ServiceResponse<List<Timetable>>(true, "Operation successful", timetables.ToList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Timetable>>(false, ex.Message, null, 500);
            }
        }

        public async Task<List<ClassTimetableData>> GetClassTimetableData(int dayId, string academicYear)
        {

            string query = @"
            SELECT 
                t.Class_id AS ClassId,
                c.Class_Name AS ClassName,
                COUNT(DISTINCT t.Period_id) AS Sessions,
                COUNT(DISTINCT t.Subject_id) AS Subjects
            FROM [dbo].[tbl_Timetable] t
            JOIN [dbo].[tbl_Class] c ON t.Class_id = c.Class_id
            WHERE t.Day_Id = @DayId
              AND t.AcademicYear = @AcademicYear
            GROUP BY t.Class_id, c.Class_Name
            ORDER BY t.Class_id";

            var classTimetableData = await _connection.QueryAsync<ClassTimetableData>(
                query,
                new { DayId = dayId, AcademicYear = academicYear }
            );

            return classTimetableData.ToList();

        }
        public async Task<ServiceResponse<ClassDayWiseDTO>> GetClassDayWiseData(int classId, int sectionId, string academicYear)
        {
            try
            {
                string query = @"SELECT 
                    t.Day_Id AS DayId,
                    d.Day_Name AS DayName,
                    t.Period_id AS PeriodId,
                    p.PeriodName,
                    t.Subject_id AS SubjectId,
                    s.SubjectName,
                    t.Employee_id AS EmployeeId,
                    e.First_Name AS EmployeeName
                FROM [dbo].[tbl_Timetable] t
                JOIN [dbo].[tbl_Day] d ON t.Day_Id = d.Day_Id
                JOIN [dbo].[tbl_Period] p ON t.Period_id = p.Period_id
                JOIN [dbo].[tbl_Subject] s ON t.Subject_id = s.Subject_id
                JOIN [dbo].[tbl_EmployeeProfileMaster] e ON t.Employee_id = e.Employee_id
                WHERE t.Class_id = @ClassId
                  AND t.Section_id = @SectionId
                  AND t.AcademicYear = @AcademicYear
                ORDER BY t.Day_Id, p.StartTime";
                var classDayWiseData = await _connection.QueryAsync<ClassDayWiseDetailDTO>(query,
                    new { ClassId = classId, SectionId = sectionId, AcademicYear = academicYear });
                var sessionsPerWeek = classDayWiseData.GroupBy(d => d.DayId).Count();
                var subjectsPerWeek = classDayWiseData.GroupBy(d => d.SubjectId)
                                                      .Select(g => new SubjectCountDTO
                                                      {
                                                          SubjectId = g.Key,
                                                          SubjectName = g.First().SubjectName,
                                                          Count = g.Count()
                                                      }).ToList();

                var result = new ClassDayWiseDTO
                {
                    ClassId = classId,
                    SectionId = sectionId,
                    AcademicYear = academicYear,
                    SessionsPerWeek = sessionsPerWeek,
                    SubjectsPerWeek = subjectsPerWeek,
                    DayWiseDetails = classDayWiseData.ToList()
                };
                return new ServiceResponse<ClassDayWiseDTO>(true, "Operation successful", result, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ClassDayWiseDTO>(false, ex.Message, null, 500);
            }
        }
    }
}
