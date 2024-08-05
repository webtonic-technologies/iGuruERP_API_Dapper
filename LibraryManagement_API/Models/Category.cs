namespace LibraryManagement_API.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public int? InstituteID { get; set; }
        public string CategoryName { get; set; }
        public string Code { get; set; }
        public bool? IsActive { get; set; }
    }
}
