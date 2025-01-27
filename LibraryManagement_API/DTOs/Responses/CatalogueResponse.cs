namespace LibraryManagement_API.DTOs.Responses
{
    public class CatalogueResponse
    {
        public int CatalogueID { get; set; }
        public int BookID { get; set; }
        public string LibraryName { get; set; }
        public string BookName { get; set; }
        public string ISBN10 { get; set; }
        public string ISBN13 { get; set; }
        public string AccessionNumber { get; set; } 
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string Category { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
        public string Funding { get; set; }
        public int NumberOfPages { get; set; }
        public List<AuthorResponse1> Authors { get; set; } = new List<AuthorResponse1>(); 
    }

    public class AuthorResponse1
    {
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
    }
}
