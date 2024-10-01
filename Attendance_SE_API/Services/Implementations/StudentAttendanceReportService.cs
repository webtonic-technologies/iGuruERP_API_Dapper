using System.Threading.Tasks;
using Attendance_API.DTOs.Requests;
using Attendance_API.DTOs.Response;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;


namespace Attendance_API.Services.Implementations
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
