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
        public string Attachments { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime SubmissionDate { get; set; }
        public bool IsActive { get; set; }
        public List<ClassSectionResponse> ClassSections { get; set; }
        public List<AssignmentDocs> AssignmentDocs { get; set; }
    }
}
