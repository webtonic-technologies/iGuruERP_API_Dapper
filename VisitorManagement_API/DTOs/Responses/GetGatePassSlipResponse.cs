namespace VisitorManagement_API.DTOs.Responses
{
    public class GatePassInstituteInfo
    {
        public string InstituteName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }

    public class GatePassVisitorSlip
    {
        public string PassNo { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string MobileNumber { get; set; }
        public string VisitorFor { get; set; }
        public string PlanOfVisit { get; set; }
        public string Purpose { get; set; }
        public string Remarks { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
    }

    public class GetGatePassSlipResponse
    {
        public GatePassInstituteInfo InstituteInfo { get; set; }
        public GatePassVisitorSlip VisitorSlip { get; set; }
    }
}
