namespace Communication_API.DTOs.Requests.DiscussionBoard
{
    public class AddDiscussionBoardReactionRequest
    {
        public int DiscussionBoardID { get; set; }
        public bool Reaction { get; set; }
        public int UserID { get; set; }
        public int UserTypeID { get; set; }
    }
}
