using Lesson_API.DTOs.Requests;
using System.Collections.Generic;

namespace Lesson_API.Models
{
    public class Homework
    {
        public int HomeworkID { get; set; }
        public string HomeworkName { get; set; }
        public int SubjectID { get; set; }
        public int HomeworkTypeID { get; set; }
        public string Notes { get; set; }
        public string Attachments { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<HomeworkClassSection> ClassSections { get; set; }
        public List<HomeworkDocs> HomeworkDocs { get; set; }
    }
}
