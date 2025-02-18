namespace StudentManagement_API.DTOs.Responses
{
    public class DocumentDetailResponse
    {
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public bool IsSubmitted { get; set; }
    }

    public class GetDocumentManagerResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public List<DocumentDetailResponse> Documents { get; set; }
    }
}
