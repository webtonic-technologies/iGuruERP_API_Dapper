namespace LibraryManagement_API.DTOs.Responses
{
    public class AuthorFetchResponse
    {
        public int AuthorID { get; set; }
        public int InstituteID { get; set; }
        public string AuthorName { get; set; }
        public bool IsActive { get; set; }
    }
}
