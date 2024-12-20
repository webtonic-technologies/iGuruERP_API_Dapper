namespace VisitorManagement_API.Models
{
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public string Appointee { get; set; }
        public string OrganizationName { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }
        public int PurposeID { get; set; }
        public int EmployeeID { get; set; }
        public string CheckInTime { get; set; }  // Changed from DateTime to string
        public string CheckOutTime { get; set; }  // Changed from DateTime to string

        public string Description { get; set; }
        public int NoOfVisitors { get; set; }
        public bool Status { get; set; }  // Assuming there is a Status column
        public int InstituteId { get; set; }
        public int ApprovalStatus { get; set; }
    }
}
