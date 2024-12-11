namespace FeesManagement_API.DTOs.Requests
{
    public class SubmitChequeClearanceRequest
    {
        public int TransactionID { get; set; }
        public string ChequeClearanceDate { get; set; }
        public string Remarks { get; set; }
    }
}
