namespace FeesManagement_API.DTOs.Requests
{
    public class SubmitFeeDiscountRequest
    {
        public List<FeeDiscount> FeeDiscounts { get; set; }
    }

    public class FeeDiscount
    {
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int InstituteID { get; set; }
        public int FeeGroupID { get; set; }
        public int FeeHeadID { get; set; }
        public int FeeTenurityID { get; set; }
        public int TenuritySTMID { get; set; }
        public int FeeCollectionSTMID { get; set; }
        public decimal Amount { get; set; }
        public int DiscountGivenBy { get; set; }
        public string Reason { get; set; }

    }
}
