using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Repository.Interfaces;
using Dapper;
using System.Data;
using System.Text.Json;

namespace Attendance_API.Repository.Implementations
{
    public class StudentAttendanceReportRepository : IStudentAttendanceReportRepository
    {
        private readonly IDbConnection _connection;

        public StudentAttendanceReportRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<dynamic>> GetStudentAttendanceDatewiseReport(StudentAttendanceDatewiseReportRequestDTO request)
        {
            try
            {
                var query = @"
    DECLARE @cols AS NVARCHAR(MAX);
    DECLARE @query AS NVARCHAR(MAX);

    -- Generate a list of all dates between StartDate and EndDate
    WITH DateList AS (
        SELECT @StartDate AS Date
        UNION ALL
        SELECT DATEADD(DAY, 1, Date)
        FROM DateList
        WHERE Date < @EndDate
    )
    SELECT @cols = STRING_AGG(QUOTENAME(CONVERT(VARCHAR, Date, 23)), ', ')
    FROM DateList;

    SET @query = '
    DECLARE @TotalDays INT ;

    SET @TotalDays = DATEDIFF(DAY, @StartDate, @EndDate);
    WITH DateList AS (
        SELECT @StartDate AS Date
        UNION ALL
        SELECT DATEADD(DAY, 1, Date)
        FROM DateList
        WHERE Date < @EndDate
    ),
    AttendanceData AS (
        SELECT 
            s.Admission_Number,
            s.First_Name,
            s.Roll_Number,
            d.Date,
            ISNULL(
                CASE 
                    WHEN a.isHoliday = 1 THEN ''H'' 
                    ELSE ast.Short_Name 
                END, 
                ''-''
            ) AS Status,
            @TotalDays - SUM(CASE WHEN a.isHoliday = 1 THEN 1 ELSE 0 END) OVER (PARTITION BY s.Student_id) AS WorkingDays,
            SUM(CASE WHEN a.isHoliday = 0 AND ast.Short_Name = ''P'' THEN 1 ELSE 0 END) OVER (PARTITION BY s.Student_id) AS Present,
            SUM(CASE WHEN a.isHoliday = 0 AND ast.Short_Name = ''A'' THEN 1 ELSE 0 END) OVER (PARTITION BY s.Student_id) AS Absent,
            ROUND(CAST(SUM(CASE WHEN a.isHoliday = 0 AND ast.Short_Name = ''P'' THEN 1 ELSE 0 END) OVER (PARTITION BY s.Student_id) AS FLOAT) 
                  / NULLIF(COUNT(*) OVER (PARTITION BY s.Student_id) - SUM(CASE WHEN a.isHoliday = 1 THEN 1 ELSE 0 END) OVER (PARTITION BY s.Student_id), 0) * 100, 2) AS AttendancePercentage
        FROM 
            DateList d
        LEFT JOIN 
            tbl_StudentAttendanceMaster a ON d.Date = a.Date
        LEFT JOIN 
            tbl_studentmaster s ON a.Student_id = s.Student_id
        LEFT JOIN 
            tbl_StudentAttendanceStatus ast ON a.Student_Attendance_Status_id = ast.Student_Attendance_Status_id
        WHERE 
            isDatewise = 1 AND d.Date BETWEEN @StartDate AND @EndDate AND (@class_id = 0 OR s.class_id = @class_id) AND  (@section_id = 0 OR s.section_id = @section_id) AND (@institute_id = 0 OR s.institute_id = @institute_id)
    )
   
            SELECT 
                Admission_Number,
                First_Name,
                Roll_Number,
                WorkingDays,
                Present,
                Absent,
                AttendancePercentage,
                ' + @cols + '
            FROM 
                AttendanceData
            PIVOT (
                MAX(Status) 
                FOR Date IN (' + @cols + ')
            ) AS PivotTable
            WHERE 
                Admission_Number IS NOT NULL AND First_Name IS NOT NULL AND Roll_Number IS NOT NULL
            ORDER BY 
                Roll_Number;'; 

    EXEC sp_executesql @query,   N'@StartDate DATE, @EndDate DATE, @class_id INT, @section_id INT, @institute_id INT', 
    @StartDate, @EndDate, @class_id, @section_id, @institute_id;";
                // Parameters for the query
                var parameters = new { StartDate = request.StartDate, EndDate = request.EndDate, section_id = request.section_id, class_id = request.class_id, institute_id = request.instituteId };

                // Execute the query and fetch the result
                var result = await _connection.QueryAsync<dynamic>(query, parameters);
                //var resultJson = JsonSerializer.Deserialize<dynamic>(result);


                return new ServiceResponse<dynamic>(true, "Operation successful", result, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<dynamic>(false, $"Error: {ex.Message}", null, 500);
            }
        }

        public async Task<ServiceResponse<dynamic>> GetStudentSubjectwiseReport(SubjectwiseAttendanceReportRequest request)
        {
            try
            {
                var query = @"
DECLARE @cols AS NVARCHAR(MAX);
DECLARE @query AS NVARCHAR(MAX);

-- Generate a list of all subjects
WITH SubjectList AS (
    SELECT DISTINCT 
        sub.Subject_Name
    FROM tbl_ClassSectionSubjectMapping s
    INNER JOIN tbl_Subject sub ON s.SubjectId = sub.Subject_id
    WHERE 
        s.class_id = @class_id AND 
        s.section_id = @section_id
)
SELECT @cols = STRING_AGG(QUOTENAME(Subject_Name), ', ')
FROM SubjectList;

SET @query = '
    DECLARE @TotalDays INT;

    -- Calculate total days
    SET @TotalDays = (SELECT COUNT(DISTINCT a.Date) 
                      FROM tbl_StudentAttendanceMaster a 
                       WHERE CAST(a.Date AS DATE) = CAST(@Date AS DATE));

    WITH AttendanceData AS (
        SELECT DISTINCT
		s.Student_id,
            s.Admission_Number,
            s.First_Name,
            s.Roll_Number,
            sub.Subject_Name,
				a.Date,
            ISNULL(
                CASE 
                    WHEN a.isHoliday = 1 THEN ''H'' 
                    ELSE ast.Short_Name 
                END, 
                ''-''
            ) AS Status
        FROM 
            tbl_StudentAttendanceMaster a
        INNER JOIN 
            tbl_studentmaster s ON a.Student_id = s.Student_id
        INNER JOIN 
            tbl_StudentAttendanceStatus ast ON a.Student_Attendance_Status_id = ast.Student_Attendance_Status_id
        INNER JOIN 
            tbl_ClassSectionSubjectMapping subMap ON a.Subject_id = subMap.SubjectId
        INNER JOIN 
            tbl_Subject sub ON subMap.SubjectId = sub.Subject_id
        WHERE 
            isDatewise = 0 
        AND CAST(a.Date AS DATE) = CAST(@Date AS DATE)
            AND (@class_id = 0 OR s.class_id = @class_id) 
            AND (@section_id = 0 OR s.section_id = @section_id) 
            AND (@institute_id = 0 OR s.institute_id = @institute_id)
    )


    SELECT 
	Student_id,
        Admission_Number,
        First_Name,
        Roll_Number,
			Date,
        ' + @cols + '
    FROM 
        AttendanceData
    PIVOT (
        MAX(Status) 
        FOR Subject_Name IN (' + @cols + ')
    ) AS PivotTable
    WHERE 
        Admission_Number IS NOT NULL AND First_Name IS NOT NULL AND Roll_Number IS NOT NULL
    ORDER BY 
        Roll_Number;'; 

EXEC sp_executesql @query, 
    N'@Date DATE,@class_id INT, @section_id INT, @institute_id INT', 
    @Date, @class_id, @section_id, @institute_id;";

                var parameters = new { Date = request.Date, section_id = request.section_id, class_id = request.class_id, institute_id = request.Institute_Id };

                var result = await _connection.QueryAsync<dynamic>(query, parameters);

                return new ServiceResponse<dynamic>(true, "Operation successful", result, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<dynamic>(false, $"Error: {ex.Message}", null, 500);
            }
        }


    }
}
