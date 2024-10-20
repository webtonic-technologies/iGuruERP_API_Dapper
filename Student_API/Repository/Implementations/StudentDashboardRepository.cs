﻿using Dapper;
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

        public async Task<ServiceResponse<StudentStatisticsDTO>> GetStudentStatisticsAsync(int Institute_id)
        {
            string sql = @"
            -- Gender-wise count
            
            SELECT 
                g.Gender_Type AS Gender,
                COALESCE(COUNT(s.student_id), 0) AS Count,
                CAST(
                    COALESCE(COUNT(s.student_id), 0) * 100.0 / 
                    (SELECT COUNT(student_id) FROM tbl_StudentMaster WHERE Institute_id =@Institute_id)
                    AS DECIMAL(5, 2)
                ) AS Percentage
            INTO #GenderWiseCount
            FROM tbl_Gender g
            LEFT JOIN tbl_StudentMaster s 
                ON s.gender_id = g.Gender_Id AND s.Institute_id = @Institute_id
            GROUP BY g.Gender_Type;

            -- Status-wise count
            SELECT 
                CASE WHEN s.isActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                COUNT(s.student_id) AS Count,
                CAST(COUNT(s.student_id) * 100.0 / SUM(COUNT(s.student_id)) OVER () AS DECIMAL(5, 2)) AS Percentage
            INTO #StatusWiseCount
            FROM tbl_StudentMaster s
            WHERE s.Institute_id = @Institute_id
            GROUP BY s.isActive;

            -- Student Type-wise count
            SELECT 
                st.Student_Type_Name AS StudentType,
                COUNT(s.student_id) AS Count,
                CAST(COUNT(s.student_id) * 100.0 / SUM(COUNT(s.student_id)) OVER () AS DECIMAL(5, 2)) AS Percentage
            INTO #StudentTypeWiseCount
            FROM tbl_StudentOtherInfo s
            JOIN tbl_StudentMaster sm ON sm.student_id = s.student_id
            JOIN tbl_StudentType st ON sm.StudentType_Id = st.Student_Type_id 
            WHERE sm.Institute_id = @Institute_id
            GROUP BY st.Student_Type_Name;

            SELECT 
            COUNT(DISTINCT CASE WHEN IsAppUser = 1 THEN UserId END) AS appUserCount,
            COUNT(DISTINCT CASE WHEN IsAppUser = 0 THEN UserId END) AS nonAppUserCount
            INTO #UserCounts
            FROM tblUserLogs
            WHERE Institute_id = @Institute_id AND UserTypeId = 2;

            -- Select data from temp tables
            SELECT * FROM #GenderWiseCount;
            SELECT * FROM #StatusWiseCount;
            SELECT * FROM #StudentTypeWiseCount;
            select * from #UserCounts;

            -- Drop temp tables
            DROP TABLE #GenderWiseCount;
            DROP TABLE #StatusWiseCount;
            DROP TABLE #StudentTypeWiseCount;
            DROP TABLE #UserCounts;
";

            try
            {
                using (var multi = await _dbConnection.QueryMultipleAsync(sql,new{ Institute_id = Institute_id}))
                {
                    var genderCounts = multi.Read<GenderWiseStudentCountDTO>().ToList();
                    var statusCounts = multi.Read<StatusWiseStudentCountDTO>().ToList();
                    var studentTypeCounts = multi.Read<StudentTypeWiseCountDTO>().ToList();
                    var appNonAppUserCount = multi.Read<AppNonAppUserCountDTO>().ToList();

                    GenderWiseStudentDTO genderWiseStudentDTO = new GenderWiseStudentDTO();
                    genderWiseStudentDTO.genderWiseStudentCounts = genderCounts;
                    genderWiseStudentDTO.TotalStudentCount = genderCounts.Sum(g => g.Count);
                    var studentStatistics = new StudentStatisticsDTO
                    {
                        GenderCounts = genderWiseStudentDTO,
                        StatusCounts = statusCounts,
                        StudentTypeCounts = studentTypeCounts,
                        appNonAppUserCount = appNonAppUserCount.FirstOrDefault()
                    };

                    return new ServiceResponse<StudentStatisticsDTO>(true, "Operation successful", studentStatistics, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StudentStatisticsDTO>(false, $"Error retrieving student statistics: {ex.Message}", null, 500);
            }
        }

        public async Task<ServiceResponse<List<HouseWiseStudentCountDTO>>> GetHouseWiseStudentCountAsync(int Institute_id)
        {
            string sql = @"
            SELECT 
                h.HouseName AS House,
                COUNT(s.student_id) AS Count,
                CAST(COUNT(s.student_id) * 100.0 / SUM(COUNT(s.student_id)) OVER () AS DECIMAL(5, 2)) AS Percentage
            FROM tbl_StudentMaster s
            --JOIN tbl_StudentOtherInfo so on s.student_id = so.student_id
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

        public async Task<ServiceResponse<List<StudentBirthdayDTO>>> GetTodaysBirthdaysAsync(int Institute_id)
        {
            try
            {
                string sql = @"
                SELECT 
                    tbl_StudentMaster.student_id as StudentId, 
                    tbl_StudentMaster.First_Name as FirstName, 
                    tbl_StudentMaster.Last_Name as LastName, 
                    c.class_name as ClassName, 
                    sec.section_name as SectionName, 
                    tbl_StudentMaster.date_of_birth as DateOfBirth
                FROM 
                    tbl_StudentMaster
             INNER JOIN tbl_Class c ON tbl_StudentMaster.class_id = c.Class_id
           INNER JOIN tbl_Section sec ON tbl_StudentMaster.section_id = sec.Section_id
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

        public async Task<ServiceResponse<List<ClassWiseGenderCountDTO>>> GetClassWiseGenderCountAsync(int Institute_id)
        {
            string sql = @"
            SELECT 
                c.class_name AS ClassName,
                SUM(CASE WHEN g.Gender_id = 1 THEN 1 ELSE 0 END) AS BoysCount,
                SUM(CASE WHEN g.Gender_id = 2 THEN 1 ELSE 0 END) AS GirlsCount
            FROM tbl_StudentMaster s
            JOIN tbl_Class c ON s.class_id = c.Class_id
            JOIN tbl_Gender g ON s.gender_id = g.Gender_Id
            WHERE s.Institute_id = @Institute_id
            GROUP BY c.class_name;";

            try
            {
                var parameters = new { Institute_id = Institute_id };
                var classWiseGenderCounts = (await _dbConnection.QueryAsync<ClassWiseGenderCountDTO>(sql, parameters)).ToList();

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


