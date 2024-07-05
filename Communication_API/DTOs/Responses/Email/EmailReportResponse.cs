namespace Communication_API.DTOs.Responses.Email
{
    public class EmailReportResponse
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
