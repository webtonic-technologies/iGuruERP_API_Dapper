
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;
namespace Attendance_SE_API.Repository.Interfaces
{
    public interface IAttendanceDashboardRepository
    {
        Task<ServiceResponse<DashboardAttendanceStatisticsResponse>> GetStudentAttendanceStatistics(int instituteId, string AcademicYearCode);
        Task<ServiceResponse<List<GetStudentAttendanceDashboardResponse>>> GetStudentAttendanceDashboard(int instituteId, string AcademicYearCode, string startDate, string endDate);
        Task<ServiceResponse<GetEmployeeAttendanceStatisticsResponse>> GetEmployeeAttendanceStatistics(int instituteId);
        Task<ServiceResponse<List<GetEmployeeOnLeaveResponse>>> GetEmployeeOnLeave(int instituteId);
        Task<List<GetAttendanceNotMarkedResponse>> GetAttendanceNotMarked(int instituteId);
        Task<List<GetAbsentStudentsResponse>> GetAbsentStudents(int instituteId);
        Task<GetStudentsMLCountResponse> GetStudentsMLCount(int instituteId);
        Task<GetHalfDayLeaveCountResponse> GetHalfDayLeaveCount(int instituteId);
        Task<IEnumerable<GetAttendanceNotMarkedExportResponse>> GetAttendanceNotMarkedExport(GetAttendanceNotMarkedExportRequest request);
        Task<IEnumerable<GetAbsentStudentsExportResponse>> GetAbsentStudentsExport(GetAbsentStudentsExportRequest request);
        Task<GetEmployeeAttendanceDashboardResponse> GetEmployeeAttendanceStatistics(int instituteId, string startDate, string endDate);
        Task<List<GetEmployeesArrivalStatsResponse>> GetEmployeesArrivalStats(int instituteId, string startDate, string endDate);

    }
}
