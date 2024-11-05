namespace FeesManagement_API.DTOs.Responses
{
    public class PaidFeeResponse
    {
        public string AdmissionNumber { get; set; }
        public string StudentName { get; set; }
        public string ClassSection { get; set; }
        public string RollNumber { get; set; }
        public string FatherName { get; set; }
        public string MobileNo { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalWaiver { get; set; }
        public decimal TotalFee { get; set; }
        public decimal Balance { get; set; }
    }
}
