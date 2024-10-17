using System.Threading.Tasks;
using Dapper;
using System.Data;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.Repository.Interfaces;
using System.Globalization; 


namespace TimeTable_API.Repository.Implementations
{
    public class EmployeeSubstitutionRepository : IEmployeeSubstitutionRepository
    {
        private readonly IDbConnection _connection;

        public EmployeeSubstitutionRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<EmployeeSubstitutionResponse>>> GetSubstitution(EmployeeSubstitutionRequest request)
        {
            try
            {
                string sql = @"
        SELECT DISTINCT
            tse.SubjectID,
            sub.SubjectName AS Subject,
            tcs.ClassID,
            tcs.SectionID,
            c.class_name + ' - ' + s.section_name AS ClassSession,
            CONVERT(VARCHAR(5), ses.StartTime, 108) + ' - ' + CONVERT(VARCHAR(5), ses.EndTime, 108) AS SessionTiming,
            empSub.First_Name + ' ' + empSub.Last_Name AS Substitution
        FROM tblTimeTableSessionSubjectEmployee tse
        JOIN tblTimeTableSessionMapping tsm ON tse.TTSessionID = tsm.TTSessionID
        JOIN tblTimeTableSessions ses ON tsm.SessionID = ses.SessionID
        JOIN tblTimeTableClassSession tcs ON tsm.GroupID = tcs.GroupID
        JOIN tbl_Class c ON tcs.ClassID = c.class_id
        JOIN tbl_Section s ON tcs.SectionID = s.section_id
        JOIN tbl_Subjects sub ON tse.SubjectID = sub.SubjectID
        JOIN tblTimeTableMaster tm ON tsm.GroupID = tm.GroupID AND tm.AcademicYearCode = @AcademicYearCode
        LEFT JOIN tblTimeTableSubstitutes subs ON tse.SubjectID = subs.SubjectID
            AND tcs.ClassID = subs.ClassID
            AND tcs.SectionID = subs.SectionID
            AND tsm.SessionID = subs.SessionID
            AND tse.EmployeeID = subs.EmployeeID
            AND subs.SubstitutesDate = @SubstitutionDate
        LEFT JOIN tbl_EmployeeProfileMaster empSub ON subs.SubstitutesEmployeeID = empSub.Employee_id
        WHERE tse.EmployeeID = @EmployeeID";

                if (!string.IsNullOrEmpty(request.SearchBySubject))
                {
                    sql += " AND sub.SubjectName LIKE @SearchBySubject";
                }

                // Convert the date to the format SQL expects
                var substitutionDate = DateTime.ParseExact(request.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                // Execute the SQL query
                var substitutionData = await _connection.QueryAsync<EmployeeSubstitutionResponse>(sql, new
                {
                    request.AcademicYearCode,
                    request.EmployeeID,
                    SubstitutionDate = substitutionDate, // Pass the correctly formatted date
                    request.SearchBySubject
                });

                // Check if any data is returned
                if (substitutionData.Any())
                {
                    return new ServiceResponse<List<EmployeeSubstitutionResponse>>(
                        true,
                        "Substitution fetched successfully",
                        substitutionData.ToList(),
                        200
                    );
                }
                else
                {
                    return new ServiceResponse<List<EmployeeSubstitutionResponse>>(
                        true,
                        "No substitutions found for the given parameters.",
                        new List<EmployeeSubstitutionResponse>(),
                        200
                    );
                }
            }
            catch (Exception ex)
            {
                // Handle error
                return new ServiceResponse<List<EmployeeSubstitutionResponse>>(
                    false,
                    ex.Message,
                    new List<EmployeeSubstitutionResponse>(),
                    500
                );
            }
        }
         
        public async Task<ServiceResponse<int>> UpdateSubstitution(EmployeeSubstitutionRequest_Update request)
        {
            try
            {
                // Convert SubstitutesDate from DD-MM-YYYY to DateTime
                var substitutesDate = DateTime.ParseExact(request.SubstitutesDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                // Check if a record with the same details already exists (ignoring SubstitutesEmployeeID)
                var checkExistingSql = @"
            SELECT COUNT(1)
            FROM tblTimeTableSubstitutes
            WHERE SubstitutesDate = @SubstitutesDate
            AND SubjectID = @SubjectID
            AND ClassID = @ClassID
            AND SectionID = @SectionID
            AND SessionID = @SessionID
            AND EmployeeID = @EmployeeID
            AND AcademicYearCode = @AcademicYearCode
            AND InstituteID = @InstituteID";  // Add InstituteID to the check

                var existingCount = await _connection.ExecuteScalarAsync<int>(checkExistingSql, new
                {
                    SubstitutesDate = substitutesDate,  // Pass the converted DateTime
                    request.SubjectID,
                    request.ClassID,
                    request.SectionID,
                    request.SessionID,
                    request.EmployeeID,
                    request.AcademicYearCode,
                    request.InstituteID  // Pass InstituteID
                });

                // If the exact same entry with same SubstitutesEmployeeID exists, return a message
                var checkExactEntrySql = @"
            SELECT COUNT(1)
            FROM tblTimeTableSubstitutes
            WHERE SubstitutesDate = @SubstitutesDate
            AND SubjectID = @SubjectID
            AND ClassID = @ClassID
            AND SectionID = @SectionID
            AND SessionID = @SessionID
            AND EmployeeID = @EmployeeID
            AND AcademicYearCode = @AcademicYearCode
            AND SubstitutesEmployeeID = @SubstitutesEmployeeID
            AND InstituteID = @InstituteID";  // Add InstituteID to the check

                var exactEntryCount = await _connection.ExecuteScalarAsync<int>(checkExactEntrySql, new
                {
                    SubstitutesDate = substitutesDate,  // Pass the converted DateTime
                    request.SubjectID,
                    request.ClassID,
                    request.SectionID,
                    request.SessionID,
                    request.EmployeeID,
                    request.AcademicYearCode,
                    request.SubstitutesEmployeeID,
                    request.InstituteID  // Pass InstituteID
                });

                if (exactEntryCount > 0)
                {
                    return new ServiceResponse<int>(false, "Same entry already exists.", 0, 200);
                }

                // If the entry exists but with a different SubstitutesEmployeeID, update the SubstitutesEmployeeID
                if (existingCount > 0)
                {
                    var updateSql = @"
                UPDATE tblTimeTableSubstitutes
                SET SubstitutesEmployeeID = @SubstitutesEmployeeID
                WHERE SubstitutesDate = @SubstitutesDate
                AND SubjectID = @SubjectID
                AND ClassID = @ClassID
                AND SectionID = @SectionID
                AND SessionID = @SessionID
                AND EmployeeID = @EmployeeID
                AND AcademicYearCode = @AcademicYearCode
                AND InstituteID = @InstituteID";  // Add InstituteID to the update

                    var result = await _connection.ExecuteAsync(updateSql, new
                    {
                        SubstitutesDate = substitutesDate,  // Pass the converted DateTime
                        request.SubjectID,
                        request.ClassID,
                        request.SectionID,
                        request.SessionID,
                        request.EmployeeID,
                        request.AcademicYearCode,
                        request.SubstitutesEmployeeID,
                        request.InstituteID  // Pass InstituteID
                    });

                    return new ServiceResponse<int>(true, "Substitute updated successfully.", result, 200);
                }
                else
                {
                    // If no entry exists, insert a new record
                    var insertSql = @"
                INSERT INTO tblTimeTableSubstitutes (SubstitutesDate, SubjectID, ClassID, SectionID, SessionID, EmployeeID, SubstitutesEmployeeID, AcademicYearCode, InstituteID)
                VALUES (@SubstitutesDate, @SubjectID, @ClassID, @SectionID, @SessionID, @EmployeeID, @SubstitutesEmployeeID, @AcademicYearCode, @InstituteID)";  // Add InstituteID to the insert

                    var result = await _connection.ExecuteAsync(insertSql, new
                    {
                        SubstitutesDate = substitutesDate,  // Pass the converted DateTime
                        request.SubjectID,
                        request.ClassID,
                        request.SectionID,
                        request.SessionID,
                        request.EmployeeID,
                        request.AcademicYearCode,
                        request.SubstitutesEmployeeID,
                        request.InstituteID  // Pass InstituteID
                    });

                    return new ServiceResponse<int>(true, "Substitute added successfully.", result, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<List<SubstituteEmployeeResponse>>> GetSubstituteEmployeeList(GetSubstituteEmployeeListRequest request)
        {
            try
            {
                // Parse StartTime and EndTime from "hh:mm tt" (e.g., "08:00 AM") to "HH:mm:ss" (24-hour format)
                TimeSpan startTime = DateTime.ParseExact(request.StartTime, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                TimeSpan endTime = DateTime.ParseExact(request.EndTime, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;

                // SQL query to fetch employee details based on the parsed time
                string sql = @"
            SELECT DISTINCT 
                tse.EmployeeID,
                emp.First_Name + ' ' + emp.Last_Name AS EmployeeName
            FROM tblTimeTableSessionSubjectEmployee tse
            JOIN tblTimeTableSessionMapping tsm ON tse.TTSessionID = tsm.TTSessionID
            JOIN tblTimeTableSessions ts ON tsm.SessionID = ts.SessionID
            JOIN tblTimeTableClassSession tcs ON tsm.GroupID = tcs.GroupID
            JOIN tblTimeTableMaster tm ON tm.GroupID = tsm.GroupID
            JOIN tbl_EmployeeProfileMaster emp ON tse.EmployeeID = emp.Employee_id
            WHERE tm.InstituteID = @InstituteID
              AND NOT (CAST(ts.StartTime AS TIME) BETWEEN @StartTime AND @EndTime
                   OR CAST(ts.EndTime AS TIME) BETWEEN @StartTime AND @EndTime);";

                // Execute the SQL query, passing the parsed times
                var employeeList = await _connection.QueryAsync<SubstituteEmployeeResponse>(sql, new
                {
                    request.InstituteID,
                    StartTime = startTime,  // Pass the TimeSpan
                    EndTime = endTime       // Pass the TimeSpan
                });

                return new ServiceResponse<List<SubstituteEmployeeResponse>>(
                    true,
                    "Employees fetched successfully.",
                    employeeList.AsList(),
                    200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<SubstituteEmployeeResponse>>(
                    false,
                    ex.Message,
                    new List<SubstituteEmployeeResponse>(),
                    500
                );
            }
        }


    }
}
