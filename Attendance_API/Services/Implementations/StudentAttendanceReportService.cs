using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;

namespace Attendance_API.Services.Implementations
{
    public class StudentAttendanceReportService : IStudentAttendanceReportService
    {
        private readonly IStudentAttendanceReportRepository _studentAttendanceReportRepository;
        public StudentAttendanceReportService(IStudentAttendanceReportRepository studentAttendanceReportRepository)
        {
            _studentAttendanceReportRepository = studentAttendanceReportRepository; 
        }
        public async Task<ServiceResponse<dynamic>> GetStudentAttendanceDatewiseReport(StudentAttendanceDatewiseReportRequestDTO request)
        {
            return await _studentAttendanceReportRepository.GetStudentAttendanceDatewiseReport(request);
        }
    }
}
