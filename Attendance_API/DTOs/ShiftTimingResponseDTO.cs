using Attendance_API.Models;

namespace Attendance_API.DTOs
{
    public class ShiftTimingResponseDTO
    {
        public int Shift_Timing_id { get; set; }
        public TimeSpan Clock_In { get; set; }
        public TimeSpan Clock_Out { get; set; }
        public TimeSpan Late_Coming { get; set; }
        public DateTime Applicable_Date { get; set; }
        public List<ShiftTimingDesignations> Designations { get; set; }
    }

    public class ShiftTimingDesignations
    {
        public int Designation_id { get; set; }
        public string DesignationName { get; set; }
    }
}
