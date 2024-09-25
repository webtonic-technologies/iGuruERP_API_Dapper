namespace EventGallery_API.DTOs.Responses
{
    public class HolidayResponse
    {
        public int HolidayID { get; set; }
        public string HolidayName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Description { get; set; }
        public List<ClassSectionResponse> ClassSection { get; set; }

        // New property to hold formatted date
        public string Date { get; set; }  // Format: "DD-MM-YYYY to DD-MM-YYYY"
    }

    public class ClassSectionResponse
    {
        public string Class { get; set; }
        public string Section { get; set; }
    }
}
