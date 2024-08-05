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
    }

    public class HomeworkClassSectionRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }
}
