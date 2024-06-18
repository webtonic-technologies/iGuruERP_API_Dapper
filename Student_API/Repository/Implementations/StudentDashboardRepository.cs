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

        public async Task<ServiceResponse<StudentStatisticsDTO>> GetStudentStatisticsAsync()
        {
            string sql = @"
            -- Gender-wise count
            SELECT 
                g.Gender_Type AS Gender,
                COUNT(s.student_id) AS Count,
                CAST(COUNT(s.student_id) * 100.0 / SUM(COUNT(s.student_id)) OVER () AS DECIMAL(5, 2)) AS Percentage
            INTO #GenderWiseCount
            FROM tbl_StudentMaster s
            JOIN tbl_Gender g ON s.gender_id = g.Gender_Id
            GROUP BY g.GenderName;

            -- Status-wise count
            SELECT 
                CASE WHEN s.isActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                COUNT(s.student_id) AS Count,
                CAST(COUNT(s.student_id) * 100.0 / SUM(COUNT(s.student_id)) OVER () AS DECIMAL(5, 2)) AS Percentage
            INTO #StatusWiseCount
            FROM tbl_StudentMaster s
            GROUP BY s.is_active;

            -- Student Type-wise count
            SELECT 
                st.Student_Type_Name AS StudentType,
                COUNT(s.student_id) AS Count,
                CAST(COUNT(s.student_id) * 100.0 / SUM(COUNT(s.student_id)) OVER () AS DECIMAL(5, 2)) AS Percentage
            INTO #StudentTypeWiseCount
            FROM tbl_StudentOtherInfo s
            JOIN tbl_StudentType st ON s.Student_Type_id = st.StudentType_Id
            GROUP BY st.StudentTypeName;

            -- Select data from temp tables
            SELECT * FROM #GenderWiseCount;
            SELECT * FROM #StatusWiseCount;
            SELECT * FROM #StudentTypeWiseCount;

            -- Drop temp tables
            DROP TABLE #GenderWiseCount;
            DROP TABLE #StatusWiseCount;
            DROP TABLE #StudentTypeWiseCount;";

            try
            {
                using (var multi = await _dbConnection.QueryMultipleAsync(sql))
                {
                    var genderCounts = multi.Read<GenderWiseStudentCountDTO>().ToList();
                    var statusCounts = multi.Read<StatusWiseStudentCountDTO>().ToList();
                    var studentTypeCounts = multi.Read<StudentTypeWiseCountDTO>().ToList();

                    var studentStatistics = new StudentStatisticsDTO
                    {
                        GenderCounts = genderCounts,
                        StatusCounts = statusCounts,
                        StudentTypeCounts = studentTypeCounts
                    };

                    return new ServiceResponse<StudentStatisticsDTO>(true, "Operation successful", studentStatistics, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StudentStatisticsDTO>(false, $"Error retrieving student statistics: {ex.Message}", null, 500);
            }
        }

        public async Task<ServiceResponse<List<HouseWiseStudentCountDTO>>> GetHouseWiseStudentCountAsync()
        {
            string sql = @"
            SELECT 
                h.HouseName AS House,
                COUNT(s.student_id) AS Count,
                CAST(COUNT(s.student_id) * 100.0 / SUM(COUNT(s.student_id)) OVER () AS DECIMAL(5, 2)) AS Percentage
            FROM tbl_StudentMaster s
            JOIN tbl_InstituteHouse h ON s.Institute_house_id = h.Institute_house_id
            GROUP BY h.HouseName;";

            try
            {
                var houseCounts = (await _dbConnection.QueryAsync<HouseWiseStudentCountDTO>(sql)).ToList();

                return new ServiceResponse<List<HouseWiseStudentCountDTO>>(true, "Operation successful", houseCounts, 200, houseCounts.Count);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HouseWiseStudentCountDTO>>(false, $"Error retrieving house-wise student counts: {ex.Message}", null, 500);
            }
        }

        public async Task<ServiceResponse<List<StudentBirthdayDTO>>> GetTodaysBirthdaysAsync()
        {
            try
            {
                string sql = @"
                SELECT 
                    tbl_StudentMaster.student_id as StudentId, 
                    tbl_StudentMaster.First_Name as FirstName, 
                    tbl_StudentMaster.Last_Name as LastName, 
                    c.class_Course as ClassName, 
                    sec.section as SectionName, 
                    tbl_StudentMaster.date_of_birth as DateOfBirth
                FROM 
                    tbl_StudentMaster
             INNER JOIN tbl_CourseClass c ON tbl_StudentMaster.class_id = c.CourseClass_id
           INNER JOIN tbl_CourseClassSection sec ON tbl_StudentMaster.section_id = sec.CourseClassSection_id
                WHERE 
                     MONTH(tbl_StudentMaster.date_of_birth) = MONTH(GETDATE()) 
                    AND DAY(tbl_StudentMaster.date_of_birth) = DAY(GETDATE())";


                var studentList = await _dbConnection.QueryAsync<StudentBirthdayDTO>(sql);

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

        public async Task<ServiceResponse<List<ClassWiseGenderCountDTO>>> GetClassWiseGenderCountAsync()
        {
            string sql = @"
            SELECT 
                c.class_Course AS ClassName,
                g.Gender_Type AS Gender,
                COUNT(s.student_id) AS Count
            FROM tbl_StudentMaster s
            JOIN tbl_CourseClass c ON s.class_id = c.CourseClass_id
            JOIN tbl_Gender g ON s.gender_id = g.Gender_Id
            GROUP BY c.class_Course, g.Gender_Type;";

            try
            {
                var classWiseGenderCounts = (await _dbConnection.QueryAsync<ClassWiseGenderCountDTO>(sql)).ToList();

                return new ServiceResponse<List<ClassWiseGenderCountDTO>>(true, "Operation successful", classWiseGenderCounts, 200, classWiseGenderCounts.Count);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ClassWiseGenderCountDTO>>(false, $"Error retrieving class-wise gender counts: {ex.Message}", null, 500);
            }
        }

        //public async Task<ServiceResponse<List<CourseClassDTO>>> GetCourseClassesAsync()
        //{
        //    string sql = @"
        //    SELECT 
        //        CourseClass_id AS CourseClassId,
        //        class_Course AS ClassName
        //    FROM tbl_CourseClass;";

        //    try
        //    {
        //        var courseClasses = (await _dbConnection.QueryAsync<CourseClassDTO>(sql)).ToList();

        //        return new ServiceResponse<List<CourseClassDTO>>(true, "Operation successful", courseClasses, 200, courseClasses.Count);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<List<CourseClassDTO>>(false, $"Error retrieving course classes: {ex.Message}", null, 500);
        //    }
        //}

        //public async Task<ServiceResponse<List<CourseClassSectionDTO>>> GetCourseClassSectionsAsync()
        //{
        //    string sql = @"
        //    SELECT 
        //        CourseClassSection_id AS CourseClassSectionId,
        //        section AS SectionName
        //    FROM tbl_CourseClassSection;";

        //    try
        //    {
        //        var courseClassSections = (await _dbConnection.QueryAsync<CourseClassSectionDTO>(sql)).ToList();

        //        return new ServiceResponse<List<CourseClassSectionDTO>>(true, "Operation successful", courseClassSections, 200, courseClassSections.Count);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<List<CourseClassSectionDTO>>(false, $"Error retrieving course class sections: {ex.Message}", null, 500);
        //    }
        //}
    }
}


