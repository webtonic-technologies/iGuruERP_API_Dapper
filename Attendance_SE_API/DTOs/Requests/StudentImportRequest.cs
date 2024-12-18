namespace Attendance_SE_API.DTOs.Requests
{
    public class StudentImportRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int InstituteID { get; set; }
        public int TimeSlotTypeID { get; set; }
        public string AcademicYearCode { get; set; }
    }
}
