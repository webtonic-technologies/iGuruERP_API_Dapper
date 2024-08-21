namespace VisitorManagement_API.DTOs.Responses
{
    public class EmployeeGatepassResponse
    {
        public int GatePassID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int Departmentid { get; set; }
        public string Departmentname { get; set; } = string.Empty;
        public string Designationname { get; set; } = string.Empty;
        public int Designationid { get; set; }
        public string PassNo { get; set; }
        public int VisitorFor { get; set; }
        public string VisitorForName { get; set; } = string.Empty;
        public DateTime CheckOutTime { get; set; }
        public DateTime CheckInTime { get; set; }
        public string Purpose { get; set; }
        public string PlanOfVisit { get; set; }
        public string Remarks { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; }  
        public int InstituteId {  get; set; }
    }
}
