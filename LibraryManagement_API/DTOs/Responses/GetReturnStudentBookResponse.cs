namespace LibraryManagement_API.DTOs.Responses
{
    public class GetReturnStudentBookResponse
    {
        public BookDetails BookDetails { get; set; }
        public BorrowerDetails BorrowerDetails { get; set; }
        public string IssueDate { get; set; }  // Changed to string
        public string DueDate { get; set; }    // Changed to string

    }

    public class BookDetails
    {
        public int CatalogueID { get; set; }
        public int BookID { get; set; }
        public string LibraryName { get; set; }
        public string BookName { get; set; }
        public string AccessionNumber { get; set; }
    }

    public class BorrowerDetails
    {
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string RollNumber { get; set; }
        public string ClassSection { get; set; }
    }
}
