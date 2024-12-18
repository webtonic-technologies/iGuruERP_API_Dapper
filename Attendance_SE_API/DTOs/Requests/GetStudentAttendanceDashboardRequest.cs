namespace Attendance_SE_API.DTOs.Requests
{
    public class GetStudentAttendanceDashboardRequest
    {
        public int InstituteID { get; set; }
        public string AcademicYearCode { get; set; } 
    }
}
