using Attendance_API.DTOs.Requests;
using Attendance_API.DTOs.Response;
using Attendance_API.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_API.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<ServiceResponse<List<StudentAttendanceResponse>>> GetAttendance(GetAttendanceRequest request);
        Task<ServiceResponse<bool>> SetAttendance(SetAttendanceRequest request);
    }
}
