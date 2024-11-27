namespace FeesManagement_API.DTOs.Responses
{
    public class DailyPaymentSummaryResponse
    {
        public string FeeHead { get; set; }
        public decimal TotalCollectedAmount { get; set; }
        public Dictionary<string, decimal> PaymentMode { get; set; }  // Dynamic payment modes
    }

}
