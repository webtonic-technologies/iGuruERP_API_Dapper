namespace Communication_API.Models.NoticeBoard
{
    public class Circular
    {
        public int CircularID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Attachment { get; set; }
        public string CircularNo { get; set; }
        public DateTime CircularDate { get; set; }
        public DateTime PublishedDate { get; set; }
        public bool IsStudent { get; set; }
        public bool IsEmployee { get; set; }
        public bool ScheduleNow { get; set; }
        public DateTime ScheduleDate { get; set; }
        public DateTime ScheduleTime { get; set; }
    }
}
