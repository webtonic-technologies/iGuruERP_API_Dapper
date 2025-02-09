namespace Communication_API.DTOs.Responses.DiscussionBoard
{
    public class GetAllDiscussionResponse
    {
        public int DiscussionBoardID { get; set; }
        public string DiscussionHeading { get; set; }
        // Formatted as "26th Apr 2024, 07:00 PM"
        public string CreatedOn { get; set; }
    }
}
