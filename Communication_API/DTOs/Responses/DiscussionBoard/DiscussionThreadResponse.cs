namespace Communication_API.DTOs.Responses.DiscussionBoard
{
    public class DiscussionThreadResponse
    {
        public int CommentID { get; set; }
        public int DiscussionBoardID { get; set; }
        public int UserID { get; set; }
        public string Comments { get; set; }
        public DateTime CommentDate { get; set; }
    }
}
