namespace FeesManagement_API.DTOs.Responses
{
    public class HeadWiseCollectedAmountResponse
    {
        public string FeeHead { get; set; }
        public decimal TotalFeeAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Balance { get; set; }
        public decimal PercentageCollected { get; set; }
    }
}
