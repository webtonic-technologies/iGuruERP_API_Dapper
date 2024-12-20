namespace VisitorManagement_API.Models
{
    public class EmployeeGatePass
    {
        public int GatePassID { get; set; }
        public int EmployeeID { get; set; }
        public string PassNo { get; set; }
        public int VisitorFor { get; set; }
        public string CheckOutTime { get; set; }  // Changed to string
        public string CheckInTime { get; set; }   // Changed to string
        public string Purpose { get; set; }
        public string PlanOfVisit { get; set; }
        public string Remarks { get; set; }
        public int StatusID { get; set; }
        public int InstituteId {  get; set; } 
    }
}
