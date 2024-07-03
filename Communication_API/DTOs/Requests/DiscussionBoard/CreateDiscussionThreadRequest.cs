namespace Communication_API.DTOs.Requests.DiscussionBoard
{
    public class CreateDiscussionThreadRequest
    {
        public int DiscussionBoardID { get; set; }
        public int UserID { get; set; }
        public string Comments { get; set; }
    }
}
