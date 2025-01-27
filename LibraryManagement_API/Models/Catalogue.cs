namespace LibraryManagement_API.Models
{
    public class Catalogue
    {
        public int CatalogueID { get; set; }
        public int? InstituteID { get; set; }
        public string BookName { get; set; }
        public string ISBN10 { get; set; }
        public string ISBN13 { get; set; }
        public string AccessionNumber { get; set; }
        public decimal Price { get; set; }
        public string Funding { get; set; }
        public int NumberOfPages { get; set; }
        public int BookID { get; set; }
        public int LibraryID { get; set; }
        public string DateOfPurchase { get; set; }
        public string LocationID { get; set; }
        public string PublishedDate { get; set; }
        public string Edition { get; set; }
        public string Volume { get; set; }
        public string FundingSource { get; set; }
        public string Comments { get; set; }
        public int PublisherID { get; set; }
        public int LanguageID { get; set; }
        public int LibraryCategoryID { get; set; }
        public int GenreID { get; set; }
        public bool? IsActive { get; set; }
        public List<int> AuthorIDs { get; set; }
    }
}
