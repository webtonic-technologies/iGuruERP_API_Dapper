namespace Communication_API.DTOs.Requests.SMS
{
    public class SendNewSMSRequest
    {
        //public string Message { get; set; }
        //public string PhoneNumber { get; set; }
        //public bool ScheduleNow { get; set; }
        //public DateTime ScheduleDate { get; set; }
        //public TimeSpan ScheduleTime { get; set; }

        public int SMSID { get; set; }
        public int PredefinedTemplateID { get; set; }
        public string? SMSMessage { get; set; }
        public int UserTypeID { get; set; }
        public int GroupID { get; set; }
        public bool Status { get; set; }
        public bool ScheduleNow { get; set; }
        public DateTime ScheduleDate { get; set; }
        public DateTime ScheduleTime { get; set; }
    }
}
