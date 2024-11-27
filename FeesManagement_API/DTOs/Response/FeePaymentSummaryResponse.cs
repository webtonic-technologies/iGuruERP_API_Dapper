namespace FeesManagement_API.DTOs.Responses
{
    public class FeePaymentSummaryResponse
    {
        public string ClassSection { get; set; }
        public string RollNumber { get; set; }
        public string Gender { get; set; }
        public string FatherName { get; set; }
        public string MobileNo { get; set; }
        public string StudentName { get; set; }
        public decimal TotalFee { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public decimal Discount { get; set; }
        public decimal Waiver { get; set; }
    }
}
