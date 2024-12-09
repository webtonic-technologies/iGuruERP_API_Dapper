namespace Lesson_API.DTOs.Responses
{
    public class LessonDetailsResponse
    {
        public DateTime LessonDate { get; set; }
        public string PlanType { get; set; }
        public string Chapter { get; set; }
        public string SubTopic { get; set; }
        public string Synopsis { get; set; }
        public string Introduction { get; set; }
        public string MainTeaching { get; set; }
        public string Conclusion { get; set; }
        public List<string> Attachments { get; set; } // List to handle multiple documents if needed

    }

    public class LessonDetail
    {
        public string PlanType { get; set; }
        public string Chapter { get; set; }
        public string SubTopic { get; set; }
    }

    public class LectureNotes
    {
        public string Synopsis { get; set; }
        public string Introduction { get; set; }
        public string MainTeaching { get; set; }
        public string Conclusion { get; set; }
    }

    public class LessonAttachment
    {
        public string Attachment { get; set; }
    }
}
