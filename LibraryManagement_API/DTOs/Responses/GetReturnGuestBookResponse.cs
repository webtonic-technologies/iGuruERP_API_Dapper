namespace LibraryManagement_API.DTOs.Responses
{
    public class GetReturnGuestBookResponse
    {
        public GBookDetails BookDetails { get; set; }
        public GBorrowerDetails BorrowerDetails { get; set; }
        public string IssueDate { get; set; }  // Changed to string
        public string DueDate { get; set; }    // Changed to string
    }
    public class GBookDetails
    {
        public int CatalogueID { get; set; }
        public int BookID { get; set; }
        public string LibraryName { get; set; }
        public string BookName { get; set; }
        public string AccessionNumber { get; set; }
    }

    public class GBorrowerDetails
    {
        public string GuestName { get; set; }
        public string GuestType { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
    }
}
