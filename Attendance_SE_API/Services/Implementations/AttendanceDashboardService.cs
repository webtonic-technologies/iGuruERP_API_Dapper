using Attendance_SE_API.Services.Interfaces;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;
using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Repository.Implementations;

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
        public async Task<ServiceResponse<List<GetStudentAttendanceDashboardResponse>>> GetStudentAttendanceDashboard(int instituteId, string AcademicYearCode, string startDate, string endDate)
        {
            return await _repository.GetStudentAttendanceDashboard(instituteId, AcademicYearCode, startDate, endDate);
        }
        public async Task<ServiceResponse<GetEmployeeAttendanceStatisticsResponse>> GetEmployeeAttendanceStatistics(int instituteId)
        {
            return await _repository.GetEmployeeAttendanceStatistics(instituteId);
        }
        public async Task<ServiceResponse<List<GetEmployeeOnLeaveResponse>>> GetEmployeeOnLeave(int instituteId)
        {
            return await _repository.GetEmployeeOnLeave(instituteId);
        }

        public async Task<ServiceResponse<List<GetAttendanceNotMarkedResponse>>> GetAttendanceNotMarked(int instituteId)
        {
            var data = await _repository.GetAttendanceNotMarked(instituteId);

            if (data != null)
            {
                return new ServiceResponse<List<GetAttendanceNotMarkedResponse>>(true, "Data retrieved successfully", data, 200);
            }

            return new ServiceResponse<List<GetAttendanceNotMarkedResponse>>(false, "No data found", null, 404);
        }

        public async Task<ServiceResponse<List<GetAbsentStudentsResponse>>> GetAbsentStudents(int instituteId)
        {
            var data = await _repository.GetAbsentStudents(instituteId);

            if (data != null)
            {
                return new ServiceResponse<List<GetAbsentStudentsResponse>>(true, "Absent students data fetched successfully.", data, 200);
            }

            return new ServiceResponse<List<GetAbsentStudentsResponse>>(false, "No data found for absent students.", null, 404);
        }

        public async Task<ServiceResponse<GetStudentsMLCountResponse>> GetStudentsMLCount(int instituteId)
        {
            var data = await _repository.GetStudentsMLCount(instituteId);

            if (data != null)
            {
                return new ServiceResponse<GetStudentsMLCountResponse>(true, "Medical leave count fetched successfully.", data, 200);
            }

            return new ServiceResponse<GetStudentsMLCountResponse>(false, "No data found for medical leave count.", null, 404);
        }

        public async Task<ServiceResponse<GetHalfDayLeaveCountResponse>> GetHalfDayLeaveCount(int instituteId)
        {
            var data = await _repository.GetHalfDayLeaveCount(instituteId);

            if (data != null)
            {
                return new ServiceResponse<GetHalfDayLeaveCountResponse>(true, "Half-day leave count fetched successfully.", data, 200);
            }

            return new ServiceResponse<GetHalfDayLeaveCountResponse>(false, "No data found for half-day leave count.", null, 404);
        }
    }
}
