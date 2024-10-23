using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.Services.Interfaces;


namespace Attendance_SE_API.Services.Implementations
{
    public class StudentAttendanceReportService : IStudentAttendanceReportService
    {
        private readonly IStudentAttendanceReportRepository _attendanceReportRepository;

        public StudentAttendanceReportService(IStudentAttendanceReportRepository attendanceReportRepository)
        {
            _attendanceReportRepository = attendanceReportRepository;
        }

        public async Task<StudentAttendanceReportResponse> GetAttendanceReport(StudentAttendanceReportRequest request)
        {
            return await _attendanceReportRepository.GetAttendanceReport(request);
        }
    }
}
