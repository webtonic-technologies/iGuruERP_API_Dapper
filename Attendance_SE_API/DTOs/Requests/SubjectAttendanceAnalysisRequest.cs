namespace Attendance_SE_API.DTOs.Requests
{
    public class SubjectAttendanceAnalysisRequest
    {
        public string AcademicYearCode { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int InstituteID { get; set; }
        public int SubjectID { get; set; }
    }

    public class SubjectAttendanceAnalysisRequest1
    {
        public string AcademicYearCode { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int InstituteID { get; set; } 
    }
}
