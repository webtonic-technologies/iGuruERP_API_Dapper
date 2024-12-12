using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.Repository.Interfaces;

namespace TimeTable_API.Repository.Implementations
{
    public class ClassWiseRepository : IClassWiseRepository
    {
        private readonly IDbConnection _connection;

        public ClassWiseRepository(IDbConnection connection)
        {
            _connection = connection;
        }


        public async Task<ServiceResponse<ClassWiseResponse>> GetClassWiseTimeTables(ClassWiseRequest request)
        {
            var response = new ServiceResponse<ClassWiseResponse>(
                success: false,
                message: "Initialization failed",
                data: new ClassWiseResponse(),
                statusCode: 500
            );

            try
            {
                var classWiseResponse = new ClassWiseResponse();

                // Fetch all classes for the given InstituteID
                var classes = await _connection.QueryAsync<ClassDetail>(
                    @"SELECT class_id as ClassID, class_name as ClassName
              FROM tbl_Class
              WHERE institute_id = @InstituteID",
                    new { request.InstituteID });

                foreach (var classDetail in classes)
                {
                    // Fetch sections for each class
                    var sections = await _connection.QueryAsync<SectionDetail>(
                        @"SELECT section_id as SectionID, section_name as SectionName
                  FROM tbl_Section
                  WHERE class_id = @ClassID",
                        new { ClassID = classDetail.ClassID });

                    foreach (var section in sections)
                    {
                        // Get GroupID by ClassID, SectionID, and AcademicYearCode
                        var groupId = await _connection.ExecuteScalarAsync<int?>(
                            @"SELECT tcs.GroupID FROM tblTimeTableClassSession tcs
                      JOIN tblTimeTableMaster tm ON tcs.GroupID = tm.GroupID
                      WHERE tcs.ClassID = @ClassID 
                        AND tcs.SectionID = @SectionID
                        AND tm.AcademicYearCode = @AcademicYearCode",
                            new { ClassID = classDetail.ClassID, SectionID = section.SectionID, AcademicYearCode = request.AcademicYearCode });

                        // If GroupID is found, calculate NumberOfSessions and NumberOfSubjects
                        if (groupId.HasValue)
                        {
                            // Count Number of Sessions for the group
                            section.NumberOfSessions = await _connection.ExecuteScalarAsync<int>(
                                @"SELECT COUNT(*) FROM tblTimeTableSessions
                          WHERE GroupID = @GroupID",
                                new { GroupID = groupId });

                            // Count Number of Subjects for the group
                            section.NumberOfSubjects = await _connection.ExecuteScalarAsync<int>(
                                @"SELECT COUNT(DISTINCT tse.SubjectID)
                          FROM tblTimeTableSessionMapping tsm
                          JOIN tblTimeTableSessionSubjectEmployee tse ON tsm.TTSessionID = tse.TTSessionID
                          WHERE tsm.GroupID = @GroupID",
                                new { GroupID = groupId });
                        }
                    }

                    classDetail.SectionList.AddRange(sections);
                    classWiseResponse.ClassList.Add(classDetail);
                }

                // Set the response as successful
                response = new ServiceResponse<ClassWiseResponse>(
                    success: true,
                    message: "Class and Section details fetched successfully.",
                    data: classWiseResponse,
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                // Set response in case of failure
                response = new ServiceResponse<ClassWiseResponse>(
                    success: false,
                    message: ex.Message,
                    data: null,
                    statusCode: 500
                );
            }

            return response;
        }


        //public async Task<ServiceResponse<ClassWiseResponse>> GetClassWiseTimeTables(ClassWiseRequest request)
        //{
        //    var response = new ServiceResponse<ClassWiseResponse>(
        //        success: false,
        //        message: "Initialization failed",
        //        data: new ClassWiseResponse(),
        //        statusCode: 500
        //    );

        //    try
        //    {
        //        var classWiseResponse = new ClassWiseResponse();

        //        // Fetch all classes for the given InstituteID
        //        var classes = await _connection.QueryAsync<ClassDetail>(
        //            @"SELECT class_id as ClassID, class_name as ClassName
        //              FROM tbl_Class
        //              WHERE institute_id = @InstituteID",
        //            new { request.InstituteID });

        //        foreach (var classDetail in classes)
        //        {
        //            // Fetch sections for each class
        //            var sections = await _connection.QueryAsync<SectionDetail>(
        //                @"SELECT section_id as SectionID, section_name as SectionName
        //                  FROM tbl_Section
        //                  WHERE class_id = @ClassID",
        //                new { ClassID = classDetail.ClassID });

        //            foreach (var section in sections)
        //            {
        //                // Get GroupID by ClassID and SectionID
        //                var groupId = await _connection.ExecuteScalarAsync<int?>(
        //                    @"SELECT GroupID FROM tblTimeTableClassSession
        //                      WHERE ClassID = @ClassID AND SectionID = @SectionID",
        //                    new { ClassID = classDetail.ClassID, SectionID = section.SectionID });

        //                // If GroupID is found, calculate NumberOfSessions and NumberOfSubjects
        //                if (groupId.HasValue)
        //                {
        //                    // Count Number of Sessions for the group
        //                    section.NumberOfSessions = await _connection.ExecuteScalarAsync<int>(
        //                        @"SELECT COUNT(*) FROM tblTimeTableSessions
        //                          WHERE GroupID = @GroupID",
        //                        new { GroupID = groupId });

        //                    // Count Number of Subjects for the group
        //                    section.NumberOfSubjects = await _connection.ExecuteScalarAsync<int>(
        //                        @"SELECT COUNT(DISTINCT tse.SubjectID)
        //                          FROM tblTimeTableSessionMapping tsm
        //                          JOIN tblTimeTableSessionSubjectEmployee tse ON tsm.TTSessionID = tse.TTSessionID
        //                          WHERE tsm.GroupID = @GroupID",
        //                        new { GroupID = groupId });
        //                }
        //            }

        //            classDetail.SectionList.AddRange(sections);
        //            classWiseResponse.ClassList.Add(classDetail);
        //        }

        //        // Set the response as successful
        //        response = new ServiceResponse<ClassWiseResponse>(
        //            success: true,
        //            message: "Class and Section details fetched successfully.",
        //            data: classWiseResponse,
        //            statusCode: 200
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        // Set response in case of failure
        //        response = new ServiceResponse<ClassWiseResponse>(
        //            success: false,
        //            message: ex.Message,
        //            data: null,
        //            statusCode: 500
        //        );
        //    }

        //    return response;
        //}








        public async Task<ServiceResponse<ClassWiseTimeTableResponse>> GetClassWiseTimeTables(GetClassWiseTimeTablesRequest request)
        {
            var response = new ServiceResponse<ClassWiseTimeTableResponse>(
                success: false,
                message: "Initialization failed",
                data: new ClassWiseTimeTableResponse(),
                statusCode: 500,
                totalCount: null
            );

            try
            {
                // Fetch ClassName and SectionName
                var classSection = await _connection.QueryFirstOrDefaultAsync<dynamic>(
                    @"SELECT c.class_name AS ClassName, s.section_name AS SectionName 
              FROM tbl_Class c
              JOIN tbl_Section s ON c.class_id = s.class_id
              WHERE c.class_id = @ClassID AND s.section_id = @SectionID",
                    new { request.ClassID, request.SectionID });

                if (classSection == null)
                {
                    return new ServiceResponse<ClassWiseTimeTableResponse>(
                        success: false,
                        message: "Class and Section not found",
                        data: null,
                        statusCode: 404
                    );
                }

                var timeTableResponse = new ClassWiseTimeTableResponse
                {
                    ClassName = classSection.ClassName,
                    SectionName = classSection.SectionName,
                    Subjects = new Dictionary<string, string>()
                };

                // Fetch GroupID based on ClassID, SectionID, and AcademicYearCode
                var groupId = await _connection.ExecuteScalarAsync<int>(
                    @"SELECT tcs.GroupID 
              FROM tblTimeTableClassSession tcs
              JOIN tblTimeTableMaster tm ON tcs.GroupID = tm.GroupID
              WHERE tcs.ClassID = @ClassID 
                AND tcs.SectionID = @SectionID
                AND tm.AcademicYearCode = @AcademicYearCode",
                    new { request.ClassID, request.SectionID, request.AcademicYearCode });

                // Fetch subject counts (e.g., Subject1: 7/week) and populate the Subjects dictionary
                var subjectCount = await _connection.QueryAsync<dynamic>(
                    @"SELECT sub.SubjectName, COUNT(*) AS CountPerWeek 
              FROM tblTimeTableSessionSubjectEmployee tse
              JOIN tbl_Subjects sub ON tse.SubjectID = sub.SubjectId
              WHERE tse.TTSessionID IN (SELECT TTSessionID FROM tblTimeTableSessionMapping WHERE GroupID = @GroupID)
              GROUP BY sub.SubjectName",
                    new { GroupID = groupId });

                foreach (var subject in subjectCount)
                {
                    timeTableResponse.Subjects.Add(subject.SubjectName, $"{subject.CountPerWeek}/Week");
                }

                // Fetch PlanID based on the GroupID
                var planId = await _connection.ExecuteScalarAsync<int>(
                    @"SELECT PlanID 
              FROM tblTimeTableDayGroupsMapping 
              WHERE GroupID = @GroupID",
                    new { GroupID = groupId });

                // Fetch the Days associated with the PlanID
                var days = await _connection.QueryAsync<ClassWiseDayResponse>(
                    @"SELECT value AS DayID, tdm.DayType 
              FROM tblTimeTableDaySetup tds
              CROSS APPLY STRING_SPLIT(tds.DayIDs, ',')
              JOIN tblTimeTableDayMaster tdm ON value = tdm.DayID
              WHERE tds.PlanID = @PlanID",
                    new { PlanID = planId });

                var daysList = new List<ClassWiseDayResponse>();

                foreach (var day in days)
                {
                    var dayResponse = new ClassWiseDayResponse
                    {
                        DayID = day.DayID,
                        DayType = day.DayType,
                        Sessions = new List<ClassWiseSessionResponse>(),
                        Breaks = new List<ClassWiseBreakResponse>()
                    };

                    // Fetch sessions for the day
                    var sessions = await _connection.QueryAsync<ClassWiseSessionResponse>(
                        @"SELECT tsm.SessionID, ts.SessionName,
                   CASE 
                       WHEN ts.StartTime IS NOT NULL AND ts.EndTime IS NOT NULL 
                       THEN FORMAT(ts.StartTime, 'hh:mm tt') + ' - ' + FORMAT(ts.EndTime, 'hh:mm tt')
                       ELSE 'Time Not Available'
                   END AS SessionTime
                FROM tblTimeTableSessionMapping tsm
                JOIN tblTimeTableSessions ts ON tsm.SessionID = ts.SessionID
                WHERE tsm.DayID = @DayID AND tsm.GroupID = @GroupID",
                        new { DayID = day.DayID, GroupID = groupId });

                    dayResponse.Sessions.AddRange(sessions);

                    // Fetch EmployeeSubjects for each session
                    foreach (var session in dayResponse.Sessions)
                    {
                        var employeeSubjects = await _connection.QueryAsync<ClassWiseEmployeeSubjectResponse>(
                            @"SELECT tse.SubjectID, sub.SubjectName, tse.EmployeeID, 
                      emp.First_Name + ' ' + emp.Last_Name as EmployeeName
                      FROM tblTimeTableSessionSubjectEmployee tse
                      JOIN tbl_Subjects sub ON tse.SubjectID = sub.SubjectId
                      JOIN tbl_EmployeeProfileMaster emp ON tse.EmployeeID = emp.Employee_id
                      JOIN tblTimeTableSessionMapping tsm ON tse.TTSessionID = tsm.TTSessionID
                      WHERE tsm.SessionID = @SessionID",
                            new { SessionID = session.SessionID });

                        session.EmployeeSubjects.AddRange(employeeSubjects);
                    }

                    // Fetch breaks for the day
                    var breaks = await _connection.QueryAsync<ClassWiseBreakResponse>(
                        @"SELECT tbm.BreaksID as BreakID, tb.BreaksName,
                   FORMAT(CAST(tb.StartTime AS DATETIME), 'hh:mm tt') + ' - ' + FORMAT(CAST(tb.EndTime AS DATETIME), 'hh:mm tt') AS BreakTime
                FROM tblTimeTableBreakMapping tbm
                JOIN tblTimeTableBreaks tb ON tbm.BreaksID = tb.BreaksID
                WHERE tbm.DayID = @DayID AND tbm.GroupID = @GroupID",
                        new { DayID = day.DayID, GroupID = groupId });

                    dayResponse.Breaks.AddRange(breaks);
                    daysList.Add(dayResponse);
                }

                timeTableResponse.Days = daysList;

                // Set response on success
                response = new ServiceResponse<ClassWiseTimeTableResponse>(
                    success: true,
                    message: "Class-wise TimeTables fetched successfully.",
                    data: timeTableResponse,
                    statusCode: 200,
                    totalCount: null
                );
            }
            catch (Exception ex)
            {
                // Handle error
                response = new ServiceResponse<ClassWiseTimeTableResponse>(
                    success: false,
                    message: ex.Message,
                    data: null,
                    statusCode: 500,
                    totalCount: null
                );
            }

            return response;
        }


        //public async Task<ServiceResponse<ClassWiseTimeTableResponse>> GetClassWiseTimeTables(GetClassWiseTimeTablesRequest request)
        //{
        //    var response = new ServiceResponse<ClassWiseTimeTableResponse>(
        //        success: false,
        //        message: "Initialization failed",
        //        data: new ClassWiseTimeTableResponse(),
        //        statusCode: 500,
        //        totalCount: null
        //    );

        //    try
        //    {
        //        // Fetch ClassName and SectionName
        //        var classSection = await _connection.QueryFirstOrDefaultAsync<dynamic>(
        //            @"SELECT c.class_name AS ClassName, s.section_name AS SectionName 
        //      FROM tbl_Class c
        //      JOIN tbl_Section s ON c.class_id = s.class_id
        //      WHERE c.class_id = @ClassID AND s.section_id = @SectionID",
        //            new { request.ClassID, request.SectionID });

        //        if (classSection == null)
        //        {
        //            return new ServiceResponse<ClassWiseTimeTableResponse>(
        //                success: false,
        //                message: "Class and Section not found",
        //                data: null,
        //                statusCode: 404
        //            );
        //        }

        //        var timeTableResponse = new ClassWiseTimeTableResponse
        //        {
        //            ClassName = classSection.ClassName,
        //            SectionName = classSection.SectionName,
        //            Subjects = new Dictionary<string, string>()
        //        };

        //        // Fetch GroupID based on ClassID and SectionID
        //        var groupId = await _connection.ExecuteScalarAsync<int>(
        //            @"SELECT GroupID FROM tblTimeTableClassSession 
        //      WHERE ClassID = @ClassID AND SectionID = @SectionID",
        //            new { request.ClassID, request.SectionID });

        //        // Fetch subject counts (e.g., Subject1: 7/week) and populate the Subjects dictionary
        //        var subjectCount = await _connection.QueryAsync<dynamic>(
        //            @"SELECT sub.SubjectName, COUNT(*) AS CountPerWeek 
        //      FROM tblTimeTableSessionSubjectEmployee tse
        //      JOIN tbl_Subjects sub ON tse.SubjectID = sub.SubjectId
        //      WHERE tse.TTSessionID IN (SELECT TTSessionID FROM tblTimeTableSessionMapping WHERE GroupID = @GroupID)
        //      GROUP BY sub.SubjectName",
        //            new { GroupID = groupId });

        //        foreach (var subject in subjectCount)
        //        {
        //            timeTableResponse.Subjects.Add(subject.SubjectName, $"{subject.CountPerWeek}/Week");
        //        }

        //        // Fetch PlanID based on the GroupID
        //        var planId = await _connection.ExecuteScalarAsync<int>(
        //            @"SELECT PlanID FROM tblTimeTableDayGroupsMapping 
        //      WHERE GroupID = @GroupID",
        //            new { GroupID = groupId });

        //        // Fetch the Days associated with the PlanID
        //        var days = await _connection.QueryAsync<ClassWiseDayResponse>(
        //            @"SELECT value AS DayID, tdm.DayType 
        //      FROM tblTimeTableDaySetup tds
        //      CROSS APPLY STRING_SPLIT(tds.DayIDs, ',')
        //      JOIN tblTimeTableDayMaster tdm ON value = tdm.DayID
        //      WHERE tds.PlanID = @PlanID",
        //            new { PlanID = planId });

        //        var daysList = new List<ClassWiseDayResponse>();

        //        foreach (var day in days)
        //        {
        //            var dayResponse = new ClassWiseDayResponse
        //            {
        //                DayID = day.DayID,
        //                DayType = day.DayType,
        //                Sessions = new List<ClassWiseSessionResponse>(),
        //                Breaks = new List<ClassWiseBreakResponse>()
        //            };

        //            // Fetch sessions for the day along with Start and End Time
        //            var sessions = await _connection.QueryAsync<ClassWiseSessionResponse>(
        //                @"SELECT tsm.SessionID, ts.SessionName,
        //           CASE 
        //               WHEN ts.StartTime IS NOT NULL AND ts.EndTime IS NOT NULL 
        //               THEN FORMAT(ts.StartTime, 'hh:mm tt') + ' - ' + FORMAT(ts.EndTime, 'hh:mm tt')
        //               ELSE 'Time Not Available'
        //           END AS SessionTime
        //        FROM tblTimeTableSessionMapping tsm
        //        JOIN tblTimeTableSessions ts ON tsm.SessionID = ts.SessionID
        //        WHERE tsm.DayID = @DayID AND tsm.GroupID = @GroupID",
        //                new { DayID = day.DayID, GroupID = groupId });

        //            dayResponse.Sessions.AddRange(sessions);

        //            // Fetch EmployeeSubjects for each session
        //            foreach (var session in dayResponse.Sessions)
        //            {
        //                var employeeSubjects = await _connection.QueryAsync<ClassWiseEmployeeSubjectResponse>(
        //                    @"SELECT tse.SubjectID, sub.SubjectName, tse.EmployeeID, 
        //                emp.First_Name + ' ' + emp.Last_Name as EmployeeName
        //        FROM tblTimeTableSessionSubjectEmployee tse
        //        JOIN tbl_Subjects sub ON tse.SubjectID = sub.SubjectId
        //        JOIN tbl_EmployeeProfileMaster emp ON tse.EmployeeID = emp.Employee_id
        //        JOIN tblTimeTableSessionMapping tsm ON tse.TTSessionID = tsm.TTSessionID
        //        WHERE tsm.SessionID = @SessionID",
        //                    new { SessionID = session.SessionID });

        //                session.EmployeeSubjects.AddRange(employeeSubjects);
        //            }

        //            // Fetch breaks for the day along with Start and End Time
        //            var breaks = await _connection.QueryAsync<ClassWiseBreakResponse>(
        //                @"SELECT tbm.BreaksID as BreakID, tb.BreaksName as BreakName,
        //           FORMAT(CAST(tb.StartTime AS DATETIME), 'hh:mm tt') + ' - ' + FORMAT(CAST(tb.EndTime AS DATETIME), 'hh:mm tt') AS BreakTime
        //        FROM tblTimeTableBreakMapping tbm
        //        JOIN tblTimeTableBreaks tb ON tbm.BreaksID = tb.BreaksID
        //        WHERE tbm.DayID = @DayID AND tbm.GroupID = @GroupID",
        //                new { DayID = day.DayID, GroupID = groupId });

        //            dayResponse.Breaks.AddRange(breaks);

        //            // Add each day's response to the list of days
        //            daysList.Add(dayResponse);
        //        }

        //        timeTableResponse.Days = daysList;

        //        // Set response on success
        //        response = new ServiceResponse<ClassWiseTimeTableResponse>(
        //            success: true,
        //            message: "Class-wise TimeTables fetched successfully.",
        //            data: timeTableResponse,
        //            statusCode: 200,
        //            totalCount: null
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle error
        //        response = new ServiceResponse<ClassWiseTimeTableResponse>(
        //            success: false,
        //            message: ex.Message,
        //            data: null,
        //            statusCode: 500,
        //            totalCount: null
        //        );
        //    }

        //    return response;
        //}

    }
}
