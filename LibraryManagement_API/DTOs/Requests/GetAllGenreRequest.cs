namespace LibraryManagement_API.DTOs.Requests
{
    public class GetAllGenreRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
