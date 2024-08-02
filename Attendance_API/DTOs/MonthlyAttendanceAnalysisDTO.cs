namespace Attendance_API.DTOs
{
    public class MonthlyAttendanceAnalysisDTO
    {
        public string MonthYear { get; set; } 
        public decimal AverageAttendancePercentage { get; set; }
    }
}
