namespace SiteAdmin_API.DTOs.Requests
{
    public class GetWhatsAppOrderRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int StatusID { get; set; }
    }
}
