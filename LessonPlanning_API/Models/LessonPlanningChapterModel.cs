namespace Lesson_API.Models
{
    public class LessonPlanningChapterModel
    {
        public int CurriculumChapterID { get; set; }
        public string ChapterName { get; set; }
        public int TotalSessions { get; set; }
        public string Attachment { get; set; }
        public int CurriculumID { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
