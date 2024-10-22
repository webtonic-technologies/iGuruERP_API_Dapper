namespace Attendance_SE_API.DTOs.Requests
{
    public class ShiftRequest
    {
        public int ShiftID { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public string LateComing { get; set; }
        public int InstituteID { get; set; } // Added InstituteID
        public List<ShiftDesignation>? Designations { get; set; } // This should be required
    }

    public class ShiftDesignation
    {
        public int ShiftID { get; set; }
        public int DesignationID { get; set; }
    }
}
