namespace Attendance_SE_API.DTOs.Response
{
    public class ShiftResponse
    {
        public int ShiftID { get; set; }
        public TimeSpan ClockIn { get; set; }
        public TimeSpan ClockOut { get; set; }
        public TimeSpan LateComing { get; set; }
        public List<DesignationResponse> Designations { get; set; } // New property for designations
    }

    public class DesignationResponse
    {
        public string DesignationName { get; set; }
    }
}
