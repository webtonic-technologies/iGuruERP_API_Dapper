namespace Attendance_SE_API.DTOs.Responses
{
    public class AttendanceStatisticsResponse
    {
        public int TotalStudents { get; set; }
        public int TotalWorkingDays { get; set; }
        public float AverageAttendancePercentage { get; set; }
        public int StudentsWith100PercentAttendance { get; set; }
        public int StudentsAbove80PercentAttendance { get; set; }
    }
}
