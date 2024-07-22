namespace LibraryManagement_API.Models
{
    public class Library
    {
        public int LibraryID { get; set; }
        public int? InstituteID { get; set; }
        public string LibraryName { get; set; }
        public string ShortName { get; set; }
        public int? LibraryInchargeID { get; set; }
        public bool? IsActive { get; set; }
    }
}
