namespace Communication_API.DTOs.Responses.NoticeBoard
{
    public class GetAllNoticeExportResponse
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }  // Formatted as dd-MM-yyyy
        public string EndDate { get; set; }    // Formatted as dd-MM-yyyy
        public string CreatedOn { get; set; }  // Formatted as dd-MM-yyyy
        public string CreatedBy { get; set; }
        public string Recipients { get; set; }
    }
}
