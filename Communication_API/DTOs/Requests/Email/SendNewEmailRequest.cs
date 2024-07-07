namespace Communication_API.DTOs.Requests.Email
{
    public class SendNewEmailRequest
    {
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public int UserTypeID { get; set; }
        public int GroupID { get; set; }
        public bool ScheduleNow { get; set; }
        public DateTime ScheduleDate { get; set; }
        public DateTime ScheduleTime { get; set; }
    }
}
