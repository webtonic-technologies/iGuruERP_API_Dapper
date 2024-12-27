namespace SiteAdmin_API.DTOs.Responses
{
    public class GetAllWhatsAppPlanResponse
    {
        public int RateID { get; set; }
        public int WhatsAppVendorID { get; set; }
        public string VendorName { get; set; }
        public int CreditCount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Plan { get; set; }
    }
}
