namespace LibraryManagement_API.Models
{
    public class Genre
    {
        public int GenreID { get; set; }
        public int? InstituteID { get; set; }
        public string GenreName { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
