namespace Attendance_SE_API.DTOs.Response
{
    public class GetEmployeeAttendanceDashboardResponse
    {
        public double EmployeePresent { get; set; }
        public double AvgLateLogins { get; set; }
        public double AvgOnTimeArrival { get; set; }
    }
}
