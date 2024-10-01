using Attendance_API.Models;

namespace Attendance_API.DTOs.Requests
{
    public class AddUpdateAttendanceStatusRequest
    {
        public List<AttendanceStatus> AttendanceStatus { get; set; } = new List<AttendanceStatus>();
    }
}
