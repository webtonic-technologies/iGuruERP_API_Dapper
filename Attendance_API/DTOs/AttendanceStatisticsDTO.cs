namespace Attendance_API.DTOs
{
    public class AttendanceStatisticsDTO
    {
        public int TotalWorkingDays { get; set; }
        public double AverageAttendance { get; set; }
        public int StudentsWith100PercentAttendance { get; set; }
        public int StudentsAbove80PercentAttendance { get; set; }
    }
}
