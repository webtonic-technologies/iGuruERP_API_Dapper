using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Repository.Interfaces;
using Dapper;
using System.Data;

namespace Attendance_API.Repository.Implementations
{
    public class ClassAttendanceAnalysisRepo : IClassAttendanceAnalysisRepo
    {
        private readonly IDbConnection _connection;

        public ClassAttendanceAnalysisRepo(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<ServiceResponse<AttendanceStatisticsDTO>> GetAttendanceStatistics(int academicYearId, int classId, int sectionId)
        {
            try
            {
                // Define your SQL query to retrieve attendance data
                string query = @"
               DECLARE @TotalWorkingDays INT;
                DECLARE @AverageAttendance FLOAT;
                DECLARE @StudentsWith100PercentAttendance INT;
                DECLARE @StudentsAbove80PercentAttendance INT;
                
                DROP TABLE IF EXISTS #FilteredAttendanceData;
                -- Create a temp table to store filtered attendance data
                CREATE TABLE #FilteredAttendanceData (
                    Student_id INT,
                    Student_Attendance_Status_id INT,
                    Date DATE,
                    isHoliday BIT
                );
                
                -- Insert filtered data into the temp table
                INSERT INTO #FilteredAttendanceData (Student_id, Student_Attendance_Status_id, Date, isHoliday)
                SELECT sa.Student_id, sa.Student_Attendance_Status_id, sa.Date, sa.isHoliday
                FROM tbl_StudentAttendanceMaster sa
                JOIN tbl_StudentMaster sm ON sa.Student_id = sm.student_id
                WHERE sa.isHoliday = 0
                    AND sa.Date BETWEEN (SELECT Startdate FROM tbl_AcademicYear WHERE Id = @AcademicYearId)
                                    AND (SELECT Enddate FROM tbl_AcademicYear WHERE Id = @AcademicYearId)
                    AND(@ClassId = 0 OR  sm.class_id = @ClassId)
                    AND (@SectionId = 0 OR sm.section_id = @SectionId);
                
                -- Calculate TotalWorkingDays
                SELECT @TotalWorkingDays = COUNT(DISTINCT Date)
                FROM #FilteredAttendanceData;
                
                -- Calculate AverageAttendance
                SELECT @AverageAttendance = AVG(CAST(CASE WHEN sas.Short_Name = 'P' THEN 1 ELSE 0 END AS FLOAT)) * 100
                FROM #FilteredAttendanceData fa
                JOIN tbl_StudentAttendanceStatus sas ON fa.Student_Attendance_Status_id = sas.Student_Attendance_Status_id;
                
                -- Calculate StudentsWith100PercentAttendance
                SELECT @StudentsWith100PercentAttendance = COUNT(*)
                FROM (
                    SELECT
                        fa.Student_id,
                        COUNT(CASE WHEN sas.Short_Name = 'P' THEN 1 ELSE NULL END) * 100.0 / COUNT(DISTINCT fa.Date) AS AttendancePercentage
                    FROM #FilteredAttendanceData fa
                    JOIN tbl_StudentAttendanceStatus sas ON fa.Student_Attendance_Status_id = sas.Student_Attendance_Status_id
                    GROUP BY fa.Student_id
                ) AS AttendanceStats
                WHERE AttendanceStats.AttendancePercentage = 100;
                
                -- Calculate StudentsAbove80PercentAttendance
                SELECT @StudentsAbove80PercentAttendance = COUNT(*)
                FROM (
                    SELECT
                        fa.Student_id,
                        COUNT(CASE WHEN sas.Short_Name = 'P' THEN 1 ELSE NULL END) * 100.0 / COUNT(DISTINCT fa.Date) AS AttendancePercentage
                    FROM #FilteredAttendanceData fa
                    JOIN tbl_StudentAttendanceStatus sas ON fa.Student_Attendance_Status_id = sas.Student_Attendance_Status_id
                    GROUP BY fa.Student_id
                ) AS AttendanceStats
                WHERE AttendanceStats.AttendancePercentage > 80;
                
                -- Select the results
                SELECT 
                    @TotalWorkingDays AS TotalWorkingDays,
                    @AverageAttendance AS AverageAttendance,
                    @StudentsWith100PercentAttendance AS StudentsWith100PercentAttendance,
                    @StudentsAbove80PercentAttendance AS StudentsAbove80PercentAttendance;
                
                -- Drop the temp table
                DROP TABLE IF EXISTS #FilteredAttendanceData;";

                // Parameters for the SQL query
                var parameters = new
                {
                    AcademicYearId = academicYearId,
                    ClassId = classId,
                    SectionId = sectionId
                };

                // Execute the query and retrieve the result
                var attendanceStats = await _connection.QueryFirstOrDefaultAsync<AttendanceStatisticsDTO>(query, parameters);

                if (attendanceStats != null)
                {
                    return new ServiceResponse<AttendanceStatisticsDTO>(true, "Attendance statistics retrieved successfully", attendanceStats, 200);
                }
                else
                {
                    return new ServiceResponse<AttendanceStatisticsDTO>(false, "No attendance statistics found for the given parameters", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AttendanceStatisticsDTO>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<MonthlyAttendanceAnalysisDTO>>> GetMonthlyAttendanceAnalysis(int academicYearId, int classId, int sectionId)
        {
            try
            {
                var query = @"
                SELECT 
                    CONCAT(LEFT(DATENAME(MONTH, sa.Date), 3), ' ', YEAR(sa.Date)) AS MonthYear,
                    AVG(CAST(CASE WHEN ast.Short_Name = 'P'  THEN 1 ELSE 0 END AS FLOAT)) * 100 AS AverageAttendancePercentag
                FROM tbl_StudentAttendanceMaster sa
                INNER JOIN tbl_studentmaster s ON sa.Student_id = s.student_id
                LEFT JOIN tbl_StudentAttendanceStatus ast ON sa.Student_Attendance_Status_id = ast.Student_Attendance_Status_id
                WHERE sa.isHoliday = 0
                    AND (@ClassId =0 OR s.class_id = @ClassId)
                    AND (@SectionId = 0 OR  s.section_id = @SectionId)
                    AND sa.Date BETWEEN (SELECT Startdate FROM tbl_AcademicYear WHERE Id = @AcademicYearId)
                                    AND (SELECT Enddate FROM tbl_AcademicYear WHERE Id = @AcademicYearId)
                GROUP BY YEAR(sa.Date), MONTH(sa.Date), DATENAME(MONTH, sa.Date)
                ORDER BY YEAR(sa.Date), MONTH(sa.Date);";

                // Parameters for the query
                var parameters = new { AcademicYearId = academicYearId, ClassId = classId, SectionId = sectionId };

                // Execute the query and fetch the result
                var result = await _connection.QueryAsync<MonthlyAttendanceAnalysisDTO>(query, parameters);

                return new ServiceResponse<List<MonthlyAttendanceAnalysisDTO>>(true, "Monthly attendance analysis retrieved successfully", result.AsList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<MonthlyAttendanceAnalysisDTO>>(false, $"Error: {ex.Message}", null, 500);
            }
        }

        public async Task<ServiceResponse<List<MonthlyAttendanceAnalysisDTO>>> GetAttendanceRangeAnalysis(int academicYearId, int classId, int sectionId)
        {
            try
            {
                var query = @"
                               WITH AttendanceRanges AS (
                    SELECT '0 - 50' AS AttendanceRange, 0 AS MinPercentage, 50 AS MaxPercentage
                    UNION SELECT '50 - 60', 50, 60
                    UNION SELECT '60 - 70', 60, 70
                    UNION SELECT '70 - 80', 70, 80
                    UNION SELECT '80 - 85', 80, 85
                    UNION SELECT '85 - 90', 85, 90
                    UNION SELECT '90 - 100', 90, 100
                )
                
                SELECT 
                    ar.AttendanceRange,
                    COUNT(ad.AttendancePercentage) AS NumberOfStudents
                FROM AttendanceRanges ar
                LEFT JOIN (
                    SELECT 
                        sa.Student_id,
                        ROUND(AVG(CAST(CASE WHEN ast.Short_Name = 'P' THEN 1 ELSE 0 END AS FLOAT)) * 100, 2) AS AttendancePercentage
                    FROM tbl_StudentAttendanceMaster sa
                    INNER JOIN tbl_studentmaster s ON sa.Student_id = s.student_id
                    LEFT JOIN tbl_StudentAttendanceStatus ast ON sa.Student_Attendance_Status_id = ast.Student_Attendance_Status_id
                    WHERE sa.isHoliday = 0
                        AND (@ClassId = 0 OR s.class_id = @ClassId)
                        AND (@SectionId = 0 OR s.section_id = @SectionId)
                        AND sa.Date BETWEEN (SELECT Startdate FROM tbl_AcademicYear WHERE Id = @AcademicYearId)
                                        AND (SELECT Enddate FROM tbl_AcademicYear WHERE Id = @AcademicYearId)
                    GROUP BY sa.Student_id
                ) AS ad ON ad.AttendancePercentage >= ar.MinPercentage AND ad.AttendancePercentage <= ar.MaxPercentage
                GROUP BY ar.AttendanceRange
                ORDER BY ar.AttendanceRange DESC";

                // Parameters for the query
                var parameters = new { AcademicYearId = academicYearId, ClassId = classId, SectionId = sectionId };

                // Execute the query and fetch the result
                var result = await _connection.QueryAsync<MonthlyAttendanceAnalysisDTO>(query, parameters);

                return new ServiceResponse<List<MonthlyAttendanceAnalysisDTO>>(true, "Monthly attendance analysis retrieved successfully", result.AsList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<MonthlyAttendanceAnalysisDTO>>(false, $"Error: {ex.Message}", null, 500);
            }
        }
    }
}