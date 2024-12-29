using System;
using System.Collections.Generic;

namespace Lesson_API.DTOs.Responses
{
    public class GetAssignmentsReportsExportResponse
    {
        public int AssignmentID { get; set; }
        public string AssignmentName { get; set; }
        public string SubjectName { get; set; }
        public string AssignmentType { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public string StartDate { get; set; }  // StartDate formatted as string
        public string SubmissionDate { get; set; }  // SubmissionDate formatted as string
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }  // Employee Name
        public string CreatedOn { get; set; }  // Formatted CreatedOn (e.g., "dd-MM-yyyy hh:mm tt")

        public string ClassSections { get; set; }  // Now a string, not a list
        public string Students { get; set; }  // Now a string, not a list


        //public List<ClassSectionReportExport> ClassSections { get; set; }
        //public List<StudentReportExport> Students { get; set; }
        public List<AssignmentDocsReportExport> AssignmentDocs { get; set; }
    }

    public class ClassSectionReportExport
    {
        public int AssignmentID { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }

    public class StudentReportExport
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
    }

    public class AssignmentDocsReportExport
    {
        public int DocID { get; set; }
        public int AssignmentID { get; set; }
        public string DocFile { get; set; } = string.Empty;
    }
}
