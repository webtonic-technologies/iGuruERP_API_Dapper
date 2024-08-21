using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Repository.Interfaces
{
    public interface IStudentAttendanceReportRepository
    {
        Task<ServiceResponse<dynamic>> GetStudentAttendanceDatewiseReport(StudentAttendanceDatewiseReportRequestDTO request);
        Task<ServiceResponse<dynamic>> GetStudentSubjectwiseReport(SubjectwiseAttendanceReportRequest request);
    }
}
