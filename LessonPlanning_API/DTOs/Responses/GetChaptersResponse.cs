namespace Lesson_API.DTOs.Responses
{
    public class GetChaptersResponse
    {
        public int CurriculumChapterID { get; set; }
        public string ChapterName { get; set; }
        public string DueDate { get; set; }
        public List<SubTopicResponse> SubTopics { get; set; }
    }

    public class SubTopicResponse
    {
        public int CurriculumChapterID { get; set; }
        public int CurriculumSubTopicID { get; set; }
        public string SubTopicName { get; set; }
        public string DueDate { get; set; }
    }
}
