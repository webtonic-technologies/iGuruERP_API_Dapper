namespace Communication_API.DTOs.Responses.NoticeBoard
{
    public class NoticeResponse
    {
        public int NoticeID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Attachments { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsStudent { get; set; }
        public bool IsEmployee { get; set; }
        public bool ScheduleNow { get; set; }
        public DateTime ScheduleDate { get; set; }
        public DateTime ScheduleTime { get; set; }
    }
}
