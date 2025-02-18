namespace StudentManagement_API.DTOs.Requests
{
    // Represents each document setting for a student.
    public class StudentDocumentRequest
    {
        public int DocumentID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public bool IsSubmitted { get; set; }
    }

    // Represents the document manager settings for a single student.
    public class SetDocumentManagerRequest
    {
        public int StudentId { get; set; }
        public List<StudentDocumentRequest> StudentDocuments { get; set; }
    }
}
