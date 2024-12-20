namespace VisitorManagement_API.DTOs.Responses
{
    public class GetAppointmentsExportResponse
    {
        public string Appointee { get; set; }
        public string OrganizationName { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }
        public string PurposeName { get; set; }
        public string EmployeeFullName { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public string Description { get; set; }
        public int NoOfVisitors { get; set; }
        public string ApprovalStatusName { get; set; }
    }
}
