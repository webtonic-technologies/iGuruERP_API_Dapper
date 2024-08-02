namespace Communication_API.DTOs.Responses.DiscussionBoard
{
    public class DiscussionResponse
    {
        public int DiscussionBoardID { get; set; }
        public string DiscussionHeading { get; set; }
        public string Description { get; set; }
        public string Attachments { get; set; }
        public bool IsStudent { get; set; }
        public bool IsEmployee { get; set; }
    }
}
