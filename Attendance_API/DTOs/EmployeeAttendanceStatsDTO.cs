namespace Attendance_API.DTOs
{
    public class EmployeeAttendanceStatsDTO
    {
        public int TotalEmployeeCount { get; set; } 
        public int PresentCount { get; set; } 
        public int OnLeaveCount { get; set; } 
        public int NotMarkedCount { get; set; } 
        public int LateLoginCount { get; set; } 
    }
}
