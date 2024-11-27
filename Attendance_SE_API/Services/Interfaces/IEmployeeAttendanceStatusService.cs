using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Models;
using Attendance_SE_API.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_SE_API.Services.Interfaces
{
    public interface IEmployeeAttendanceStatusService
    {
        Task<ServiceResponse<string>> AddUpdateAttendanceStatus(List<AttendanceStatus> attendanceStatuses);
        Task<ServiceResponse<List<AttendanceStatus>>> GetAllAttendanceStatuses(GetAllAttendanceStatusRequest request);
        Task<ServiceResponse<List<AttendanceStatus>>> GetAllAttendanceStatusesDDL(GetAllAttendanceStatusDDLRequest request);

        Task<ServiceResponse<AttendanceStatus>> GetAttendanceStatusById(int statusId);
        Task<ServiceResponse<bool>> DeleteStatus(int statusId);
    }
}
