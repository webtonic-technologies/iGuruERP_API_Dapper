namespace FeesManagement_API.DTOs.Requests
{
    public class SubmitChequeBounceRequest
    {
        public int ChequeBounceID { get; set; }
        public int TransactionID { get; set; }
        public decimal ChequeBounceCharges { get; set; }
        public string Reason { get; set; }
    }
}
