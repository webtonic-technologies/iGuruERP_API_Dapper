namespace SiteAdmin_API.DTOs.Requests
{
    public class AddUpdateSMSPlanRequest
    {
        public int RateID { get; set; }
        public int SMSVendorID { get; set; }
        public int CreditCount { get; set; }
        public decimal CreditAmount { get; set; } 
    }
}
