namespace FeesManagement_API.DTOs.Responses
{
    public class GetWalletHistoryResponse
    {
        public List<GetWalletHistoryItem> WalletHistory { get; set; }
        public GetWalletHistoryTotal Total { get; set; }
    }

    public class GetWalletHistoryItem
    {
        public string PaymentDate { get; set; }
        public string PaymentMode { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Comment { get; set; }
    }
     
    public class GetWalletHistoryTotal
    {
        public decimal TotalDebits { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalAvailableBalance { get; set; }
    }

}
