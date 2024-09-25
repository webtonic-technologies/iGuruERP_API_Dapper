namespace EventGallery_API.DTOs.Requests
{
    public class HolidaySearchRequest
    {
        public int AcademicYearID { get; set; }
        public int InstituteID { get; set; }
        public string Search { get; set; } // Search by HolidayName
    }

}
