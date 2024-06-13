namespace Attendance_API.DTOs
{
    public class EmployeeAttendanceMasterRequestDTO
    {
        public int Department_id { get; set; }
        public DateTime Date {  get; set; }
        public int? pageNumber { get; set; }
        public int? pageSize { get; set; }
    }
}
