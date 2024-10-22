using Attendance_SE_API.Models;

namespace Attendance_SE_API.DTOs.Response
{
    public class AttendanceStatusResponse
    {
        public List<AttendanceStatus>? AttendanceStatuses { get; set; }
        public int? TotalCount { get; set; }
    }
}
