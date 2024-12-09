using Lesson_API.DTOs.Requests;
using System.Collections.Generic;

namespace Lesson_API.Models
{
    public class Curriculum
    {
        public int CurriculumID { get; set; }
        public string AcademicYearID { get; set; }
        public int ClassID { get; set; }
        public int SubjectID { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<CurriculumChapter1> CurriculumChapters { get; set; }
    }

    public class CurriculumChapter1
    {
        public int CurriculumChapterID { get; set; }
        public string ChapterName { get; set; }
        public int TotalSessions { get; set; }
        public string Attachment { get; set; }
        public int CurriculumID { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<CurriculumSubTopic> CurriculumSubTopics { get; set; }
        public List<chapterDocs> chapterDocs { get; set; }
    }

    public class CurriculumSubTopic
    {
        public int CurriculumSubTopicID { get; set; }
        public string SubTopicName { get; set; }
        public int TotalSession { get; set; }
        public string Attachment { get; set; }
        public int CurriculumChapterID { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<CurriculumResourceDetails> CurriculumResourceDetails { get; set; }
        public List<SubtopicDocs> SubtopicDocs { get; set; }
    }

    public class CurriculumResourceDetails
    {
        public int CurriculumResourceID { get; set; }
        public string LearningObjectives { get; set; }
        public string SuggestedActivity { get; set; }
        public string TeachingResouces { get; set; }
        public string TeachingMethod { get; set; }
        public string Criteria { get; set; }
        public int CurriculumSubTopicID { get; set; }
        public bool IsActive { get; set; }
    }
}
