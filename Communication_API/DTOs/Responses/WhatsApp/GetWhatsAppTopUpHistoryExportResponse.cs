namespace Communication_API.DTOs.Responses.WhatsApp
{
    public class GetWhatsAppTopUpHistoryExportResponse
    {
        public int WhatsAppCredits { get; set; }
        public decimal Amount { get; set; }
        public string TransactionDate { get; set; }
    }
}
