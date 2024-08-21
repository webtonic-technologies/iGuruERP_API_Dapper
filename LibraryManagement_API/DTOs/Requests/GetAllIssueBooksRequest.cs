namespace LibraryManagement_API.DTOs.Requests
{
    public class GetAllIssueBooksRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
