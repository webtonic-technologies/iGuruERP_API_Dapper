using Attendance_API.Models;

namespace Attendance_API.DTOs
{
    public class EmployeeAttendanceMasterRequestDTO
    {
        public int Department_id { get; set; }
        [ValidDateString("dd-MM-yyyy hh:mm tt")]
        public string Date {  get; set; }
        public int? pageNumber { get; set; }
        public int? pageSize { get; set; }
    }
}
