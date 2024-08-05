namespace LibraryManagement_API.DTOs.Responses
{
    public class IssueBookResponse
    {
        public int BookID { get; set; }
        public string Library { get; set; }
        public string BookName { get; set; }
        public string ISBN10 { get; set; }
        public string ISBN13 { get; set; }
        public string AccessionNumber { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
    }
}
