using Lesson_API.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;

namespace Lesson_API.DTOs.Requests
{
    public class HomeworkRequest
    {
        public int HomeworkID { get; set; }
        public string HomeworkName { get; set; }
        public int SubjectID { get; set; }
        public int HomeworkTypeID { get; set; }
        public string Notes { get; set; }
        public string Attachments { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<HomeworkClassSectionRequest> ClassSections { get; set; }
        public List<HomeworkDocs> HomeworkDocs {  get; set; }
    }

    public class HomeworkClassSectionRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }
    public class HomeworkDocs
    {
        public int DocumentsId { get; set; }
        public int HomeworkID { get; set; }
        public string DocFile { get; set; } = string.Empty;
    }
}
