namespace LibraryManagement_API.DTOs.Requests
{
    public class GetAllCataloguesRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; }
    }
}
