namespace Attendance_SE_API.DTOs.Response
{
    public class DashboardAttendanceStatisticsResponse
    {
        public int NoOfPresent { get; set; }
        public int NoOfAbsent { get; set; }
        public int NoOfNotMarked { get; set; }
    }
}
