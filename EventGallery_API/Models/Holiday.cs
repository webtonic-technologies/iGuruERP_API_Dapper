namespace EventGallery_API.Models
{
    public class Holiday
    {
        public int HolidayID { get; set; }
        public string HolidayName { get; set; }
        public DateTime HolidayDate { get; set; }
        public string Description { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
