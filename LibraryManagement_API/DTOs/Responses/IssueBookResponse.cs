namespace LibraryManagement_API.DTOs.Responses
{
    public class IssueBookResponse
    {
        public int IssueBookID { get; set; }
        public string IssuedOn { get; set; }
        public string BorrowerType { get; set; }
        public int BookID { get; set; }
        public string BookName { get; set; }
        public string ISBN10 { get; set; }
        public string ISBN13 { get; set; }
        public string AccessionNumber { get; set; } 
        public string Publisher { get; set; }
        public string LibraryName { get; set; }
        public List<AuthorResponse2> Authors { get; set; } = new List<AuthorResponse2>();
    }

    public class AuthorResponse2
    {
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
    }
}
