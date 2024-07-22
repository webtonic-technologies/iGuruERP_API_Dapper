namespace LibraryManagement_API.Models
{
    public class Language
    {
        public int LanguageID { get; set; }
        public int? InstituteID { get; set; }
        public string LanguageName { get; set; }
        public bool? IsActive { get; set; }
    }
}
