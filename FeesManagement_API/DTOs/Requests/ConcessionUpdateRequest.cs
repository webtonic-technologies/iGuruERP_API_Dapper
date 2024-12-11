namespace FeesManagement_API.DTOs.Requests
{
    public class ConcessionUpdateRequest
    {
        public int ConcessionGroupID { get; set; }
        public string? InActiveReason { get; set; }
    }

}
