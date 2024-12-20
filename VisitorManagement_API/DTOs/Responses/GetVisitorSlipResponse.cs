namespace VisitorManagement_API.DTOs.Responses
{
    public class InstituteInfo
    {
        public string InstituteName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }

    public class VisitorSlip
    {
        public string VisitorCode { get; set; }
        public string VisitorName { get; set; }
        public string OrganizationName { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string Purpose { get; set; } 
        public string MeetingWith { get; set; }
        public string Remarks { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
    }

    public class GetVisitorSlipResponse
    {
        public InstituteInfo InstituteInfo { get; set; }
        public VisitorSlip VisitorSlip { get; set; }
    }
}
