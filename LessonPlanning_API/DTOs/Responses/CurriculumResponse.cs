namespace Lesson_API.DTOs.Responses
{
    public class CurriculumResponse
    {
        public int CurriculumID { get; set; }
        public int ClassID { get; set; }
        public int SubjectID { get; set; }
        public int InstituteID { get; set; }
        public List<CurriculumChapterResponse> CurriculumChapters { get; set; }
    }

    public class CurriculumChapterResponse
    {
        public int CurriculumChapterID { get; set; }
        public string ChapterName { get; set; }
        public int TotalSessions { get; set; }
        public string Attachment { get; set; }
        public List<CurriculumSubTopicResponse> CurriculumSubTopics { get; set; }
    }

    public class CurriculumSubTopicResponse
    {
        public int CurriculumSubTopicID { get; set; }
        public string SubTopicName { get; set; }
        public int TotalSession { get; set; }
        public string Attachment { get; set; }
        public List<CurriculumResourceDetailsResponse> CurriculumResourceDetails { get; set; }
    }

    public class CurriculumResourceDetailsResponse
    {
        public int CurriculumResourceID { get; set; }
        public string LearningObjectives { get; set; }
        public string SuggestedActivity { get; set; }
        public string TeachingResouces { get; set; }
        public string TeachingMethod { get; set; }
        public string Criteria { get; set; }
    }
}
