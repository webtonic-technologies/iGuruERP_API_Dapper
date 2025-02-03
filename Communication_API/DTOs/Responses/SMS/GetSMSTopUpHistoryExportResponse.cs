namespace Communication_API.DTOs.Responses
{
    public class GetSMSTopUpHistoryExportResponse
    {
        public int SMSCredits { get; set; }
        public decimal Amount { get; set; }
        public string TransactionDate { get; set; }  // Formatted Date String
    }
}
