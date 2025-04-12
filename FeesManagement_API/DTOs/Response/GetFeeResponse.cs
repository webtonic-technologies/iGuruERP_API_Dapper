namespace FeesManagement_API.DTOs.Responses
{
    public class FeeCollectionDetail
    {
        public string FeeHead { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal PaidAmount { get; set; }      // Amount paid
        public decimal WaiverAmount { get; set; }      // Waiver amount
        public decimal DiscountAmount { get; set; }    // Discount amount
        public decimal BalanceAmount { get; set; }     // Calculated balance

        // New parameters to show in response
        public int FeeGroupID { get; set; }
        public int FeeHeadID { get; set; }
        public int FeeTenurityID { get; set; }
        public int? TenuritySTMID { get; set; }
        public int? FeeCollectionSTMID { get; set; }
    }

    public class FeeTypeDetail
    {
        // List of fee details for a given fee type (e.g., Term 1 can include multiple fee items)
        public List<FeeCollectionDetail> FeeDetails { get; set; } = new List<FeeCollectionDetail>();

        // Aggregated total fee amount for the fee type (e.g., term total)
        public decimal SectionTotalFee { get; set; }
    }

    public class GetFeeResponse
    {
        // FeeType dictionary mapping fee type key (e.g., "Term 1") to fee details & term total
        public Dictionary<string, FeeTypeDetail> FeeType { get; set; }
        public decimal TotalFee { get; set; }       // Total Fee Amount (across all fee types)
        public decimal TotalFeePaid { get; set; }   // Total Paid Amount
        public decimal TotalWaiver { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalBalance { get; set; }   // Total Balance Amount

    }
}
