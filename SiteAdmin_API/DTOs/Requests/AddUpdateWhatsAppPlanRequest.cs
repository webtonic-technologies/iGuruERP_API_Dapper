namespace SiteAdmin_API.DTOs.Requests
{
    public class AddUpdateWhatsAppPlanRequest
    {
        public int RateID { get; set; }
        public int WhatsAppVendorID { get; set; }
        public int CreditCount { get; set; }
        public decimal CreditAmount { get; set; }
    }
}
