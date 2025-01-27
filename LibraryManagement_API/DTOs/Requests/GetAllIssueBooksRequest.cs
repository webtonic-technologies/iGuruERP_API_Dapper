namespace LibraryManagement_API.DTOs.Requests
{
    public class GetAllIssueBooksRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }  // Add StartDate (Format: DD-MM-YYYY)
        public string EndDate { get; set; }    // Add EndDate (Format: DD-MM-YYYY) 
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
