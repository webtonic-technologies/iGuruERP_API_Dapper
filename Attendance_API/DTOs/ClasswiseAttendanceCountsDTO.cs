namespace Attendance_API.DTOs
{
    public class ClasswiseAttendanceCountsDTO
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int NotMarkedCount { get; set; }
    }
}
