using Attendance_API.DTOs.Requests;
using Attendance_API.Models;
using Attendance_API.ServiceResponse;

namespace Attendance_API.Repository.Interfaces
{
    public interface IStudentAttendanceStatusRepository
    {
        Task<ServiceResponse<string>> AddUpdateAttendanceStatus(AttendanceStatus request);
        Task<ServiceResponse<List<AttendanceStatus>>> GetAllAttendanceStatuses(GetAllAttendanceStatusRequest request);
        Task<ServiceResponse<AttendanceStatus>> GetAttendanceStatusById(int statusId);
        Task<ServiceResponse<bool>> DeleteStatus(int statusId);
    }
}
