using System;
using System.Collections.Generic;

namespace Lesson_API.DTOs.Requests
{
    public class AssignmentRequest
    {
        public int AssignmentID { get; set; }
        public string AssignmentName { get; set; }
        public bool IsClasswise { get; set; }
        public bool IsStudentwise { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int StudentID { get; set; }
        public int SubjectID { get; set; }
        public int AssignmentTypeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public string Attachments { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<AssignmentClassSectionRequest> ClassSections { get; set; }
    }

    public class AssignmentClassSectionRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }
}
