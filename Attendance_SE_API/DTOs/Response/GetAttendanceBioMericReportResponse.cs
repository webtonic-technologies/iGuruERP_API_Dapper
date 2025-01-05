namespace Attendance_SE_API.DTOs.Response
{
    public class GetAttendanceBioMericReportResponse
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string PrimaryMobileNumber { get; set; }
        public List<BioMetricAttendance> BioMetricAttendance { get; set; }
    }

    public class BioMetricAttendance
    {
        public string Date { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public string Time { get; set; }
    }

}
