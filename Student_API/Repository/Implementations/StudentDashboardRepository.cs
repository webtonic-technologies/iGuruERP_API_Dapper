using Dapper;
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Models;
using Student_API.Repository.Interfaces;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Student_API.Repository.Implementations
{
    public class StudentDashboardRepository : IStudentDashboardRepository
    {
        private readonly IDbConnection _dbConnection;

        public StudentDashboardRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<StudentStatisticsDTO> GetStudentStatisticsAsync()
        {
            string sql = @"
            -- Gender-wise count
            SELECT 
                g.GenderName AS Gender,
                COUNT(s.student_id) AS Count,
                CAST(COUNT(s.student_id) * 100.0 / SUM(COUNT(s.student_id)) OVER () AS DECIMAL(5, 2)) AS Percentage
            INTO #GenderWiseCount
            FROM tbl_StudentMaster s
            JOIN tbl_Gender g ON s.gender_id = g.Gender_Id
            GROUP BY g.GenderName;

            -- Status-wise count
            SELECT 
                CASE WHEN s.is_active = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                COUNT(s.student_id) AS Count,
                CAST(COUNT(s.student_id) * 100.0 / SUM(COUNT(s.student_id)) OVER () AS DECIMAL(5, 2)) AS Percentage
            INTO #StatusWiseCount
            FROM tbl_StudentMaster s
            GROUP BY s.is_active;

            -- Student Type-wise count
            SELECT 
                st.StudentTypeName AS StudentType,
                COUNT(s.student_id) AS Count,
                CAST(COUNT(s.student_id) * 100.0 / SUM(COUNT(s.student_id)) OVER () AS DECIMAL(5, 2)) AS Percentage
            INTO #StudentTypeWiseCount
            FROM tbl_StudentMaster s
            JOIN tbl_StudentType st ON s.student_type_id = st.StudentType_Id
            GROUP BY st.StudentTypeName;

            -- Select data from temp tables
            SELECT * FROM #GenderWiseCount;
            SELECT * FROM #StatusWiseCount;
            SELECT * FROM #StudentTypeWiseCount;

            -- Drop temp tables
            DROP TABLE #GenderWiseCount;
            DROP TABLE #StatusWiseCount;
            DROP TABLE #StudentTypeWiseCount;";

            using (var multi = await _dbConnection.QueryMultipleAsync(sql))
            {
                var genderCounts = multi.Read<GenderWiseStudentCountDTO>().ToList();
                var statusCounts = multi.Read<StatusWiseStudentCountDTO>().ToList();
                var studentTypeCounts = multi.Read<StudentTypeWiseCountDTO>().ToList();

                return new StudentStatisticsDTO
                {
                    GenderCounts = genderCounts,
                    StatusCounts = statusCounts,
                    StudentTypeCounts = studentTypeCounts
                };
            }
        }
        public async Task<ServiceResponse<List<StudentBirthdayDTO>>> GetTodaysBirthdaysAsync(int sectionId, int classId)
        {
            try
            {
                string sql = @"
                SELECT 
                    tbl_StudentMaster.student_id as StudentId, 
                    tbl_StudentMaster.First_Name as FirstName, 
                    tbl_StudentMaster.Last_Name as LastName, 
                    tbl_Class.class_name as ClassName, 
                    tbl_Section.section_name as SectionName, 
                    tbl_StudentMaster.date_of_birth as DateOfBirth
                FROM 
                    tbl_StudentMaster
             INNER JOIN tbl_CourseClass c ON tbl_StudentMaster.class_id = c.CourseClass_id
           INNER JOIN tbl_CourseClassSection sec ON tbl_StudentMaster.section_id = sec.CourseClassSection_id
                WHERE 
                    tbl_StudentMaster.class_id = @classId 
                    AND tbl_StudentMaster.section_id = @sectionId
                    AND MONTH(tbl_StudentMaster.date_of_birth) = MONTH(GETDATE()) 
                    AND DAY(tbl_StudentMaster.date_of_birth) = DAY(GETDATE())";


                var studentList = await _dbConnection.QueryAsync<StudentBirthdayDTO>(sql, new { classId, sectionId });

                if (studentList != null && studentList.Any())
                {
                    return new ServiceResponse<List<StudentBirthdayDTO>>(true, "Operation successful", studentList.ToList(), 200, studentList.Count());
                }
                else
                {
                    return new ServiceResponse<List<StudentBirthdayDTO>>(false, "No students found with birthdays today", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentBirthdayDTO>>(false, $"Error retrieving student birthdays: {ex.Message}", null, 500);
            }
        }
    }
}