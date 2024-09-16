namespace Communication_API.DTOs.Responses.NoticeBoard
{
    public class CircularResponse
    {
        public int CircularID { get; set; }
        public string AcademicYear { get; set; } // Adjust type if necessary
        public string CircularNo { get; set; }
        public string Title { get; set; }
        public DateTime CircularDate { get; set; }
        public string Recipients { get; set; }
    }

}
