namespace LibraryManagement_API.DTOs.Requests
{
    public class GetAllLibraryRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
