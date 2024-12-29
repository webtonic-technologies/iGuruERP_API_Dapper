using Lesson_API.DTOs.Responses;
using System;
using System.Collections.Generic;

namespace Lesson_API.DTOs.Responses
{
    public class GetAllLessonPlanningResponse
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int SectionID { get; set; }
        public string SectionName { get; set; }
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
    }

    public class GetLessonPlanningResponse1
    {
        public int LessonPlanningID { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string SubjectName { get; set; }
        public string Teacher { get; set; }
        public List<LessonDetailsResponse> Lessons { get; set; }
    } 
}
