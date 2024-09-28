namespace Institute_API.Models
{
    public class AcademicInfo
    {
        public int Academic_Info_id { get; set; }
        public int Institute_id { get; set; }
        public DateTime AcademicYearStartMonth { get; set; }
        public DateTime AcademicYearEndMonth { get; set; }
        public bool Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string AcaInfoYearCode { get; set; } = string.Empty;
    }
}
