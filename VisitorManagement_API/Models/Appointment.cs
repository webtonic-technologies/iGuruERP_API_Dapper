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
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public string Description { get; set; }
        public int NoOfVisitors { get; set; }
        public bool Status { get; set; }  // Assuming there is a Status column
    }
}
