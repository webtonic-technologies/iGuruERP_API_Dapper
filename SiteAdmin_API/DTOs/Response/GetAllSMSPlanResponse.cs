namespace SiteAdmin_API.DTOs.Responses
{
    public class GetAllSMSPlanResponse
    {
        public int RateID { get; set; }
        public int SMSVendorID { get; set; }
        public string VendorName { get; set; }
        public int CreditCount { get; set; }  // Add CreditCount
        public decimal CreditAmount { get; set; }  // Add CreditAmount
        public string Plan { get; set; }  // Format: "CreditCount credits for ₹ CreditAmount"
    }
}
