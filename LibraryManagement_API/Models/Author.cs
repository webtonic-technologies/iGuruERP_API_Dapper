namespace LibraryManagement_API.Models
{
    public class Author
    {
        public int AuthorID { get; set; }
        public int? InstituteID { get; set; }
        public string AuthorName { get; set; }
        public bool? IsActive { get; set; }
    }
}
