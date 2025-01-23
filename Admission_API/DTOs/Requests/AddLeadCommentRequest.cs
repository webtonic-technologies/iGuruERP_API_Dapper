namespace Admission_API.DTOs.Requests
{
    public class AddLeadCommentRequest
    {
        public int LeadID { get; set; }
        public string FollowupDate { get; set; } // Format: DD-MM-YYYY
        public string Comments { get; set; }
        public int LeadStageID { get; set; }
    }
}
