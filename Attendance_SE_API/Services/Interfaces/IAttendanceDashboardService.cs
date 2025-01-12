
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;
namespace Attendance_SE_API.Services.Interfaces
{
    public interface IAttendanceDashboardService
    {
        Task<ServiceResponse<DashboardAttendanceStatisticsResponse>> GetStudentAttendanceStatistics(int instituteId, string AcademicYearCode);
        Task<ServiceResponse<List<GetStudentAttendanceDashboardResponse>>> GetStudentAttendanceDashboard(int instituteId, string AcademicYearCode, string startDate, string endDate);
        Task<ServiceResponse<GetEmployeeAttendanceStatisticsResponse>> GetEmployeeAttendanceStatistics(int instituteId);
        Task<ServiceResponse<List<GetEmployeeOnLeaveResponse>>> GetEmployeeOnLeave(int instituteId);
        Task<ServiceResponse<List<GetAttendanceNotMarkedResponse>>> GetAttendanceNotMarked(int instituteId);
        Task<ServiceResponse<List<GetAbsentStudentsResponse>>> GetAbsentStudents(int instituteId);
        Task<ServiceResponse<GetStudentsMLCountResponse>> GetStudentsMLCount(int instituteId);
        Task<ServiceResponse<GetHalfDayLeaveCountResponse>> GetHalfDayLeaveCount(int instituteId);
        Task<ServiceResponse<byte[]>> GetAttendanceNotMarkedExport(GetAttendanceNotMarkedExportRequest request);
        Task<ServiceResponse<byte[]>> GetAbsentStudentsExport(GetAbsentStudentsExportRequest request);
        Task<ServiceResponse<GetEmployeeAttendanceDashboardResponse>> GetEmployeeAttendance(int instituteId, string startDate, string endDate);
        Task<ServiceResponse<List<GetEmployeesArrivalStatsResponse>>> GetEmployeesArrivalStats(int instituteId, string startDate, string endDate);

    }
}
