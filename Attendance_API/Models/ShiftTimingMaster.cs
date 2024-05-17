namespace Attendance_API.Models
{
    public class ShiftTimingMaster
    {
        public int Shift_Timing_id { get; set; }
        public string Clock_In { get; set; } = string.Empty;
        public string Clock_Out { get; set; } = string.Empty;
        public string Late_Coming { get; set; } = string.Empty;
        public DateTime Applicable_Date { get; set; }
    }
}
