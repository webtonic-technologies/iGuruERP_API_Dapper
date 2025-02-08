namespace Communication_API.DTOs.Responses.NoticeBoard
{
    public class NoticeResponse
    {
        public int NoticeID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Recipients { get; set; }
        public string CreatedOn { get; set; } // Ensure it's a string formatted as dd-MM-yyyy
        public string CreatedBy { get; set; }    // Keep as int since it's a user ID

    }

}
