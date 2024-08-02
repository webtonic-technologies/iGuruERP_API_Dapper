namespace Attendance_API.DTOs
{
    public class EmployeeAttendanceReportRequestDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int instituteId {  get; set; }   
    }
}
