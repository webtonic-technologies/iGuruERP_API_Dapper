using Attendance_API.Models;

namespace Attendance_API.DTOs.Response
{
    public class AttendanceStatusResponse
    {
        public List<AttendanceStatus>? AttendanceStatuses { get; set; }
        public int? TotalCount { get; set; }
    }
}
