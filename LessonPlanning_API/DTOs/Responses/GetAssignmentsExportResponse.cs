namespace Lesson_API.DTOs.Responses
{
    public class GetAssignmentsExportResponse
    {
        public int AssignmentID { get; set; }
        public string AssignmentName { get; set; }
        public string SubjectName { get; set; }
        public string AssignmentType { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public string StartDate { get; set; }
        public string SubmissionDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }  // CreatedBy (Employee Name)
        public string CreatedOn { get; set; }  // CreatedOn (Formatted as string)
        public string StudentsOrClassSection { get; set; } 

        public List<ClassSectionASResponseEX> ClassSections { get; set; }
        public List<StudentASResponseEX> Students { get; set; }
        public List<AssignmentDocsEX> AssignmentDocs { get; set; }
    }

    public class ClassSectionASResponseEX
    {
        public int AssignmentID { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }

    public class StudentASResponseEX
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
    }

    public class AssignmentDocsEX
    {
        public int DocumentsId { get; set; }
        public int AssignmentID { get; set; }
        public string DocFile { get; set; } = string.Empty;
    }
}
