namespace Attendance_API.DTOs
{
    public class ShiftTimingRequestDTO
    {
        public int Shift_Timing_id { get; set; }
        public TimeSpan Clock_In { get; set; }
        public TimeSpan Clock_Out { get; set; }
        public TimeSpan Late_Coming { get; set; }
        public DateTime Applicable_Date { get; set; }
        public List<int> Designations { get; set; }
    }
}
