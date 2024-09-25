namespace EventGallery_API.DTOs.Requests
{
    public class DateRangeRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int InstituteID { get; set; }
    }
}
