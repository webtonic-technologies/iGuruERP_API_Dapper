using Institute_API.Helper;

namespace Institute_API.DTOs
{
    public class HolidayRequestDTO
    {
        public int Holiday_id { get; set; }
        public string HolidayName { get; set; }

        [ValidDateString("dd-MM-yyyy hh:mm tt")]
        public string StartDate { get; set; }
        [ValidDateString("dd-MM-yyyy hh:mm tt")]
        public string EndDate { get; set; }
        public string Description { get; set; }
        //public bool IsApproved { get; set; }
        //public int? ApprovedBy { get; set; }
        public int Institute_id {  get; set; }
        public int Academic_year_id { get; set; }
        public List<HolidayClassSessionMapping> ClassSessionMappings { get; set; }
    }

    public class HolidayClassSessionMapping
    {
        public int HolidayClassSessionMapping_id { get; set; }
        public int Holiday_id { get; set; }
        public int Class_id { get; set; }
        public int Section_id { get; set; }
    }
}
