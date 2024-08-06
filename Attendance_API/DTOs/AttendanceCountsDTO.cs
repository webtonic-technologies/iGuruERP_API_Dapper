namespace Attendance_API.DTOs
{
    public class AttendanceCountsDTO
    {
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int NotMarkedCount { get; set; }
        public int HalfDayCount { get; set; } 
        public int MedicalLeaveCount { get; set; } 
    }
}
