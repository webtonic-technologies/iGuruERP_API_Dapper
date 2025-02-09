namespace Communication_API.DTOs.Responses.NoticeBoard
{
    public class GetAllCircularExportResponse
    {
        public string AcademicYear { get; set; }
        public string CircularNo { get; set; }
        public string Title { get; set; }
        public string CircularDate { get; set; }   // Formatted as dd-MM-yyyy
        public string PublishedDate { get; set; }  // Formatted as dd-MM-yyyy
        public string Recipients { get; set; }
        public string CreatedOn { get; set; }      // Formatted as dd-MM-yyyy
        public string CreatedBy { get; set; }
    }
}
