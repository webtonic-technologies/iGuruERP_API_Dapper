namespace StudentManagement_API.DTOs.Requests
{
    public class GetRejectedHistoryRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        // Dates as strings in "dd-MM-yyyy" format
        public string AcademicYearCode { get; set; }  // New property added
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Search { get; set; }
    }
}
