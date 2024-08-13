namespace LibraryManagement_API.DTOs.Responses
{
    public class LibraryFetchResponse
    {
        public int LibraryID { get; set; }
        public int InstituteID { get; set; }
        public string LibraryName { get; set; }
        public string ShortName { get; set; }
        public string LibraryIncharge { get; set; }
    }
}
