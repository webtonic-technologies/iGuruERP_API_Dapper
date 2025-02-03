namespace SiteAdmin_API.DTOs.Requests
{
    public class CreateWhatsAppOrderRequest
    {
        public int WhatsAppVendorID { get; set; }
        public int InstituteID { get; set; }
        public string TransactionID { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public int OrderStatus { get; set; }
        public int RateID { get; set; }
    }
}
