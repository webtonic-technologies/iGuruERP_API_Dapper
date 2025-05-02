namespace FeesManagement_API.DTOs.Requests
{
    public class StudentPreviousFeeRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string? Search { get; set; }
        public string AcademicYearCode { get; set; }
    }
}
