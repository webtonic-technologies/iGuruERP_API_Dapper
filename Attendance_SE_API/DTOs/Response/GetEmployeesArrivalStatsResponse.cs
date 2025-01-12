namespace Attendance_SE_API.DTOs.Responses
{
    public class GetEmployeesArrivalStatsResponse
    {
        public string AttendanceDate { get; set; }  // Format: "DD-MM-YYYY"
        public int EarlyOnTimeArrivals { get; set; }
        public int LateArrivals { get; set; }
        public int NoLogs { get; set; }
    }
}
