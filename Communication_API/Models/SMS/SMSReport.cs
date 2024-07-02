namespace Communication_API.Models.SMS
{
    public class SMSReport
    {
        //public int SMSID { get; set; }
        //public string Message { get; set; }
        //public string PhoneNumber { get; set; }
        //public DateTime SentDate { get; set; }
        //public bool Status { get; set; }


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
