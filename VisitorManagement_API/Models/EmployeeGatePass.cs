namespace VisitorManagement_API.Models
{
    public class EmployeeGatePass
    {
        public int GatePassID { get; set; }
        public int EmployeeID { get; set; }
        public string PassNo { get; set; }
        public int VisitorFor { get; set; }
        public DateTime CheckOutTime { get; set; }
        public DateTime CheckInTime { get; set; }
        public string Purpose { get; set; }
        public string PlanOfVisit { get; set; }
        public string Remarks { get; set; }
        public int StatusID { get; set; }
        public int InstituteId {  get; set; }
        public bool IsDeleted {  get; set; }
    }
}
