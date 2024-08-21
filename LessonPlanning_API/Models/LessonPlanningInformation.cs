using Lesson_API.DTOs.Requests;
using System;

namespace Lesson_API.Models
{
    public class LessonPlanningInformation
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
        public List<documents> Documents { get; set; }
    }
}
