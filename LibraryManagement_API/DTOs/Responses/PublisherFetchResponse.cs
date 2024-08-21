namespace LibraryManagement_API.DTOs.Responses
{
    public class PublisherFetchResponse
    {
        public int PublisherID { get; set; }
        public int InstituteID { get; set; }
        public string PublisherName { get; set; }
        public string MobileNumber { get; set; }
        public int? CountryID { get; set; }
        public bool IsActive { get; set; }
    }
}
