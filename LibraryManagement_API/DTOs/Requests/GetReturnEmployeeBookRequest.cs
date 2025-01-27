namespace LibraryManagement_API.DTOs.Requests
{
    public class GetReturnEmployeeBookRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }  // Changed to string
        public string EndDate { get; set; }    // Changed to string
    }
}
