namespace FeesManagement_API.DTOs.Requests
{
    public class SubmitChequeClearanceRequest
    {
        public int TransactionID { get; set; }
        public DateTime ChequeClearanceDate { get; set; } // Use DateTime for proper date handling
        public string Remarks { get; set; }
    }
}
