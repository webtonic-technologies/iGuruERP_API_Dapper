namespace Communication_API.Models.DiscussionBoard
{
    public class DiscussionBoard
    {
        public int DiscussionBoardID { get; set; }
        public string DiscussionHeading { get; set; }
        public string Description { get; set; }
        public string Attachments { get; set; }
        public bool IsStudent { get; set; }
        public bool IsEmployee { get; set; }
    }
}
