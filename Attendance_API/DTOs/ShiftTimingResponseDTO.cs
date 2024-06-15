using Attendance_API.Models;

namespace Attendance_API.DTOs
{
    public class ShiftTimingResponseDTO
    {
        public IEnumerable<ShiftTimingResponse> Data { get; set; }
        public long Total { get; set; }
    }
    public class ShiftTimingResponse
    {
        public int Shift_Timing_id { get; set; }
        public string Clock_In { get; set; }
        public string Clock_Out { get; set; }
        public string Late_Coming { get; set; }
        public DateTime Applicable_Date { get; set; }
        public List<ShiftTimingDesignations> Designations { get; set; }
    }

    public class ShiftTimingDesignations
    {
        public int Designation_id { get; set; }
        public string DesignationName { get; set; }
    }
}
