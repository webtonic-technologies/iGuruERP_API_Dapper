namespace LibraryManagement_API.DTOs.Requests
{
    public class AddUpdateLanguageRequest
    {
        public List<LanguageDto> Languages { get; set; }
    }

    public class LanguageDto
    {
        public int LanguageID { get; set; }
        public int InstituteID { get; set; }
        public string LanguageName { get; set; }
        public bool IsActive { get; set; }
    }
}
