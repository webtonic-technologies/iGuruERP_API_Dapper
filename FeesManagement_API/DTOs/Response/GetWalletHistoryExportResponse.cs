namespace FeesManagement_API.DTOs.Responses
{
    public class GetWalletHistoryExportResponse
    {
        public List<GetWalletHistoryExportItem> WalletHistory { get; set; }
        public GetWalletHistoryExportTotal Total { get; set; }
    }

    public class GetWalletHistoryExportItem
    {
        public string PaymentDate { get; set; }
        public string PaymentMode { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Comment { get; set; }
    }

    public class GetWalletHistoryExportTotal
    {
        public decimal TotalDebits { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalAvailableBalance { get; set; }
    }
}
