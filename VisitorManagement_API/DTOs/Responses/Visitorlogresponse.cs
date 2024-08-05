namespace VisitorManagement_API.DTOs.Responses
{
    public class Visitorlogresponse
    {
        public int VisitorID { get; set; }
        public string VisitorCodeID { get; set; }
        public string VisitorName { get; set; }
        public string Photo { get; set; }
        public int SourceID { get; set; }
        public string Sourcename { get; set; }
        public int PurposeID { get; set; }
        public string Purposename { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }
        public string Address { get; set; }
        public string OrganizationName { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeFullName { get; set; } = string.Empty;
        public int NoOfVisitor { get; set; }
        public string AccompaniedBy { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public string Remarks { get; set; }
        public int IDProofDocumentID { get; set; }
        public string IDProofDocumentName { get; set; } = string.Empty;
        public string Information { get; set; }
        public List<VisitorDocumentResponse>? VisitorDocuments { get; set; }
        public int ApprovalTypeID { get; set; }
        public string ApprovalTypeName { get; set; } = string.Empty;
        public bool Status { get; set; } 
        public int InstituteId {  get; set; }
    }
    public class VisitorDocumentResponse
    {
        public int DocumentId { get; set; }
        public int VisitorId { get; set; }
        public string PdfDoc { get; set; } = string.Empty;
    }
}
