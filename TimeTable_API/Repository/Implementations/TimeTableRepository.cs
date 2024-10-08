using System.Data;
using System.Threading.Tasks;
using Dapper;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.Repository.Interfaces;

namespace TimeTable_API.Repository.Implementations
{
    public class TimeTableRepository : ITimeTableRepository
    {
        private readonly IDbConnection _connection;

        public TimeTableRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        //public async Task<ServiceResponse<int>> AddUpdateTimeTable(AddUpdateTimeTableRequest request)
        //{
        //    // Ensure the connection is open before starting the transaction
        //    if (_connection.State != ConnectionState.Open)
        //    {
        //        _connection.Open();
        //    }

        //    using (var transaction = _connection.BeginTransaction())
        //    {
        //        try
        //        {
        //            // Step 1: Insert or update the timetable details in tblTimeTableMaster
        //            if (request.TimeTableID == 0)
        //            {
        //                string insertSql = @"
        //            INSERT INTO tblTimeTableMaster (InstituteID, AcademicYearCode, ClassID, SectionID)
        //            VALUES (@InstituteID, @AcademicYearCode, @ClassID, @SectionID);
        //            SELECT CAST(SCOPE_IDENTITY() as int);";

        //                request.TimeTableID = await _connection.ExecuteScalarAsync<int>(insertSql, new
        //                {
        //                    request.InstituteID,
        //                    request.AcademicYearCode,
        //                    request.ClassID,
        //                    request.SectionID
        //                }, transaction);
        //            }
        //            else
        //            {
        //                string updateSql = @"
        //            UPDATE tblTimeTableMaster 
        //            SET InstituteID = @InstituteID, AcademicYearCode = @AcademicYearCode, ClassID = @ClassID, SectionID = @SectionID
        //            WHERE TimeTableID = @TimeTableID";

        //                await _connection.ExecuteAsync(updateSql, new
        //                {
        //                    request.TimeTableID,
        //                    request.InstituteID,
        //                    request.AcademicYearCode,
        //                    request.ClassID,
        //                    request.SectionID
        //                }, transaction);
        //            }

        //            // Step 2: Add or update the days and their sessions/breaks
        //            foreach (var day in request.Days)
        //            {
        //                // Handle breaks
        //                foreach (var breakObj in day.Breaks)
        //                {
        //                    string breakSql = @"
        //                INSERT INTO tblTimeTableMaster (TimeTableID, DayID, BreaksID, IsBreak)
        //                VALUES (@TimeTableID, @DayID, @BreaksID, 1);";

        //                    await _connection.ExecuteAsync(breakSql, new
        //                    {
        //                        TimeTableID = request.TimeTableID,
        //                        DayID = day.DayID, // Ensure DayID is passed here
        //                        BreaksID = breakObj.BreaksID
        //                    }, transaction);
        //                }

        //                // Handle sessions
        //                foreach (var session in day.Sessions)
        //                {
        //                    string sessionSql = @"
        //                INSERT INTO tblTimeTableMaster (TimeTableID, DayID, SessionID, SubjectID, EmployeeID, IsBreak)
        //                VALUES (@TimeTableID, @DayID, @SessionID, @SubjectID, @EmployeeID, 0);";

        //                    await _connection.ExecuteAsync(sessionSql, new
        //                    {
        //                        TimeTableID = request.TimeTableID,
        //                        DayID = day.DayID, // Ensure DayID is passed here
        //                        SessionID = session.SessionID,
        //                        SubjectID = session.SubjectID,
        //                        EmployeeID = session.EmployeeID
        //                    }, transaction);
        //                }
        //            }

        //            transaction.Commit();
        //            return new ServiceResponse<int>(true, "TimeTable added/updated successfully", request.TimeTableID, 200);
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            return new ServiceResponse<int>(false, ex.Message, 0, 500);
        //        }
        //    }
        //}

        public async Task<ServiceResponse<int>> AddUpdateTimeTable(AddUpdateTimeTableRequest request)
        {
            try
            {
                // Ensure the connection is open
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open(); // Use the synchronous Open method
                }

                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        // Step 1: Fetch GroupID based on ClassID and SectionID from tblTimeTableClassSession
                        int groupId = await _connection.ExecuteScalarAsync<int>(
                            @"SELECT GroupID FROM tblTimeTableClassSession 
                    WHERE ClassID = @ClassID AND SectionID = @SectionID",
                            new { request.ClassID, request.SectionID }, transaction);

                        // Step 2: Add or Update tblTimeTableMaster
                        if (request.TimeTableID == 0)
                        {
                            // Insert new TimeTable
                            string insertTimeTableSql = @"
                        INSERT INTO tblTimeTableMaster (AcademicYearCode, ClassID, SectionID, GroupID, InstituteID)
                        VALUES (@AcademicYearCode, @ClassID, @SectionID, @GroupID, @InstituteID);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

                            request.TimeTableID = await _connection.ExecuteScalarAsync<int>(insertTimeTableSql, new
                            {
                                request.AcademicYearCode,
                                request.ClassID,
                                request.SectionID,
                                GroupID = groupId,
                                request.InstituteID
                            }, transaction);
                        }
                        else
                        {
                            // Update existing TimeTable
                            string updateTimeTableSql = @"
                        UPDATE tblTimeTableMaster 
                        SET AcademicYearCode = @AcademicYearCode, ClassID = @ClassID, SectionID = @SectionID, GroupID = @GroupID, InstituteID = @InstituteID
                        WHERE TimeTableID = @TimeTableID";

                            await _connection.ExecuteAsync(updateTimeTableSql, new
                            {
                                request.TimeTableID,
                                request.AcademicYearCode,
                                request.ClassID,
                                request.SectionID,
                                GroupID = groupId,
                                request.InstituteID
                            }, transaction);
                        }

                        // Step 3: Process Days and their associated Sessions and Breaks
                        foreach (var day in request.Days)
                        {
                            // Step 3.1: Process Sessions for the day
                            foreach (var session in day.Sessions)
                            {
                                // Insert session into tblTimeTableSessionMapping
                                string sessionInsertSql = @"
                            INSERT INTO tblTimeTableSessionMapping (SessionID, GroupID, DayID)
                            VALUES (@SessionID, @GroupID, @DayID);
                            SELECT CAST(SCOPE_IDENTITY() as int);";

                                session.TTSessionID = await _connection.ExecuteScalarAsync<int>(sessionInsertSql, new
                                {
                                    session.SessionID,
                                    GroupID = groupId,
                                    day.DayID
                                }, transaction);

                                // Step 4: Insert Employee-Subject Mapping
                                foreach (var empSubject in session.EmployeeSubjects)
                                {
                                    string empSubjectInsertSql = @"
                                INSERT INTO tblTimeTableSessionSubjectEmployee (TTSessionID, SubjectID, EmployeeID)
                                VALUES (@TTSessionID, @SubjectID, @EmployeeID);";

                                    await _connection.ExecuteAsync(empSubjectInsertSql, new
                                    {
                                        TTSessionID = session.TTSessionID,
                                        empSubject.SubjectID,
                                        empSubject.EmployeeID
                                    }, transaction);
                                }
                            }

                            // Step 3.2: Process Breaks for the day
                            foreach (var breakInfo in day.Breaks)
                            {
                                string breakInsertSql = @"
                            INSERT INTO tblTimeTableBreakMapping (BreaksID, GroupID, DayID)
                            VALUES (@BreaksID, @GroupID, @DayID);";

                                await _connection.ExecuteAsync(breakInsertSql, new
                                {
                                    breakInfo.BreaksID,
                                    GroupID = groupId,
                                    day.DayID
                                }, transaction);
                            }
                        }

                        transaction.Commit();
                        return new ServiceResponse<int>(true, "Time Table added/updated successfully", request.TimeTableID, 200);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new ServiceResponse<int>(false, ex.Message, 0, 500);
                    }
                }
            }
            catch (Exception outerEx)
            {
                return new ServiceResponse<int>(false, outerEx.Message, 0, 500);
            }
        }



        public async Task<ServiceResponse<List<TimeTableResponse>>> GetAllTimeTables(GetAllTimeTablesRequest request)
        {
            var response = new ServiceResponse<List<TimeTableResponse>>(
                success: false,
                message: "Initialization failed",
                data: new List<TimeTableResponse>(),
                statusCode: 500,
                totalCount: null
            );

            try
            {
                var timeTables = new List<TimeTableResponse>();

                // Fetch the GroupID based on ClassID and SectionID
                var groupId = await _connection.ExecuteScalarAsync<int>(
                    @"SELECT GroupID FROM tblTimeTableClassSession 
                      WHERE ClassID = @ClassID AND SectionID = @SectionID",
                    new { request.ClassID, request.SectionID });

                // Fetch the PlanID based on the GroupID
                var planId = await _connection.ExecuteScalarAsync<int>(
                    @"SELECT PlanID FROM tblTimeTableDayGroupsMapping 
                      WHERE GroupID = @GroupID",
                    new { GroupID = groupId });

                // Fetch the Days associated with the PlanID
                var days = await _connection.QueryAsync<DayResponse>(
                    @"SELECT value AS DayID, tdm.DayType 
                      FROM tblTimeTableDaySetup tds
                      CROSS APPLY STRING_SPLIT(tds.DayIDs, ',')
                      JOIN tblTimeTableDayMaster tdm ON value = tdm.DayID
                      WHERE tds.PlanID = @PlanID",
                    new { PlanID = planId });

                var timeTableResponse = new TimeTableResponse();

                // Fetch and populate Sessions and Breaks for each day
                //foreach (var day in days)
                //{
                //    var dayResponse = new TimeTableResponse
                //    {
                //        DayID = day.DayID,
                //        DayType = day.DayType,
                //        Sessions = new List<SessionResponse>(),
                //        Breaks = new List<BreakResponse>()
                //    };

                //    // Fetch sessions for the day
                //    var sessions = await _connection.QueryAsync<SessionResponse>(
                //        @"SELECT tsm.SessionID, ts.SessionName
                //          FROM tblTimeTableSessionMapping tsm
                //          JOIN tblTimeTableSessions ts ON tsm.SessionID = ts.SessionID
                //          WHERE tsm.DayID = @DayID AND tsm.GroupID = @GroupID",
                //        new { DayID = day.DayID, GroupID = groupId });

                //    dayResponse.Sessions.AddRange(sessions);

                //    // Fetch Employee-Subject Mapping for each session
                //    foreach (var session in dayResponse.Sessions)
                //    {
                //        var employeeSubjects = await _connection.QueryAsync<EmployeeSubjectResponse>(
                //            @"SELECT tse.SubjectID, sub.SubjectName, tse.EmployeeID, 
                //                     emp.First_Name + ' ' + emp.Last_Name as EmployeeName
                //              FROM tblTimeTableSessionSubjectEmployee tse
                //              JOIN tbl_Subjects sub ON tse.SubjectID = sub.SubjectId
                //              JOIN tbl_EmployeeProfileMaster emp ON tse.EmployeeID = emp.Employee_id
                //              WHERE tse.TTSessionID = @SessionID",
                //            new { SessionID = session.SessionID });

                //        session.EmployeeSubjects.AddRange(employeeSubjects);
                //    }

                //    // Fetch breaks for the day
                //    var breaks = await _connection.QueryAsync<BreakResponse>(
                //        @"SELECT tbm.BreaksID as BreakID, tb.BreaksName as BreakName
                //          FROM tblTimeTableBreakMapping tbm
                //          JOIN tblTimeTableBreaks tb ON tbm.BreaksID = tb.BreaksID
                //          WHERE tbm.DayID = @DayID AND tbm.GroupID = @GroupID",
                //        new { DayID = day.DayID, GroupID = groupId });

                //    dayResponse.Breaks.AddRange(breaks);

                //    timeTables.Add(dayResponse);
                //}



                foreach (var day in days)
                {
                    var dayResponse = new TimeTableResponse
                    {
                        DayID = day.DayID,
                        DayType = day.DayType,
                        Sessions = new List<SessionResponse>(),
                        Breaks = new List<BreakResponse>()
                    };

                    // Fetch sessions for the day
                    var sessions = await _connection.QueryAsync<SessionResponse>(
                        @"SELECT tsm.SessionID, ts.SessionName
          FROM tblTimeTableSessionMapping tsm
          JOIN tblTimeTableSessions ts ON tsm.SessionID = ts.SessionID
          WHERE tsm.DayID = @DayID AND tsm.GroupID = @GroupID",
                        new { DayID = day.DayID, GroupID = groupId });

                    dayResponse.Sessions.AddRange(sessions);

                    // Fetch EmployeeSubjects for each session
                    foreach (var session in dayResponse.Sessions)
                    {
                        //          var employeeSubjects = await _connection.QueryAsync<EmployeeSubjectResponse>(
                        //              @"SELECT tse.SubjectID, sub.SubjectName, tse.EmployeeID, 
                        //       emp.First_Name + ' ' + emp.Last_Name as EmployeeName
                        //FROM tblTimeTableSessionSubjectEmployee tse
                        //JOIN tbl_Subjects sub ON tse.SubjectID = sub.SubjectId
                        //JOIN tbl_EmployeeProfileMaster emp ON tse.EmployeeID = emp.Employee_id
                        //WHERE tse.TTSessionID = @TTSessionID",
                        //              new { TTSessionID = session.SessionID });


                        var employeeSubjects = await _connection.QueryAsync<EmployeeSubjectResponse>(
                            @"SELECT tse.SubjectID, sub.SubjectName, tse.EmployeeID, 
                     emp.First_Name + ' ' + emp.Last_Name as EmployeeName
              FROM tblTimeTableSessionSubjectEmployee tse
			  JOIN tblTimeTableSessionMapping tts ON tse.TTSessionID = tts.TTSessionID
              JOIN tbl_Subjects sub ON tse.SubjectID = sub.SubjectId
              JOIN tbl_EmployeeProfileMaster emp ON tse.EmployeeID = emp.Employee_id
              WHERE tts.SessionID = @TTSessionID",
                            new { TTSessionID = session.SessionID });

                        session.EmployeeSubjects.AddRange(employeeSubjects);
                    }

                    // Fetch breaks for the day
                    var breaks = await _connection.QueryAsync<BreakResponse>(
                        @"SELECT tbm.BreaksID as BreakID, tb.BreaksName as BreakName
          FROM tblTimeTableBreakMapping tbm
          JOIN tblTimeTableBreaks tb ON tbm.BreaksID = tb.BreaksID
          WHERE tbm.DayID = @DayID AND tbm.GroupID = @GroupID",
                        new { DayID = day.DayID, GroupID = groupId });

                    dayResponse.Breaks.AddRange(breaks);

                    timeTables.Add(dayResponse);
                }


                // Set the correct response details on success
                response = new ServiceResponse<List<TimeTableResponse>>(
                    success: true,
                    message: "TimeTables fetched successfully.",
                    data: timeTables,
                    statusCode: 200,
                    totalCount: timeTables.Count
                );
            }
            catch (Exception ex)
            {
                // Set the response details on failure
                response = new ServiceResponse<List<TimeTableResponse>>(
                    success: false,
                    message: ex.Message,
                    data: null,
                    statusCode: 500,
                    totalCount: null
                );
            }

            return response;
        }

        public async Task<ServiceResponse<List<EmployeeResponse>>> GetEmployees(GetInstituteRequest request)
        {
            try
            {
                string sql = @"
            SELECT 
                Employee_id AS EmployeeID,
                First_Name AS FirstName,
                Middle_Name AS MiddleName,
                Last_Name AS LastName,
                mobile_number AS MobileNumber,
                EmailID,
                Employee_code_id AS EmployeeCode,
                Department_id AS DepartmentID,
                Designation_id AS DesignationID,
                EmpPhoto AS EmployeePhoto
            FROM tbl_EmployeeProfileMaster
            WHERE Institute_id = @InstituteID AND Status = 1";

                var employees = await _connection.QueryAsync<EmployeeResponse>(sql, new { request.InstituteID });

                return new ServiceResponse<List<EmployeeResponse>>(true, "Employees retrieved successfully", employees.ToList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeResponse>>(false, ex.Message, new List<EmployeeResponse>(), 500);
            }
        }

        public async Task<ServiceResponse<List<SubjectResponse>>> GetSubjects(GetInstituteRequest request)
        {
            try
            {
                string sql = @"
            SELECT 
                SubjectId AS SubjectID,
                InstituteId,
                SubjectName,
                SubjectCode,
                subject_type_id AS SubjectTypeID
            FROM tbl_Subjects
            WHERE InstituteId = @InstituteID AND IsDeleted = 0";

                var subjects = await _connection.QueryAsync<SubjectResponse>(sql, new { request.InstituteID });

                return new ServiceResponse<List<SubjectResponse>>(true, "Subjects retrieved successfully", subjects.ToList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<SubjectResponse>>(false, ex.Message, new List<SubjectResponse>(), 500);
            }
        }


    }
}
