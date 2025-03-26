namespace StudentManagement_API.DTOs.Requests
{
    public class GetApprovedHistoryRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string AcademicYearCode { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Search { get; set; }
    }
}
