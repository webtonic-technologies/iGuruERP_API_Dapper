namespace LibraryManagement_API.DTOs.Responses
{
    public class LanguageResponse
    {
        public int LanguageID { get; set; }
        public int InstituteID { get; set; }
        public string LanguageName { get; set; }
        public bool IsActive { get; set; }
    }
}
