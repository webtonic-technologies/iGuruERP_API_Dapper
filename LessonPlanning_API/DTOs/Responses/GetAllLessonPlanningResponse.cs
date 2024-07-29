using System;
using System.Collections.Generic;

namespace Lesson_API.DTOs.Responses
{
    public class GetAllLessonPlanningResponse
    {
        public int LessonPlanningID { get; set; }
        public string AcademicYear { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public int SectionID { get; set; }
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<LessonPlanningInformationResponse> LessonPlanningInformation { get; set; }

        public string ClassSection => $"{ClassName} - {SectionName}";
        public string SubjectTeacher => $"{SubjectName} | {TeacherName}";
    }

    public class LessonPlanningInformationResponse
    {
        public int LessonPlanningInfoID { get; set; }
        public int LessonPlanningID { get; set; }
        public DateTime LessonDate { get; set; }
        public int PlanTypeID { get; set; }
        public int CurriculumChapterID { get; set; }
        public int CurriculumSubTopicID { get; set; }
        public string Synopsis { get; set; }
        public string Introduction { get; set; }
        public string MainTeaching { get; set; }
        public string Conclusion { get; set; }
        public string Attachments { get; set; }
    }
}
