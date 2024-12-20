﻿using Lesson_API.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;

namespace Lesson_API.DTOs.Requests
{
    public class CurriculumRequest
    {
        public int CurriculumID { get; set; }
        public string AcademicYearID { get; set; }
        public int ClassID { get; set; }
        public int SubjectID { get; set; }
        public int InstituteID { get; set; }
        public List<CurriculumChapterRequest> CurriculumChapters { get; set; }
    }

    public class CurriculumChapterRequest
    {
        public int CurriculumChapterID { get; set; }
        public string ChapterName { get; set; }
        public int TotalSessions { get; set; } 
        public List<CurriculumSubTopicRequest> CurriculumSubTopics { get; set; }
        public List<chapterDocs> chapterDocs { get; set; }
    }

    public class CurriculumSubTopicRequest
    {
        public int CurriculumSubTopicID { get; set; }
        public string SubTopicName { get; set; }
        public int TotalSession { get; set; } 
        public List<CurriculumResourceDetailsRequest> CurriculumResourceDetails { get; set; }
        public List<SubtopicDocs> SubtopicDocs { get; set; }
    }

    public class CurriculumResourceDetailsRequest
    {
        public int CurriculumResourceID { get; set; }
        public string LearningObjectives { get; set; }
        public string SuggestedActivity { get; set; }
        public string TeachingResouces { get; set; }
        public string TeachingMethod { get; set; }
        public string Criteria { get; set; }
    }
    public class chapterDocs
    {
        public int DocumentsId { get; set; }
        public int CurriculumChapterID { get; set; }
        public string DocFile { get; set; } = string.Empty;
    }
    public class SubtopicDocs
    {
        public int DocumentsId { get; set; }
        public int CurriculumSubTopicID { get; set; }
        public string DocFile { get; set; } = string.Empty;
    }
}
