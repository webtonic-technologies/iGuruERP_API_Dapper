namespace FeesManagement_API.DTOs.Requests
{
    public class UpdateStatusRequest
    {
        public int StudentConcessionID { get; set; }
        public string InActiveReason { get; set; }
    }

}
