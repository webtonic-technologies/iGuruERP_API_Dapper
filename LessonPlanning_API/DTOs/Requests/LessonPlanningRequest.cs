﻿using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;

namespace Lesson_API.DTOs.Requests
{
    public class LessonPlanningRequest
    {
        public int LessonPlanningID { get; set; }
        public string AcademicYear { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int SubjectID { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<LessonPlanningInformationRequest> LessonPlanningInformation { get; set; }
    }

    public class LessonPlanningInformationRequest
    {
        public int LessonPlanningInfoID { get; set; }
        public int LessonPlanningID { get; set; }
        public string LessonDate { get; set; }
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
    public class documents
    {
        public int DocumentsId { get; set; }
        public int LessonPlanningInfoID { get; set; }
        public string DocFile { get; set; } = string.Empty;
    }
}
