namespace FeesManagement_API.DTOs.Requests
{
    public class AddWalletAmountRequest
    {
        public int StudentID { get; set; }
        public decimal Amount { get; set; }
        public int PaymentModeID { get; set; }
        public string Comment { get; set; }
        public int InstituteID { get; set; }
    }
}
