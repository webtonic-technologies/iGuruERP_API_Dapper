using Lesson_API.DTOs.Requests;

namespace Lesson_API.DTOs.Responses
{
    public class GetAllAssignmentsResponse
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

        public List<ClassSectionASResponse> ClassSections { get; set; }
        public List<StudentASResponse> Students { get; set; }

        public List<AssignmentDocs> AssignmentDocs { get; set; }
    }
}
