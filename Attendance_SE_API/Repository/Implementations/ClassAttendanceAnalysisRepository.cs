using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;

namespace Attendance_SE_API.Repository.Implementations
{
    public class ClassAttendanceAnalysisRepository : IClassAttendanceAnalysisRepository
    {
        private readonly IConfiguration _config;

        public ClassAttendanceAnalysisRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<AttendanceStatisticsResponse> GetStudentAttendanceStatistics(ClassAttendanceAnalysisRequest request)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var query = @"
        WITH AttendanceStats AS (
            SELECT 
                sa.StudentID,
                COUNT(CASE WHEN sa.StatusID = 1 THEN 1 END) AS PresentCount,
                COUNT(CASE WHEN sa.StatusID = 2 THEN 1 END) AS AbsentCount,
                COUNT(sa.StatusID) AS TotalCount,
                MAX(sa.ClassID) AS ClassID,
                MAX(sa.SectionID) AS SectionID
            FROM 
                tblStudentAttendance sa
            INNER JOIN 
                tblAttendanceTypeMaster atm ON sa.AttendanceTypeID = atm.AttendanceTypeID
            WHERE 
                sa.InstituteID = @InstituteID
                AND sa.ClassID = @ClassID
                AND sa.SectionID = @SectionID
            GROUP BY 
                sa.StudentID
        )
        SELECT 
            COUNT(DISTINCT StudentID) AS TotalStudents,
            SUM(TotalCount) AS TotalWorkingDays,
            CAST(SUM(PresentCount) AS FLOAT) / NULLIF(SUM(TotalCount), 0) * 100 AS AverageAttendancePercentage,
            SUM(CASE WHEN PresentCount = TotalCount THEN 1 ELSE 0 END) AS StudentsWith100PercentAttendance,
            COUNT(CASE WHEN (TotalCount * 1.0 / 90) >= 0.8 THEN 1 END) AS StudentsAbove80PercentAttendance
        FROM 
            AttendanceStats;";

                // Pass ClassID, SectionID, and InstituteID from the request
                var result = await connection.QuerySingleOrDefaultAsync<AttendanceStatisticsResponse>(query, new
                {
                    InstituteID = request.InstituteID,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID
                });

                return result;
            }
        }

        public async Task<ServiceResponse<IEnumerable<MonthlyAttendanceAnalysisResponse>>> GetMonthlyAttendanceAnalysis(ClassAttendanceAnalysisRequest request)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var query = @"
                WITH MonthlyAttendance AS (
                    SELECT 
                        YEAR(sa.AttendanceDate) AS AttendanceYear,
                        MONTH(sa.AttendanceDate) AS AttendanceMonth,
                        COUNT(CASE WHEN sas.StatusID = 1 THEN 1 END) AS PresentCount,
                        COUNT(sas.StatusID) AS TotalCount
                    FROM 
                        tblStudentAttendance sa
                    INNER JOIN 
                        tblStudentAttendanceStatus sas ON sa.StatusID = sas.StatusID
                    WHERE 
                        sa.InstituteID = @InstituteID
                        AND sa.ClassID = @ClassID
                        AND sa.SectionID = @SectionID
                        AND sa.AttendanceDate >= DATEADD(MONTH, -12, GETDATE())  -- Last 12 months
                    GROUP BY 
                        YEAR(sa.AttendanceDate), MONTH(sa.AttendanceDate)
                )
                SELECT 
                    AttendanceYear,
                    AttendanceMonth,
                    CAST(SUM(PresentCount) AS FLOAT) / NULLIF(SUM(TotalCount), 0) * 100 AS AverageAttendancePercentage
                FROM 
                    MonthlyAttendance
                GROUP BY 
                    AttendanceYear, AttendanceMonth
                ORDER BY 
                    AttendanceYear DESC, AttendanceMonth DESC;";

                // Query returns an IEnumerable<MonthlyAttendanceAnalysisResponse>
                var result = await connection.QueryAsync<MonthlyAttendanceAnalysisResponse>(query, new
                {
                    InstituteID = request.InstituteID,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID
                });

                // Constructing the ServiceResponse with all the required parameters
                var response = new ServiceResponse<IEnumerable<MonthlyAttendanceAnalysisResponse>>(
                    success: true,
                    message: "Monthly attendance analysis fetched successfully.",
                    data: result,  // Assigning the result which is an IEnumerable
                    statusCode: 200
                );

                return response;
            }
        }

        public async Task<ServiceResponse<IEnumerable<AttendanceRangeAnalysisResponse>>> GetAttendanceRangeAnalysis(ClassAttendanceAnalysisRequest request)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var query = @"
        WITH AttendanceStatistics AS (
            SELECT 
                sa.StudentID,
                COUNT(CASE WHEN sas.StatusID = 1 THEN 1 END) AS PresentCount,
                COUNT(sas.StatusID) AS TotalCount
            FROM 
                tblStudentAttendance sa
            INNER JOIN 
                tblStudentAttendanceStatus sas ON sa.StatusID = sas.StatusID
            WHERE
                sa.InstituteID = @InstituteID
                AND sa.ClassID = @ClassID
                AND sa.SectionID = @SectionID
            GROUP BY 
                sa.StudentID
        ),
        AttendancePercentages AS (
            SELECT 
                StudentID,
                (CAST(PresentCount AS DECIMAL) / NULLIF(TotalCount, 0)) * 100 AS AttendancePercentage
            FROM 
                AttendanceStatistics
        ),
        AttendanceRanges AS (
            SELECT 
                '95 to 100%' AS AttendanceRange, 95 AS MinValue, 100 AS MaxValue
            UNION ALL
            SELECT '90 to 94.99%', 90, 94.99
            UNION ALL
            SELECT '85 to 89.99%', 85, 89.99
            UNION ALL
            SELECT '80 to 84.99%', 80, 84.99
            UNION ALL
            SELECT '70 to 79.99%', 70, 79.99
            UNION ALL
            SELECT '60 to 69.99%', 60, 69.99
            UNION ALL
            SELECT '50 to 59.99%', 50, 59.99
            UNION ALL
            SELECT '0 to 49.99%', 0, 49.99
        )
        SELECT 
            ar.AttendanceRange,
            COUNT(ap.StudentID) AS NumberOfStudents
        FROM 
            AttendanceRanges ar
        LEFT JOIN 
            AttendancePercentages ap ON ap.AttendancePercentage BETWEEN ar.MinValue AND ar.MaxValue
        GROUP BY 
            ar.AttendanceRange, ar.MinValue
        ORDER BY 
            ar.MinValue;";

                // Pass ClassID, SectionID, and InstituteID from the request to the SQL query
                var result = await connection.QueryAsync<AttendanceRangeAnalysisResponse>(query, new
                {
                    InstituteID = request.InstituteID,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID
                });

                // Return the response with IEnumerable
                return new ServiceResponse<IEnumerable<AttendanceRangeAnalysisResponse>>(
                    success: true,
                    message: "Attendance range analysis fetched successfully.",
                    data: result,
                    statusCode: 200
                );
            }
        }

        public async Task<ServiceResponse<IEnumerable<StudentDayWiseAttendanceResponse>>> GetStudentDayWiseAttendance(ClassAttendanceAnalysisRequest request)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var query = @"
                WITH AttendanceSummary AS (
                    SELECT 
                        sa.AttendanceDate,
                        COUNT(sa.StudentID) AS TotalStudents,
                        SUM(CASE WHEN sas.StatusID = 1 THEN 1 ELSE 0 END) AS PresentCount
                    FROM 
                        tblStudentAttendance sa
                    INNER JOIN 
                        tblStudentAttendanceStatus sas ON sa.StatusID = sas.StatusID
                    WHERE 
                        sa.AttendanceDate >= DATEADD(DAY, -60, GETDATE())  -- Filter for the last 60 days
                        AND sa.InstituteID = @InstituteID
                        AND sa.ClassID = @ClassID
                        AND sa.SectionID = @SectionID
                    GROUP BY 
                        sa.AttendanceDate
                ),
                AttendancePercentage AS (
                    SELECT 
                        AttendanceDate,
                        TotalStudents,
                        PresentCount,
                        CAST(PresentCount AS DECIMAL(10, 2)) / NULLIF(TotalStudents, 0) * 100 AS AttendancePercentage
                    FROM 
                        AttendanceSummary
                )

                SELECT 
                    AttendanceDate,
                    AttendancePercentage
                FROM 
                    AttendancePercentage
                ORDER BY 
                    AttendanceDate DESC;";

                var result = await connection.QueryAsync<StudentDayWiseAttendanceResponse>(query, new
                {
                    InstituteID = request.InstituteID,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID
                });

                return new ServiceResponse<IEnumerable<StudentDayWiseAttendanceResponse>>(
                    success: true,
                    message: "Student day-wise attendance fetched successfully.",
                    data: result,
                    statusCode: 200
                );
            }
        }

        public async Task<ServiceResponse<IEnumerable<StudentAttendanceAnalysisResponse>>> GetStudentsAttendanceAnalysis(ClassAttendanceAnalysisRequest request)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                // Calculate offset
                int offset = (request.PageNumber - 1) * request.PageSize;

                var query = @"
        WITH AttendanceSummary AS (
            SELECT 
                sm.student_id AS StudentID,
                sm.Admission_Number AS AdmissionNumber,
                CONCAT(sm.First_Name, ' ', COALESCE(sm.Middle_Name, ''), ' ', sm.Last_Name) AS StudentName,
                COUNT(sa.AttendanceID) AS TotalAttendance,
                SUM(CASE WHEN sas.StatusID = 1 THEN 1 ELSE 0 END) AS PresentCount
            FROM 
                tbl_StudentMaster sm
            LEFT JOIN 
                tblStudentAttendance sa ON sm.student_id = sa.StudentID
            LEFT JOIN 
                tblStudentAttendanceStatus sas ON sa.StatusID = sas.StatusID
            WHERE 
                sm.Institute_id = @InstituteID
                AND sm.class_id = @ClassID
                AND sm.section_id = @SectionID
            GROUP BY 
                sm.student_id, sm.Admission_Number, sm.First_Name, sm.Middle_Name, sm.Last_Name
        ),
        TotalCount AS (
            SELECT COUNT(*) AS TotalRecords FROM AttendanceSummary
        )
        SELECT 
            ROW_NUMBER() OVER (ORDER BY 
                (CASE 
                    WHEN TotalAttendance = 0 THEN 0 
                    ELSE (CAST(PresentCount AS DECIMAL(10, 2)) / NULLIF(TotalAttendance, 0) * 100) 
                END) DESC
            ) AS [SNo],
            a.AdmissionNumber,
            a.StudentName,
            a.TotalAttendance,
            a.PresentCount AS TotalAttended,
            CASE 
                WHEN a.TotalAttendance = 0 THEN 0
                ELSE (CAST(a.PresentCount AS DECIMAL(10, 2)) / NULLIF(a.TotalAttendance, 0) * 100)
            END AS AttendancePercentage
        FROM 
            AttendanceSummary AS a
        CROSS JOIN 
            TotalCount AS tc
        ORDER BY 
            [SNo]
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                var result = await connection.QueryAsync<StudentAttendanceAnalysisResponse>(query, new
                {
                    InstituteID = request.InstituteID,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID,
                    Offset = offset,
                    PageSize = request.PageSize
                });

                // Get total count separately
                var totalCountQuery = @"
        WITH AttendanceSummary AS (
            SELECT 
                sm.student_id
            FROM 
                tbl_StudentMaster sm
            LEFT JOIN 
                tblStudentAttendance sa ON sm.student_id = sa.StudentID
            WHERE 
                sm.Institute_id = @InstituteID
                AND sm.class_id = @ClassID
                AND sm.section_id = @SectionID
        )
        SELECT COUNT(*) AS TotalRecords FROM AttendanceSummary;";

                var totalCount = await connection.ExecuteScalarAsync<int>(totalCountQuery, new
                {
                    InstituteID = request.InstituteID,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID
                });

                return new ServiceResponse<IEnumerable<StudentAttendanceAnalysisResponse>>(
                    success: true,
                    message: "Students attendance analysis fetched successfully.",
                    data: result,
                    statusCode: 200,
                    totalCount: totalCount
                );
            }
        }


        //public async Task<ServiceResponse<IEnumerable<StudentAttendanceAnalysisResponse>>> GetStudentsAttendanceAnalysis(ClassAttendanceAnalysisRequest request)
        //{
        //    using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        //    {
        //        var query = @"
        //WITH AttendanceSummary AS (
        //    SELECT 
        //        sm.student_id AS StudentID,
        //        sm.Admission_Number AS AdmissionNumber,
        //        CONCAT(sm.First_Name, ' ', COALESCE(sm.Middle_Name, ''), ' ', sm.Last_Name) AS StudentName,
        //        COUNT(sa.AttendanceID) AS TotalAttendance,
        //        SUM(CASE WHEN sas.StatusID = 1 THEN 1 ELSE 0 END) AS PresentCount
        //    FROM 
        //        tbl_StudentMaster sm
        //    LEFT JOIN 
        //        tblStudentAttendance sa ON sm.student_id = sa.StudentID
        //    LEFT JOIN 
        //        tblStudentAttendanceStatus sas ON sa.StatusID = sas.StatusID
        //    WHERE 
        //        sm.Institute_id = @InstituteID
        //        AND sm.class_id = @ClassID
        //        AND sm.section_id = @SectionID
        //    GROUP BY 
        //        sm.student_id, sm.Admission_Number, sm.First_Name, sm.Middle_Name, sm.Last_Name
        //)

        //SELECT 
        //    ROW_NUMBER() OVER (ORDER BY 
        //        (CAST(PresentCount AS DECIMAL(10, 2)) / NULLIF(TotalAttendance, 0) * 100) DESC
        //    ) AS [S.No],
        //    AdmissionNumber,
        //    StudentName,
        //    TotalAttendance,
        //    PresentCount AS TotalAttended,
        //    -- Handle cases where TotalAttendance is 0 (i.e., no attendance records)
        //    CASE 
        //        WHEN TotalAttendance = 0 THEN 0
        //        ELSE (CAST(PresentCount AS DECIMAL(10, 2)) / NULLIF(TotalAttendance, 0) * 100)
        //    END AS AttendancePercentage
        //FROM 
        //    AttendanceSummary
        //ORDER BY 
        //    AttendancePercentage DESC;";

        //        var result = await connection.QueryAsync<StudentAttendanceAnalysisResponse>(query, new
        //        {
        //            InstituteID = request.InstituteID,
        //            ClassID = request.ClassID,
        //            SectionID = request.SectionID
        //        });

        //        return new ServiceResponse<IEnumerable<StudentAttendanceAnalysisResponse>>(
        //            success: true,
        //            message: "Students attendance analysis fetched successfully.",
        //            data: result,
        //            statusCode: 200
        //        );
        //    }
        //}

        public async Task<IEnumerable<dynamic>> GetStudentsAttendanceAnalysisForExport(GetStudentsAttendanceAnalysisExcelExportRequest request)
        {
            var query = @"
        WITH AttendanceSummary AS (
            SELECT 
                sm.student_id AS StudentID,
                sm.Admission_Number AS AdmissionNumber,
                CONCAT(sm.First_Name, ' ', COALESCE(sm.Middle_Name, ''), ' ', sm.Last_Name) AS StudentName,
                COUNT(sa.AttendanceID) AS TotalAttendance,
                SUM(CASE WHEN sas.StatusID = 1 THEN 1 ELSE 0 END) AS PresentCount
            FROM 
                tbl_StudentMaster sm
            LEFT JOIN 
                tblStudentAttendance sa ON sm.student_id = sa.StudentID
            LEFT JOIN 
                tblStudentAttendanceStatus sas ON sa.StatusID = sas.StatusID
            WHERE 
                sm.Institute_id = @InstituteID
                AND sm.class_id = @ClassID
                AND sm.section_id = @SectionID
            GROUP BY 
                sm.student_id, sm.Admission_Number, sm.First_Name, sm.Middle_Name, sm.Last_Name
        )
        SELECT 
            ROW_NUMBER() OVER (ORDER BY 
                (CAST(PresentCount AS DECIMAL(10, 2)) / NULLIF(TotalAttendance, 0) * 100) DESC
            ) AS [S.No],
            AdmissionNumber,
            StudentName,
            TotalAttendance,
            PresentCount AS TotalAttended,
            CASE 
                WHEN TotalAttendance = 0 THEN 0
                ELSE (CAST(PresentCount AS DECIMAL(10, 2)) / NULLIF(TotalAttendance, 0) * 100)
            END AS AttendancePercentage
        FROM 
            AttendanceSummary
        ORDER BY 
            AttendancePercentage DESC;";

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return await connection.QueryAsync<dynamic>(query, new
            {
                request.InstituteID,
                request.ClassID,
                request.SectionID
            });
        }

    }
}
