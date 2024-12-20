namespace VisitorManagement_API.DTOs.Responses
{
    public class GetVisitorLogsExportResponse
    {
        public string VisitorCodeID { get; set; }
        public string VisitorName { get; set; }
        public string Sourcename { get; set; }
        public string Purposename { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }
        public string Address { get; set; }
        public string OrganizationName { get; set; }
        public string EmployeeFullName { get; set; }
        public int NoOfVisitor { get; set; }
        public string AccompaniedBy { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public string Remarks { get; set; }
        public string ApprovalTypeName { get; set; }
    }
}
