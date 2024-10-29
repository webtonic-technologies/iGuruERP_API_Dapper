namespace FeesManagement_API.DTOs.Responses
{
    public class FeeCollectionDetail
    {
        public string FeeHead { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal PaidAmount { get; set; }      // Amount paid
        public decimal WaiverAmount { get; set; }    // Waiver amount
        public decimal DiscountAmount { get; set; }   // Discount amount
        public decimal BalanceAmount { get; set; }   // Discount amount
    }


    public class GetFeeResponse
    {
        public Dictionary<string, FeeCollectionDetail> FeeType { get; set; }
        public decimal TotalFee { get; set; }            // Total Fee Amount
        public decimal TotalFeePaid { get; set; }        // Total Paid Amount
        public decimal TotalBalance { get; set; }        // Total Balance Amount
    }

}
