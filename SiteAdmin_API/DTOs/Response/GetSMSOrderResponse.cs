namespace SiteAdmin_API.DTOs.Responses
{
    public class GetSMSOrderResponse
    {
        public int SMSVendorID { get; set; }
        public string VendorName { get; set; }
        public int InstituteID { get; set; }
        public string TransactionID { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public int OrderStatus { get; set; }
        public string Status { get; set; } // Pending/Executed
    }
}
