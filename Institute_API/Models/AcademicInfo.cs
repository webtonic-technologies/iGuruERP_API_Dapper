namespace Institute_API.Models
{
    public class AcademicInfo
    {
        public int Academic_Info_id { get; set; }
        public int Institute_id { get; set; }
        public DateTime AcademicYearStartMonth { get; set; }
        public DateTime AcademicYearEndMonth { get; set; }
    }
}
