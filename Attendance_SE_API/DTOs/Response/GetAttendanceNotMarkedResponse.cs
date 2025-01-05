namespace Attendance_SE_API.DTOs.Responses
{
    public class GetAttendanceNotMarkedResponse
    {
        public string Class { get; set; }
        public string Section { get; set; }
        public int SectionStrength { get; set; }
        public int EmployeeId { get; set; }
        public string ClassTeacher { get; set; }
    }
}
