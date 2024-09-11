using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Services.Interfaces
{
    public interface IStudentAttendanceReportService
    {
        Task<ServiceResponse<dynamic>> GetStudentAttendanceDatewiseReport(StudentAttendanceDatewiseReportRequestDTO request);
        Task<ServiceResponse<dynamic>> GetStudentSubjectwiseReport(SubjectwiseAttendanceReportRequest request);
        Task<ServiceResponse<string>> ExportStudentAttendanceDatewiseReportToExcel(StudentAttendanceDatewiseReportRequestDTO request);
        Task<ServiceResponse<string>> ExportStudentSubjectwiseReportToExcel(SubjectwiseAttendanceReportRequest request);
    }
}
