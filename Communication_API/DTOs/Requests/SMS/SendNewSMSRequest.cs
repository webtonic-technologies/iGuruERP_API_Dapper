namespace Communication_API.DTOs.Requests.SMS
{
    public class SendNewSMSRequest
    {
        public int SMSID { get; set; }
        public int PredefinedTemplateID { get; set; }
        public string SMSMessage { get; set; }
        public int UserTypeID { get; set; }
        public int GroupID { get; set; }
        public bool Status { get; set; }
        public bool ScheduleNow { get; set; }
        public DateTime ScheduleDate { get; set; }
        public DateTime ScheduleTime { get; set; }

        // Add these properties
        public List<int>? StudentIDs { get; set; }
        public List<int>? EmployeeIDs { get; set; } 
    }

}
