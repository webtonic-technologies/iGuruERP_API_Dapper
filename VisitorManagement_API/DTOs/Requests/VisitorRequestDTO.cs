namespace VisitorManagement_API.DTOs.Requests
{
    public class VisitorRequestDTO
    {
        public int VisitorID { get; set; }
        public string VisitorCodeID { get; set; }
        public string VisitorName { get; set; }
        public string Photo { get; set; }
        public int SourceID { get; set; }
        public int PurposeID { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }
        public string Address { get; set; }
        public string OrganizationName { get; set; }
        public int EmployeeID { get; set; }
        public int NoOfVisitor { get; set; }
        public string AccompaniedBy { get; set; }
        public string CheckInTime { get; set; }  // Changed to string
        public string CheckOutTime { get; set; } // Changed to string
        public string Remarks { get; set; }
        public int IDProofDocumentID { get; set; }
        public string Information { get; set; }
        public List<VisitorDocument>? VisitorDocuments { get; set; }
        public int ApprovalTypeID { get; set; }
        public bool Status { get; set; }
        public int InstituteId { get; set; }
    }
    public class VisitorDocument
    {
        public int DocumentId { get; set; }
        public int VisitorId { get; set; }
        public string PdfDoc { get; set; } = string.Empty;
    }
}
