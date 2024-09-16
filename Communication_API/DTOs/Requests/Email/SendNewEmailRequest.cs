namespace Communication_API.DTOs.Requests.Email
{
    public class SendNewEmailRequest
    {
        public int EmailSendID { get; set; }  // For updating an email record
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public int UserTypeID { get; set; }
        public int GroupID { get; set; }
        public bool Status { get; set; }
        public bool ScheduleNow { get; set; }
        public DateTime ScheduleDate { get; set; }
        public DateTime ScheduleTime { get; set; }

        // Add these properties for student and employee IDs
        public List<int>? StudentIDs { get; set; }  // For mapping students to the email
        public List<int>? EmployeeIDs { get; set; }  // For mapping employees to the email
    }
}
