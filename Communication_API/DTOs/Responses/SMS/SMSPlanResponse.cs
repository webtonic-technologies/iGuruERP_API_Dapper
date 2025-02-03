namespace Communication_API.DTOs.Responses.SMS
{
    public class SMSPlanResponse
    {
        public int RateID { get; set; }            // The unique identifier for the SMS plan
        public int CreditCount { get; set; }       // The number of credits in this plan
        public decimal CreditAmount { get; set; }  // The total cost in INR (₹)

        // The Plan property returns a formatted string representing the plan details
        public string Plan => $"{CreditCount} Credits for ₹ {CreditAmount}";
    }
}
