namespace Communication_API.DTOs.Responses.SMS
{
    public class GetSMSTopUpHistoryResponse
    {
        public int SMSOrderID { get; set; }
        public int SMSCredits { get; set; }
        public decimal Amount { get; set; }
        public string TransactionDate { get; set; }
    }
}
