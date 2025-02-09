namespace Communication_API.DTOs.Responses.DiscussionBoard
{
    public class GetDiscussionBoardCommentsResponse
    {
        public int UserTypeID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Category { get; set; }
        public string Comment { get; set; }
    }
}
