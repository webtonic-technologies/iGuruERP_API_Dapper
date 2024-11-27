
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;
namespace Attendance_SE_API.Services.Interfaces
{
    public interface IAttendanceDashboardService
    {
        Task<ServiceResponse<DashboardAttendanceStatisticsResponse>> GetStudentAttendanceStatistics(int instituteId);
        Task<ServiceResponse<List<GetStudentAttendanceDashboardResponse>>> GetStudentAttendanceDashboard(int instituteId);
        Task<ServiceResponse<GetEmployeeAttendanceStatisticsResponse>> GetEmployeeAttendanceStatistics(int instituteId);
        Task<ServiceResponse<List<GetEmployeeOnLeaveResponse>>> GetEmployeeOnLeave(int instituteId);

    }
}
