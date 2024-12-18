
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;
namespace Attendance_SE_API.Repository.Interfaces
{
    public interface IAttendanceDashboardRepository
    {
        Task<ServiceResponse<DashboardAttendanceStatisticsResponse>> GetStudentAttendanceStatistics(int instituteId, string AcademicYearCode);
        Task<ServiceResponse<List<GetStudentAttendanceDashboardResponse>>> GetStudentAttendanceDashboard(int instituteId, string AcademicYearCode);
        Task<ServiceResponse<GetEmployeeAttendanceStatisticsResponse>> GetEmployeeAttendanceStatistics(int instituteId);
        Task<ServiceResponse<List<GetEmployeeOnLeaveResponse>>> GetEmployeeOnLeave(int instituteId);

    }
}
