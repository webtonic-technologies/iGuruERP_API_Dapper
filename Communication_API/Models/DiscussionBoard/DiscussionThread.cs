namespace Communication_API.Models.DiscussionBoard
{
    public class DiscussionThread
    {
        public int CommentID { get; set; }
        public int DiscussionBoardID { get; set; }
        public int UserID { get; set; }
        public string Comments { get; set; }
        public DateTime CommentDate { get; set; }
    }
}
