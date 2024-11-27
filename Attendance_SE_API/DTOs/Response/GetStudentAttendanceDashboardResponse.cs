namespace Attendance_SE_API.DTOs.Responses
{
    public class GetStudentAttendanceDashboardResponse
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int NotMarked { get; set; }
    }
}
