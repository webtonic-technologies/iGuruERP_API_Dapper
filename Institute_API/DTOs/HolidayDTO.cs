namespace Institute_API.DTOs
{
    public class HolidayDTO
    {
        public int Holiday_id { get; set; }
        public string HolidayName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public bool IsApproved { get; set; }
        public int? ApprovedBy { get; set; }
        public List<HolidayClassSessionMappingDTO> ClassSessionMappings { get; set; }
    }

    public class HolidayClassSessionMappingDTO
    {
        public int HolidayClassSessionMapping_id { get; set; }
        public int Holiday_id { get; set; }
        public int Class_id { get; set; }
        public int Section_id { get; set; }
    }
}
