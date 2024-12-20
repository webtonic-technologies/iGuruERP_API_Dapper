namespace VisitorManagement_API.DTOs.Responses
{
    public class GetEmployeeGatePassExportResponse
    {
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string PassNo { get; set; }
        public string VisitorForName { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public string Purpose { get; set; }
        public string PlanOfVisit { get; set; }
        public string Remarks { get; set; }
        public string StatusName { get; set; }
    }
}
