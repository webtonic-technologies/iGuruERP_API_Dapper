namespace LibraryManagement_API.DTOs.Responses
{
    public class PublisherResponse
    {
        public int PublisherID { get; set; }
        public int InstituteID { get; set; }
        public string PublisherName { get; set; }
        public string MobileNumber { get; set; }
        public string CountryName { get; set; } // Include this in the response DTO
        public bool IsActive { get; set; }
    }
}
