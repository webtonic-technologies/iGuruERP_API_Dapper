using Lesson_API.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public List<AssignmentDocs> AssignmentDocs {  get; set; }
    }

    public class AssignmentClassSectionRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }
    public class AssignmentDocs
    {
        public int DocumentsId { get; set; }
        public int AssignmentID { get; set; }
        public string DocFile { get; set; } = string.Empty;
    }
}
