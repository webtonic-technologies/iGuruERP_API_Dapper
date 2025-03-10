﻿using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;

namespace Attendance_SE_API.Repository.Implementations
{
    public class SubjectAttendanceAnalysisRepository : ISubjectAttendanceAnalysisRepository
    {
        private readonly IConfiguration _config;

        public SubjectAttendanceAnalysisRepository(IConfiguration config)
        {
            _config = config;
        }


        public async Task<SubjectAttendanceStatisticsResponse> GetStudentAttendanceStatisticsForSubject(SubjectAttendanceAnalysisRequest request)
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
                        MAX(sa.SectionID) AS SectionID,
                        MAX(sa.SubjectID) AS SubjectID
                    FROM 
                        tblStudentAttendance sa
                    INNER JOIN 
                        tblAttendanceTypeMaster atm ON sa.AttendanceTypeID = atm.AttendanceTypeID
                    WHERE 
                        sa.InstituteID = @InstituteID
                        AND sa.ClassID = @ClassID
                        AND sa.SectionID IN @SectionIDs   -- Modified to support multiple sections
                        AND sa.SubjectID IN @SubjectIDs   -- Modified to support multiple subjects
                    GROUP BY 
                        sa.StudentID
                )
                SELECT 
                    SUM(TotalCount) AS TotalSessions, -- Total number of attendance entries (sessions)
                    SUM(TotalCount) AS TotalWorkingDays, -- Retained to align with TotalSessions logic
                    CAST(SUM(PresentCount) AS FLOAT) / NULLIF(SUM(TotalCount), 0) * 100 AS AverageAttendancePercentage,
                    SUM(CASE WHEN PresentCount = TotalCount THEN 1 ELSE 0 END) AS StudentsWith100PercentAttendance,
                    COUNT(CASE WHEN (TotalCount * 1.0 / 90) >= 0.8 THEN 1 END) AS StudentsAbove80PercentAttendance
                FROM 
                    AttendanceStats;";

                // Execute the query with the provided request parameters
                var result = await connection.QuerySingleOrDefaultAsync<SubjectAttendanceStatisticsResponse>(query, new
                {
                    InstituteID = request.InstituteID,
                    ClassID = request.ClassID,
                    SectionIDs = request.SectionIDs,  // Pass the list of SectionIDs
                    SubjectIDs = request.SubjectIDs   // Pass the list of SubjectIDs
                });

                return result;
            }
        }



        //public async Task<SubjectAttendanceStatisticsResponse> GetStudentAttendanceStatisticsForSubject(SubjectAttendanceAnalysisRequest request)
        //{
        //    using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        //    {
        //        var query = @"
        //WITH AttendanceStats AS (
        //    SELECT 
        //        sa.StudentID,
        //        COUNT(CASE WHEN sa.StatusID = 1 THEN 1 END) AS PresentCount,
        //        COUNT(CASE WHEN sa.StatusID = 2 THEN 1 END) AS AbsentCount,
        //        COUNT(sa.StatusID) AS TotalCount,
        //        MAX(sa.ClassID) AS ClassID,
        //        MAX(sa.SectionID) AS SectionID,
        //        MAX(sa.SubjectID) AS SubjectID
        //    FROM 
        //        tblStudentAttendance sa
        //    INNER JOIN 
        //        tblAttendanceTypeMaster atm ON sa.AttendanceTypeID = atm.AttendanceTypeID
        //    WHERE 
        //        sa.InstituteID = @InstituteID
        //        AND sa.ClassID = @ClassID
        //        AND sa.SectionID IN @SectionIDs   -- Modified to support multiple sections
        //        AND sa.SubjectID IN @SubjectIDs   -- Modified to support multiple subjects
        //    GROUP BY 
        //        sa.StudentID
        //)
        //SELECT 
        //    COUNT(DISTINCT StudentID) AS TotalStudents,
        //    SUM(TotalCount) AS TotalWorkingDays,
        //    CAST(SUM(PresentCount) AS FLOAT) / NULLIF(SUM(TotalCount), 0) * 100 AS AverageAttendancePercentage,
        //    SUM(CASE WHEN PresentCount = TotalCount THEN 1 ELSE 0 END) AS StudentsWith100PercentAttendance,
        //    COUNT(CASE WHEN (TotalCount * 1.0 / 90) >= 0.8 THEN 1 END) AS StudentsAbove80PercentAttendance
        //FROM 
        //    AttendanceStats;";

        //        var result = await connection.QuerySingleOrDefaultAsync<SubjectAttendanceStatisticsResponse>(query, new
        //        {
        //            InstituteID = request.InstituteID,
        //            ClassID = request.ClassID,
        //            SectionIDs = request.SectionIDs,  // Pass the list of SectionIDs
        //            SubjectIDs = request.SubjectIDs   // Pass the list of SubjectIDs
        //        });

        //        return result;
        //    }
        //}




        public async Task<ServiceResponse<IEnumerable<MonthlyAttendanceSubjectAnalysisResponse>>> GetMonthlyAttendanceAnalysisForSubject(SubjectAttendanceAnalysisRequest request)
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
                AND sa.SectionID IN @SectionIDs   -- Modified to support multiple SectionIDs
                AND sa.SubjectID IN @SubjectIDs   -- Modified to support multiple SubjectIDs
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

                var result = await connection.QueryAsync<MonthlyAttendanceSubjectAnalysisResponse>(query, new
                {
                    InstituteID = request.InstituteID,
                    ClassID = request.ClassID,
                    SectionIDs = request.SectionIDs,  // Pass the list of SectionIDs
                    SubjectIDs = request.SubjectIDs   // Pass the list of SubjectIDs
                });

                return new ServiceResponse<IEnumerable<MonthlyAttendanceSubjectAnalysisResponse>>(
                    success: true,
                    message: "Monthly attendance analysis for subjects fetched successfully.",
                    data: result,
                    statusCode: 200
                );
            }
        }


        //public async Task<ServiceResponse<IEnumerable<MonthlyAttendanceSubjectAnalysisResponse>>> GetMonthlyAttendanceAnalysisForSubject(SubjectAttendanceAnalysisRequest request)
        //{
        //    using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        //    {
        //        var query = @"
        //        WITH MonthlyAttendance AS (
        //            SELECT 
        //                YEAR(sa.AttendanceDate) AS AttendanceYear,
        //                MONTH(sa.AttendanceDate) AS AttendanceMonth,
        //                COUNT(CASE WHEN sas.StatusID = 1 THEN 1 END) AS PresentCount,
        //                COUNT(sas.StatusID) AS TotalCount
        //            FROM 
        //                tblStudentAttendance sa
        //            INNER JOIN 
        //                tblStudentAttendanceStatus sas ON sa.StatusID = sas.StatusID
        //            WHERE 
        //                sa.InstituteID = @InstituteID
        //                AND sa.ClassID = @ClassID
        //                AND sa.SectionID = @SectionID
        //                AND sa.SubjectID = @SubjectID
        //                AND sa.AttendanceDate >= DATEADD(MONTH, -12, GETDATE())  -- Last 12 months
        //            GROUP BY 
        //                YEAR(sa.AttendanceDate), MONTH(sa.AttendanceDate)
        //        )
        //        SELECT 
        //            AttendanceYear,
        //            AttendanceMonth,
        //            CAST(SUM(PresentCount) AS FLOAT) / NULLIF(SUM(TotalCount), 0) * 100 AS AverageAttendancePercentage
        //        FROM 
        //            MonthlyAttendance
        //        GROUP BY 
        //            AttendanceYear, AttendanceMonth
        //        ORDER BY 
        //            AttendanceYear DESC, AttendanceMonth DESC;";

        //        var result = await connection.QueryAsync<MonthlyAttendanceSubjectAnalysisResponse>(query, new
        //        {
        //            InstituteID = request.InstituteID,
        //            ClassID = request.ClassID,
        //            SectionID = request.SectionID,
        //            SubjectID = request.SubjectID
        //        });

        //        return new ServiceResponse<IEnumerable<MonthlyAttendanceSubjectAnalysisResponse>>(
        //            success: true,
        //            message: "Monthly attendance analysis for subject fetched successfully.",
        //            data: result,
        //            statusCode: 200
        //        );
        //    }
        //}


        public async Task<ServiceResponse<IEnumerable<SubjectsAttendanceAnalysisResponse>>> GetSubjectsAttendanceAnalysis(SubjectAttendanceAnalysisRequest1 request)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                // Add ROW_NUMBER() for pagination and calculate total records
                var query = @"
WITH AttendanceSummary AS (
    SELECT 
        sm.student_id AS StudentID,
        sm.Admission_Number AS AdmissionNumber,
        CONCAT(sm.First_Name, ' ', COALESCE(sm.Middle_Name, ''), ' ', sm.Last_Name) AS StudentName,
        sa.SubjectID,
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
        AND sm.section_id IN @SectionIDs
        AND sa.SubjectID IN @SubjectIDs
    GROUP BY 
        sm.student_id, sm.Admission_Number, sm.First_Name, sm.Middle_Name, sm.Last_Name, sa.SubjectID
),
Subjects AS (
    SELECT 
        SubjectID,
        SubjectName
    FROM 
        tbl_Subjects
    WHERE 
        InstituteId = @InstituteID
),
PaginatedResults AS (
    SELECT 
        ROW_NUMBER() OVER (ORDER BY 
            (CASE WHEN TotalAttendance = 0 THEN 0 
                  ELSE (CAST(PresentCount AS DECIMAL(10, 2)) / NULLIF(TotalAttendance, 0) * 100) 
            END) DESC) AS RowNum,
        a.StudentID,
        a.StudentName,
        a.AdmissionNumber,
        s.SubjectID,
        s.SubjectName,
        TotalAttendance,
        PresentCount AS TotalAttended,
        CASE 
            WHEN TotalAttendance = 0 THEN 0
            ELSE (CAST(PresentCount AS DECIMAL(10, 2)) / NULLIF(TotalAttendance, 0) * 100)
        END AS AttendancePercentage
    FROM 
        AttendanceSummary AS a
    JOIN 
        Subjects AS s ON a.SubjectID = s.SubjectID
),
TotalCount AS (
    SELECT COUNT(*) AS TotalRecords FROM PaginatedResults
)
SELECT 
    p.StudentID,
    p.StudentName,
    p.AdmissionNumber,
    p.SubjectID,
    p.SubjectName,
    p.TotalAttendance,
    p.TotalAttended,
    p.AttendancePercentage,
    tc.TotalRecords
FROM 
    PaginatedResults AS p
CROSS JOIN 
    TotalCount AS tc
WHERE 
    p.RowNum BETWEEN @StartRow AND @EndRow
ORDER BY 
    p.RowNum;";

                // Calculate start and end rows for pagination
                int startRow = (request.PageNumber - 1) * request.PageSize + 1;
                int endRow = request.PageNumber * request.PageSize;

                var result = await connection.QueryAsync<dynamic>(query, new
                {
                    request.InstituteID,
                    request.ClassID,
                    SectionIDs = request.SectionIDs,
                    SubjectIDs = request.SubjectIDs,
                    StartRow = startRow,
                    EndRow = endRow
                });

                // Extract total count from the first row
                int totalCount = result.Any() ? result.First().TotalRecords : 0;

                // Process data at the code level to replace null values with 0
                var groupedResults = result
                    .GroupBy(r => new { r.StudentID, r.StudentName, r.AdmissionNumber })
                    .Select(g => new SubjectsAttendanceAnalysisResponse
                    {
                        StudentID = g.Key.StudentID,
                        StudentName = g.Key.StudentName,
                        AdmissionNumber = g.Key.AdmissionNumber,
                        Subjects = g.ToDictionary(
                            x => (string)x.SubjectName,  // Explicitly cast SubjectName to string
                            x => new SubjectAttendanceDetails
                            {
                                TotalAttendance = x.TotalAttendance ?? 0, // Ensure null is replaced with 0
                                TotalAttended = x.TotalAttended ?? 0, // Ensure null is replaced with 0
                                AttendancePercentage = x.AttendancePercentage ?? 0 // Ensure null is replaced with 0
                            })
                    }).ToList();

                return new ServiceResponse<IEnumerable<SubjectsAttendanceAnalysisResponse>>(
                    success: true,
                    message: "Subjects attendance analysis fetched successfully.",
                    data: groupedResults,
                    statusCode: 200,
                    totalCount: totalCount // Include total record count
                );
            }
        }



        ////Working ------------------ GetSubjectsAttendanceAnalysis
        //public async Task<ServiceResponse<IEnumerable<SubjectsAttendanceAnalysisResponse>>> GetSubjectsAttendanceAnalysis(SubjectAttendanceAnalysisRequest1 request)
        //{
        //    using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        //    {
        //        var query = @"
        //WITH AttendanceSummary AS (
        //    SELECT 
        //        sm.student_id AS StudentID,
        //        sm.Admission_Number AS AdmissionNumber,
        //        CONCAT(sm.First_Name, ' ', COALESCE(sm.Middle_Name, ''), ' ', sm.Last_Name) AS StudentName,
        //        sa.SubjectID,
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
        //        sm.student_id, sm.Admission_Number, sm.First_Name, sm.Middle_Name, sm.Last_Name, sa.SubjectID
        //),
        //Subjects AS (
        //    SELECT 
        //        SubjectID,
        //        SubjectName
        //    FROM 
        //        tbl_Subjects
        //    WHERE 
        //        InstituteId = @InstituteID
        //)
        //SELECT 
        //    StudentID,
        //    StudentName,
        //    AdmissionNumber,
        //    s.SubjectID,
        //    s.SubjectName,
        //    TotalAttendance,
        //    PresentCount AS TotalAttended,
        //    CASE 
        //        WHEN TotalAttendance = 0 THEN 0
        //        ELSE (CAST(PresentCount AS DECIMAL(10, 2)) / NULLIF(TotalAttendance, 0) * 100)
        //    END AS AttendancePercentage
        //FROM 
        //    AttendanceSummary AS a
        //JOIN 
        //    Subjects AS s ON a.SubjectID = s.SubjectID
        //ORDER BY 
        //    AttendancePercentage DESC;";

        //        var result = await connection.QueryAsync<dynamic>(query, new
        //        {
        //            InstituteID = request.InstituteID,
        //            ClassID = request.ClassID,
        //            SectionID = request.SectionID
        //        });

        //        // Process data at the code level to replace null values with 0
        //        var groupedResults = result
        //            .GroupBy(r => new { r.StudentID, r.StudentName, r.AdmissionNumber })
        //            .Select(g => new SubjectsAttendanceAnalysisResponse
        //            {
        //                StudentID = g.Key.StudentID,
        //                StudentName = g.Key.StudentName,
        //                AdmissionNumber = g.Key.AdmissionNumber,
        //                Subjects = g.ToDictionary(
        //                    x => (string)x.SubjectName,  // Explicitly cast SubjectName to string
        //                    x => new SubjectAttendanceDetails
        //                    {
        //                        TotalAttendance = x.TotalAttendance ?? 0, // Ensure null is replaced with 0
        //                        TotalAttended = x.TotalAttended ?? 0, // Ensure null is replaced with 0
        //                        AttendancePercentage = x.AttendancePercentage ?? 0 // Ensure null is replaced with 0
        //                    })
        //            }).ToList();

        //        return new ServiceResponse<IEnumerable<SubjectsAttendanceAnalysisResponse>>(
        //            success: true,
        //            message: "Subjects attendance analysis fetched successfully.",
        //            data: groupedResults,
        //            statusCode: 200
        //        );
        //    }
        //}
        ////Working ------------------ GetSubjectsAttendanceAnalysis

        //public async Task<ServiceResponse<IEnumerable<SubjectsAttendanceAnalysisResponse>>> GetSubjectsAttendanceAnalysis(SubjectAttendanceAnalysisRequest request)
        //{
        //    using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        //    {
        //        var query = @"
        //WITH AttendanceSummary AS (
        //    SELECT 
        //        sm.student_id AS StudentID,
        //        sm.Admission_Number AS AdmissionNumber,
        //        CONCAT(sm.First_Name, ' ', COALESCE(sm.Middle_Name, ''), ' ', sm.Last_Name) AS StudentName,
        //        sa.SubjectID,
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
        //        sm.student_id, sm.Admission_Number, sm.First_Name, sm.Middle_Name, sm.Last_Name, sa.SubjectID
        //),
        //Subjects AS (
        //    SELECT 
        //        SubjectID,
        //        SubjectName
        //    FROM 
        //        tbl_Subjects
        //    WHERE 
        //        InstituteId = @InstituteID
        //)
        //SELECT 
        //    StudentID,
        //    StudentName,
        //    AdmissionNumber,
        //    s.SubjectID,
        //    s.SubjectName,
        //    TotalAttendance,
        //    PresentCount AS TotalAttended,
        //    CASE 
        //        WHEN TotalAttendance = 0 THEN 0
        //        ELSE (CAST(PresentCount AS DECIMAL(10, 2)) / NULLIF(TotalAttendance, 0) * 100)
        //    END AS AttendancePercentage
        //FROM 
        //    AttendanceSummary AS a
        //JOIN 
        //    Subjects AS s ON a.SubjectID = s.SubjectID
        //ORDER BY 
        //    AttendancePercentage DESC;";

        //        var result = await connection.QueryAsync<dynamic>(query, new
        //        {
        //            InstituteID = request.InstituteID,
        //            ClassID = request.ClassID,
        //            SectionID = request.SectionID,
        //            SubjectID = request.SubjectID
        //        });

        //        // Format data into the desired response structure
        //        var groupedResults = result
        //            .GroupBy(r => new { StudentID = r.StudentID, StudentName = r.StudentName, AdmissionNumber = r.AdmissionNumber })
        //            .Select(g => new SubjectsAttendanceAnalysisResponse
        //            {
        //                StudentID = g.Key.StudentID,
        //                StudentName = g.Key.StudentName,
        //                AdmissionNumber = g.Key.AdmissionNumber,
        //                Subjects = g.ToDictionary(
        //                    x => (string)x.SubjectName,  // Explicitly cast SubjectName to string
        //                    x => new SubjectAttendanceDetails
        //                    {
        //                        TotalAttendance = x.TotalAttendance,
        //                        TotalAttended = x.TotalAttended,
        //                        AttendancePercentage = x.AttendancePercentage
        //                    })
        //            }).ToList();

        //        return new ServiceResponse<IEnumerable<SubjectsAttendanceAnalysisResponse>>(
        //            success: true,
        //            message: "Subjects attendance analysis fetched successfully.",
        //            data: groupedResults,
        //            statusCode: 200
        //        );
        //    }
        //}

        public async Task<IEnumerable<dynamic>> GetExportableSubjectsAttendanceData(GetSubjectsAttendanceAnalysisExportRequest request)
        {
            var query = @"
        WITH AttendanceSummary AS (
            SELECT 
                sm.student_id AS StudentID,
                sm.Admission_Number AS AdmissionNumber,
                CONCAT(sm.First_Name, ' ', COALESCE(sm.Middle_Name, ''), ' ', sm.Last_Name) AS StudentName,
                sa.SubjectID,
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
                AND sm.section_id IN @SectionIDs
                AND sa.SubjectID IN @SubjectIDs
            GROUP BY 
                sm.student_id, sm.Admission_Number, sm.First_Name, sm.Middle_Name, sm.Last_Name, sa.SubjectID
        ),
        Subjects AS (
            SELECT 
                SubjectID,
                SubjectName
            FROM 
                tbl_Subjects
            WHERE 
                InstituteId = @InstituteID
        )
        SELECT 
            StudentID,
            StudentName,
            AdmissionNumber,
            s.SubjectID,
            s.SubjectName,
            TotalAttendance,
            PresentCount AS TotalAttended,
            CASE 
                WHEN TotalAttendance = 0 THEN 0
                ELSE (CAST(PresentCount AS DECIMAL(10, 2)) / NULLIF(TotalAttendance, 0) * 100)
            END AS AttendancePercentage
        FROM 
            AttendanceSummary AS a
        JOIN 
            Subjects AS s ON a.SubjectID = s.SubjectID
        ORDER BY 
            AttendancePercentage DESC;";

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                return await connection.QueryAsync<dynamic>(query, new
                {
                    request.InstituteID,
                    request.ClassID,
                    SectionIDs = request.SectionIDs,
                    SubjectIDs = request.SubjectIDs
                });
            }
        }

    }
}
