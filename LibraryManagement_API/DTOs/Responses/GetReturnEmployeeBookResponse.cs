namespace LibraryManagement_API.DTOs.Responses
{
    public class GetReturnEmployeeBookResponse
    {
        public EBookDetails BookDetails { get; set; }
        public EBorrowerDetails BorrowerDetails { get; set; }
        public string IssueDate { get; set; }  // Changed to string
        public string DueDate { get; set; }    // Changed to string
    }

    public class EBookDetails
    {
        public int CatalogueID { get; set; }
        public int BookID { get; set; }
        public string LibraryName { get; set; }
        public string BookName { get; set; }
        public string AccessionNumber { get; set; }
    }

    public class EBorrowerDetails
    {
        public string EmployeeName { get; set; }
        public string EmployeeID { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
    }
}
