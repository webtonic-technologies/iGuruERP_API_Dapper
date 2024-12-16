namespace Attendance_SE_API.DTOs.Responses
{
    public class SubjectAttendanceStatisticsResponse
    {
        public int TotalSessions { get; set; }
        public int TotalWorkingDays { get; set; }
        public decimal AverageAttendancePercentage { get; set; }
        public int StudentsWith100PercentAttendance { get; set; }
        public int StudentsAbove80PercentAttendance { get; set; }
    }
}
