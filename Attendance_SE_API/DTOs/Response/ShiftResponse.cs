namespace Attendance_SE_API.DTOs.Response
{
    public class ShiftResponse
    {
        public int ShiftID { get; set; }
        public string ClockIn { get; set; }  // Changed to string
        public string ClockOut { get; set; }  // Changed to string
        public string LateComing { get; set; }  // Changed to string
        public List<DesignationResponse> Designations { get; set; } // New property for designations
    }

    public class DesignationResponse
    {
        public string DesignationName { get; set; }
    }
}
