namespace Attendance_SE_API.DTOs.Responses
{
    public class MonthlyAttendanceAnalysisResponse
    {
        public int AttendanceYear { get; set; }
        public int AttendanceMonth { get; set; }
        public float AverageAttendancePercentage { get; set; }
    }
}
