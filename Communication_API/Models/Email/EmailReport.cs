namespace Communication_API.Models.Email
{
    public class EmailReport
    {
        public int EmailSendID { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public int UserTypeID { get; set; }
        public int GroupID { get; set; }
        public bool Status { get; set; }
        public DateTime ScheduleDate { get; set; }
        public DateTime ScheduleTime { get; set; }
    }
}
