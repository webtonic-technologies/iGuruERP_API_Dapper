namespace Communication_API.DTOs.Requests.NoticeBoard
{
    public class GetAllCircularRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public DateTime? StartDate { get; set; } // Add this property
        public DateTime? EndDate { get; set; }   // Add this property
        public int? InstituteID { get; set; }    // Add this property
    }

}
