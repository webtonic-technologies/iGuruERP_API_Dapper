namespace Attendance_SE_API.DTOs.Responses
{
    public class AttendanceDetails
    {
        public string Date { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public string Time { get; set; }
    }

    public class GetAttendanceGeoFencingReportResponse
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string PrimaryMobileNumber { get; set; }
        public List<AttendanceDetails> Attendance { get; set; }
    }
}
