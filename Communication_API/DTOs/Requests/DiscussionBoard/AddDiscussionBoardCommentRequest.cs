namespace Communication_API.DTOs.Requests.DiscussionBoard
{
    public class AddDiscussionBoardCommentRequest
    {
        public int DiscussionBoardID { get; set; }
        public int UserID { get; set; }
        public int UserTypeID { get; set; }
        public string Comments { get; set; }
    }
}
