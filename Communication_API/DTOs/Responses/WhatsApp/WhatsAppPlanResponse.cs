namespace Communication_API.DTOs.Responses.WhatsApp
{
    public class WhatsAppPlanResponse
    {
        public int RateID { get; set; }
        public int CreditCount { get; set; }
        public decimal CreditAmount { get; set; }

        public string Plan => $"{CreditCount} Credits for ₹ {CreditAmount}";
    }
}
