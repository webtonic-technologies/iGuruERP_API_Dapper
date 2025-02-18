using System.Collections.Generic;

namespace StudentManagement_API.DTOs.Responses
{
    public class DocumentDetailExportResponse
    {
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public bool IsSubmitted { get; set; }
    }

    public class GetDocumentManagerExportResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public List<DocumentDetailExportResponse> Documents { get; set; }
    }
}
