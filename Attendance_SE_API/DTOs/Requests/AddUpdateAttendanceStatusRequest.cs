using Attendance_SE_API.Models;

namespace Attendance_SE_API.DTOs.Requests
{
    public class AddUpdateAttendanceStatusRequest
    {
        public List<AttendanceStatus> AttendanceStatus { get; set; } = new List<AttendanceStatus>();
    }
}
