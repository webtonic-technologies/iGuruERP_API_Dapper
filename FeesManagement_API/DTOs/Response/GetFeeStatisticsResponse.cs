namespace FeesManagement_API.DTOs.Responses
{
    public class GetFeeStatisticsResponse
    {
        public decimal TotalAmountCollected { get; set; }
        public decimal TotalPendingAmount { get; set; }
        public decimal TotalFineCollected { get; set; }
    }
}
