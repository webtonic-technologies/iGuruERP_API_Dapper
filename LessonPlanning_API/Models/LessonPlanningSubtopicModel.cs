namespace Lesson_API.Models
{
    public class LessonPlanningSubtopicModel
    {
        public int CurriculumSubTopicID { get; set; }
        public int CurriculumChapterID { get; set; }
        public string SubTopicName { get; set; }
        public int TotalSession { get; set; }
        public string Attachment { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
