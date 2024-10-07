namespace Institute_API.Models
{
    public class AcademicInfo
    {
        public int Academic_Info_id { get; set; }
        public int Institute_id { get; set; }
        public string AcademicYearStartMonth { get; set; }
        public string AcademicYearEndMonth { get; set; }
        public bool Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string AcaInfoYearCode { get; set; } = string.Empty;
    }
}
