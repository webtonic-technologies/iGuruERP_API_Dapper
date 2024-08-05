namespace LibraryManagement_API.DTOs.Responses
{
    public class GenreResponse
    {
        public int GenreID { get; set; }
        public int InstituteID { get; set; }
        public string GenreName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
