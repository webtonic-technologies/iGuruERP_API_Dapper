using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.Repository.Interfaces;

namespace TimeTable_API.Repository.Implementations
{
    public class EmployeeWiseRepository : IEmployeeWiseRepository
    {
        private readonly IDbConnection _connection;

        public EmployeeWiseRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<EmployeeWiseResponse>>> GetEmployeeWiseTimetable(EmployeeWiseRequest request)
        {
            var response = new ServiceResponse<List<EmployeeWiseResponse>>(false, "Failed to retrieve timetable", new List<EmployeeWiseResponse>(), 500);

            try
            {
                // Fetch all days
                var allDays = await _connection.QueryAsync<string>("SELECT DayType FROM tblTimeTableDayMaster");

                // Fetch sessions with employee mapping for given AcademicYearCode and EmployeeID
                var groupSessions = await _connection.QueryAsync<EmployeeWiseSessionResponse>(
                    @"SELECT 
                        d.DayType AS Day,
                        s.SessionName,
                        c.class_name + ' - ' + sec.section_name AS ClassNameSection,
                        CONVERT(VARCHAR(5), s.StartTime, 108) AS StartTime,
                        CONVERT(VARCHAR(5), s.EndTime, 108) AS EndTime
                    FROM tblTimeTableSessionSubjectEmployee tse
                    JOIN tblTimeTableSessionMapping tsm ON tse.TTSessionID = tsm.TTSessionID
                    JOIN tblTimeTableSessions s ON tsm.SessionID = s.SessionID
                    JOIN tblTimeTableDayMaster d ON tsm.DayID = d.DayID
                    JOIN tblTimeTableClassSession tcs ON tcs.GroupID = tsm.GroupID
                    JOIN tblTimeTableMaster tm ON tm.GroupID = tsm.GroupID AND tm.AcademicYearCode = @AcademicYearCode
                    JOIN tbl_Class c ON tcs.ClassID = c.class_id
                    JOIN tbl_Section sec ON tcs.SectionID = sec.section_id
                    WHERE tse.EmployeeID = @EmployeeID AND tm.InstituteID = @InstituteID",
                    new { request.EmployeeID, request.AcademicYearCode, request.InstituteID }
                );

                // Group sessions by day, keeping blanks for sessions without employee assignments
                var groupedByDay = allDays.Select(day => new EmployeeWiseResponse
                {
                    Day = day,
                    Sessions = groupSessions
                        .Where(s => s.Day == day)
                        .Select(s => new EmployeeWiseSessionResponse
                        {
                            Day = s.Day,
                            SessionName = s.SessionName,
                            ClassNameSection = s.ClassNameSection,
                            StartTime = s.StartTime,
                            EndTime = s.EndTime
                        })
                        .ToList()
                }).ToList();

                response = new ServiceResponse<List<EmployeeWiseResponse>>(true, "Timetable retrieved successfully", groupedByDay, 200);
            }
            catch (Exception ex)
            {
                response = new ServiceResponse<List<EmployeeWiseResponse>>(false, ex.Message, null, 500);
            }

            return response;
        }
    }
}
