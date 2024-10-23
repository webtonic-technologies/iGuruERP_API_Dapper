using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.Models;
using Attendance_SE_API.ServiceResponse;

namespace Attendance_SE_API.Services.Interfaces
{
    public interface IStudentAttendanceStatusService
    {
        Task<ServiceResponse<string>> AddUpdateAttendanceStatus(List<AttendanceStatus> attendanceStatuses); // Update here
        Task<ServiceResponse<List<AttendanceStatus>>> GetAllAttendanceStatuses(GetAllAttendanceStatusRequest request);
        Task<ServiceResponse<AttendanceStatus>> GetAttendanceStatusById(int statusId);
        Task<ServiceResponse<bool>> DeleteStatus(int statusId);
    }
}
