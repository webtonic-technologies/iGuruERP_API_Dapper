namespace Attendance_SE_API.DTOs.Requests
{
    public class GetEmployeeAttendanceDashboardRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }  // Format: "yyyy-MM-dd"
        public string EndDate { get; set; }    // Format: "yyyy-MM-dd"

    }
}
