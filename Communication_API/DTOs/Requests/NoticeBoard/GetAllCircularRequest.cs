namespace Communication_API.DTOs.Requests.NoticeBoard
{
    public class GetAllCircularRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? StartDate { get; set; } // Add this property
        public string? EndDate { get; set; }   // Add this property
        public int? InstituteID { get; set; }    // Add this property
    }

}
