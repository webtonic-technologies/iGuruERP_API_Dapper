namespace Communication_API.DTOs.Responses.WhatsApp
{
    public class GetWhatsAppTopUpHistoryResponse
    {
        public int WhatsAppOrderID { get; set; }
        public int WhatsAppCredits { get; set; }
        public decimal Amount { get; set; }
        public string TransactionDate { get; set; } // Change to string for formatted date
    }
}
