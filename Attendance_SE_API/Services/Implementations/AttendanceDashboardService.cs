using Attendance_SE_API.Services.Interfaces;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;
using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Response;

namespace Attendance_SE_API.Services.Implementations
{
    public class AttendanceDashboardService : IAttendanceDashboardService
    {
        private readonly IAttendanceDashboardRepository _repository;

        public AttendanceDashboardService(IAttendanceDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<DashboardAttendanceStatisticsResponse>> GetStudentAttendanceStatistics(int instituteId, string AcademicYearCode)
        {
            return await _repository.GetStudentAttendanceStatistics(instituteId, AcademicYearCode);
        }
        public async Task<ServiceResponse<List<GetStudentAttendanceDashboardResponse>>> GetStudentAttendanceDashboard(int instituteId, string AcademicYearCode)
        {
            return await _repository.GetStudentAttendanceDashboard(instituteId, AcademicYearCode);
        }
        public async Task<ServiceResponse<GetEmployeeAttendanceStatisticsResponse>> GetEmployeeAttendanceStatistics(int instituteId)
        {
            return await _repository.GetEmployeeAttendanceStatistics(instituteId);
        }
        public async Task<ServiceResponse<List<GetEmployeeOnLeaveResponse>>> GetEmployeeOnLeave(int instituteId)
        {
            return await _repository.GetEmployeeOnLeave(instituteId);
        }
    }
}
