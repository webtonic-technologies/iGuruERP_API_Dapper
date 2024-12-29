


using Dapper;
using Lesson_API.DTOs.Responses;
using Lesson_API.Model;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Lesson_API.Repository.Implementations
{
    public class TeacherTrackingRepository : ITeacherTrackingRepository
    {
        private readonly IDbConnection _dbConnection;

        public TeacherTrackingRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<IEnumerable<GetTeacherTrackingResponse>> GetTeacherTrackingAsync(int instituteID, int employeeID)
        {
            string query = @"
    SELECT 
        emp.employee_id AS EmployeeID,
        CONCAT(emp.First_Name, ' ', emp.Middle_Name, ' ', emp.Last_Name) AS Teacher,
        c.class_name AS ClassName,
        s.section_name AS SectionName,
        STRING_AGG(subject.SubjectName, ', ') WITHIN GROUP (ORDER BY subject.SubjectName) AS Subjects
    FROM tbl_EmployeeProfileMaster emp
    JOIN tbl_EmployeeStappMapClassSection class_section
        ON emp.Employee_id = class_section.EmployeeId
    JOIN tbl_Subjects subject
        ON class_section.SubjectId = subject.SubjectId
    JOIN tbl_Class c
        ON class_section.ClassId = c.class_id
    JOIN tbl_Section s
        ON class_section.SectionId = s.section_id
    WHERE emp.Institute_id = @InstituteID
      AND ( 
            emp.employee_id = 0 
            OR emp.employee_id = CASE 
                                   WHEN @EmployeeID > 0 THEN @EmployeeID
                                   ELSE emp.employee_id
                                 END
          )
    GROUP BY emp.Employee_id, emp.First_Name, emp.Middle_Name, emp.Last_Name, c.class_name, s.section_name;
    ";

            // Map the result to a strongly typed list
            var result = await _dbConnection.QueryAsync<TeacherTrackingQueryResult>(query, new { InstituteID = instituteID, EmployeeID = employeeID });

            // Now perform the grouping and transformation using LINQ
            var teacherTrackingResponse = result
                .GroupBy(x => new { x.EmployeeID, x.Teacher })
                .Select(g => new GetTeacherTrackingResponse
                {
                    EmployeeID = g.Key.EmployeeID,
                    Teacher = g.Key.Teacher,
                    // Correctly map ClassSections
                    ClassSections = g.Select(x => new TTClassSection
                    {
                        ClassName = x.ClassName,
                        SectionName = x.SectionName
                    }).ToList(),
                    // Split the Subjects string into a List<string>
                    Subjects = g.Select(x => x.Subjects).FirstOrDefault()?.Split(',').Select(s => s.Trim()).ToList() ?? new List<string>()
                }).ToList();

            return teacherTrackingResponse;
        }

        public async Task<GetTeacherClassSectionSubjectResponse> GetTeacherClassSectionSubjectAsync(int employeeID)
        {
            string query = @"
                SELECT 
                    CONCAT(c.class_name, '-', s.section_name) AS ClassSection,
                    STRING_AGG(subject.SubjectName, ', ') WITHIN GROUP (ORDER BY subject.SubjectName) AS Subjects
                FROM tbl_EmployeeProfileMaster emp
                JOIN tbl_EmployeeStappMapClassSection class_section
                    ON emp.Employee_id = class_section.EmployeeId
                JOIN tbl_Subjects subject
                    ON class_section.SubjectId = subject.SubjectId
                JOIN tbl_Class c
                    ON class_section.ClassId = c.class_id
                JOIN tbl_Section s
                    ON class_section.SectionId = s.section_id
                WHERE emp.Employee_id = @EmployeeID
                GROUP BY c.class_name, s.section_name;
            ";

            var result = await _dbConnection.QueryAsync(query, new { EmployeeID = employeeID });

            // Assuming only one result is expected, otherwise you can adjust for multiple results
            var teacherClassSectionSubject = result
                .Select(x => new GetTeacherClassSectionSubjectResponse
                {
                    ClassSection = x.ClassSection,
                    Subjects = x.Subjects // Return as a single string
                }).FirstOrDefault();

            return teacherClassSectionSubject;
        }

        public async Task<IEnumerable<GetChaptersResponse>> GetChaptersAsync(int classID, int subjectID, int instituteID)
        {
            string chaptersQuery = @"
                SELECT 
                    c.CurriculumChapterID,
                    c.ChapterName,
                    MIN(l.LessonDate) AS DueDate
                FROM tblCurriculumChapter c
                JOIN tblCurriculumSubTopic s
                    ON c.CurriculumChapterID = s.CurriculumChapterID
                JOIN tblLessonPlanningInformation l
                    ON s.CurriculumSubTopicID = l.CurriculumSubTopicID
                JOIN tblCurriculum cur
                    ON s.CurriculumChapterID = cur.CurriculumID
                WHERE cur.ClassID = @ClassID
                  AND cur.SubjectID = @SubjectID
                  AND cur.InstituteID = @InstituteID
                GROUP BY c.CurriculumChapterID, c.ChapterName;
            ";

            var chapters = await _dbConnection.QueryAsync<GetChaptersResponse>(chaptersQuery, new { ClassID = classID, SubjectID = subjectID, InstituteID = instituteID });

            foreach (var chapter in chapters)
            {
                // Ensure DueDate is correctly formatted
                chapter.DueDate = FormatDueDate(chapter.DueDate);
                
                string subTopicsQuery = @"
                    SELECT
                        s.CurriculumChapterID,
                        s.CurriculumSubTopicID,
                        s.SubTopicName,
                        l.LessonDate AS DueDate
                    FROM tblCurriculumSubTopic s
                    JOIN tblLessonPlanningInformation l
                        ON s.CurriculumSubTopicID = l.CurriculumSubTopicID
                    JOIN tblCurriculum cur
                        ON s.CurriculumChapterID = cur.CurriculumID
                    WHERE s.CurriculumChapterID = @CurriculumChapterID
                    GROUP BY s.CurriculumChapterID, s.CurriculumSubTopicID, s.SubTopicName, l.LessonDate;
                ";

                var subTopics = await _dbConnection.QueryAsync<SubTopicResponse>(subTopicsQuery, new { CurriculumChapterID = chapter.CurriculumChapterID });

                foreach (var subTopic in subTopics)
                {
                    // Ensure SubTopic DueDate is correctly formatted
                    subTopic.DueDate = FormatDueDate(subTopic.DueDate);
                }

                chapter.SubTopics = subTopics.ToList();
            }

            return chapters;
        }

        // Helper method to format DueDate as "20th Jan 2025"
        private string FormatDueDate(string? date)
        {
            // If the input date string is null or empty, return an empty string
            if (string.IsNullOrEmpty(date)) return string.Empty;

            // Try to parse the string into DateTime
            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                // Return the formatted date as a string in the format "DD-MM-YYYY"
                return parsedDate.ToString("dd-MM-yyyy");
            }

            // Return empty string if the date could not be parsed
            return string.Empty;
        }

        public async Task<GetTeacherInfoResponse> GetTeacherInfoAsync(int employeeID)
        {
           
                string query = @"
                    SELECT 
                        CONCAT(emp.First_Name, ' ', emp.Middle_Name, ' ', emp.Last_Name) AS EmployeeName,
                        dept.DepartmentName AS Department,
                        desig.DesignationName AS Designation,
                        emp.EmailID,
                        emp.Employee_code_id AS EmployeeCode
                    FROM tbl_EmployeeProfileMaster emp
                    JOIN tbl_Department dept ON emp.Department_id = dept.Department_id
                    JOIN tbl_Designation desig ON emp.Designation_id = desig.Designation_id
                    WHERE emp.Employee_id = @EmployeeID;
                ";

                var result = await _dbConnection.QuerySingleOrDefaultAsync<GetTeacherInfoResponse>(query, new { EmployeeID = employeeID });

                return result;
        }
    }
}


 