namespace Communication_API.DTOs.Requests.DiscussionBoard
{
    public class GetAllDiscussionRequest
    {
        public int InstituteID { get; set; }
        // Expected format: "dd-MM-yyyy"
        public string StartDate { get; set; }
        // Expected format: "dd-MM-yyyy"
        public string EndDate { get; set; }
    }
}
