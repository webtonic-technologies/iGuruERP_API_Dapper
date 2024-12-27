namespace SiteAdmin_API.DTOs.Requests
{
    public class GetSMSOrderRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StatusID { get; set; }
    }
}
