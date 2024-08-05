using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;

namespace Attendance_API.Services.Implementations
{
    public class StudentAttendanceDashboardService : IStudentAttendanceDashboardService
    {
        private readonly IStudentAttendanceDashboardRepo _studentAttendanceDashboardRepo;

        public StudentAttendanceDashboardService(IStudentAttendanceDashboardRepo studentAttendanceDashboardRepo)
        {
            _studentAttendanceDashboardRepo = studentAttendanceDashboardRepo;
        }

        public async Task<ServiceResponse<AttendanceCountsDTO>> GetAttendanceCountsForTodayAsync(int instituteId)
        {
            return await _studentAttendanceDashboardRepo.GetAttendanceCountsForTodayAsync(instituteId);
        }

        public async Task<ServiceResponse<IEnumerable<ClasswiseAttendanceCountsDTO>>> GetClasswiseAttendanceCountsForTodayAsync(int instituteId)
        {
            return await _studentAttendanceDashboardRepo.GetClasswiseAttendanceCountsForTodayAsync(instituteId);
        }
    }
}
