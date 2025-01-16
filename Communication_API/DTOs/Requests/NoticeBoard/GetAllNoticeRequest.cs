namespace Communication_API.DTOs.Requests.NoticeBoard
{
    public class GetAllNoticeRequest
    {
        public int InstituteID { get; set; } // Add this line
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string StartDate { get; set; } // Nullable to make the filters optional
        public string EndDate { get; set; }   // Nullable to make the filters optional
        public string Search { get; set; }
    }
}
