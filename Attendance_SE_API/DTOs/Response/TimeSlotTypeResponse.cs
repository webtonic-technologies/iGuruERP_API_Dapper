namespace Attendance_SE_API.DTOs.Responses
{
    public class TimeSlotTypeResponse
    {
        public int TimeSlotTypeID { get; set; }
        public string TimeSlotType { get; set; } = string.Empty;
        public double AttendanceScore { get; set; }
    }
}
