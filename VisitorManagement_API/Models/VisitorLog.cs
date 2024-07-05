namespace VisitorManagement_API.Models
{
    public class VisitorLog
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
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public string Remarks { get; set; }
        public int IDProofDocumentID { get; set; }
        public string Information { get; set; }
        public string Document { get; set; }
        public int ApprovalTypeID { get; set; }
        public bool Status { get; set; }  // Assuming there is a Status column
    }
}
