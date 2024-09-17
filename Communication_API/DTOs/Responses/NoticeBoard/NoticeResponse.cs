namespace Communication_API.DTOs.Responses.NoticeBoard
{
    public class NoticeResponse
    {
        public int NoticeID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Recipients { get; set; }
    }

}
