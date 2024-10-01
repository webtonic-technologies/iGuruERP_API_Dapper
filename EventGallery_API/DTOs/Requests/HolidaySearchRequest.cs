namespace EventGallery_API.DTOs.Requests
{
    public class HolidaySearchRequest
    {
        public string AcademicYearCode { get; set; }
        public int InstituteID { get; set; }
        public string Search { get; set; } // Search by HolidayName

    }

}
