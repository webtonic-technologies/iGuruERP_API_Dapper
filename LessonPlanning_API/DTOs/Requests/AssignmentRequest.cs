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
        public List<int> ?StudentIDs { get; set; }
        public int SubjectID { get; set; }
        public int AssignmentTypeID { get; set; }
        public string StartDate { get; set; }
        public string SubmissionDate { get; set; }
        public bool IsSubmission { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; } 
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; } // Newly added CreatedBy
        public DateTime CreatedOn { get; set; } // Newly added CreatedOn

        public List<AssignmentClassSectionRequest> ?ClassSections { get; set; }
        public List<AssignmentDocs> AssignmentDocs {  get; set; }
    }

    public class AssignmentClassSectionRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }
    public class AssignmentDocs
    {
        public int DocID { get; set; }
        public int AssignmentID { get; set; }
        public string DocFile { get; set; } = string.Empty;
    }
}
