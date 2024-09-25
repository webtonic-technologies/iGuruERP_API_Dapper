namespace EventGallery_API.DTOs.Requests
{
    public class HolidayRequest
    {
        public int? HolidayID { get; set; }
        public string HolidayName { get; set; }
        public string FromDate { get; set; } // Keep as string for flexible input format
        public string ToDate { get; set; } // Keep as string for flexible input format
        public string Description { get; set; }
        public List<ClassSectionRequest> ClassSection { get; set; }
        public int InstituteID { get; set; }
        public int AcademicYearID { get; set; }
    }


    public class ClassSectionRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }
}
