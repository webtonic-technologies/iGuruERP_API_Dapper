namespace Attendance_SE_API.DTOs.Responses
{
    public class MonthlyAttendanceSubjectAnalysisResponse
    {
        public int AttendanceYear { get; set; }
        public int AttendanceMonth { get; set; }
        public decimal AverageAttendancePercentage { get; set; }
    }
}
