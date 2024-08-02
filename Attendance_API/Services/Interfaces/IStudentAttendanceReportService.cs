using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Services.Interfaces
{
    public interface IStudentAttendanceReportService
    {
        Task<ServiceResponse<dynamic>> GetStudentAttendanceDatewiseReport(StudentAttendanceDatewiseReportRequestDTO request);
    }
}
