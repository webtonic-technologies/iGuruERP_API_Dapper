namespace EventGallery_API.DTOs.Responses.Approvals
{
    public class GetAllHolidaysApprovalsResponse
    {
        public int HolidayID { get; set; }
        public string HolidayName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Description { get; set; }
        public List<ClassSectionResponse1> ClassSection { get; set; }
        public string Date { get; set; } // Formatted date
        public string ReviewedBy { get; set; } // Name of the reviewer
        public int StatusID { get; set; } // Status ID
    }

    public class ClassSectionResponse1
    {
        public string Class { get; set; }
        public string Section { get; set; }
    }
}
