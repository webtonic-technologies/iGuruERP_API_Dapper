namespace EventGallery_API.DTOs.Requests
{
    public class EventApprovalRequest
    {
        public int InstituteID { get; set; }
        public string Search { get; set; } // For searching events by name
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    } 
}
