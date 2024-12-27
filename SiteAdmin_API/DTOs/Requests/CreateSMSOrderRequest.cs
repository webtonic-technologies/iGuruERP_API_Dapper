namespace SiteAdmin_API.DTOs.Requests
{
    public class CreateSMSOrderRequest
    {
        public int SMSVendorID { get; set; }
        public int InstituteID { get; set; }
        public string TransactionID { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public int OrderStatus { get; set; } // Assuming 1 for 'Pending', 2 for 'Completed', etc.
    }
}
