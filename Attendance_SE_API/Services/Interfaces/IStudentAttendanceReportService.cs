using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;

namespace Attendance_SE_API.Services.Interfaces
{
    public interface IStudentAttendanceReportService
    {
        Task<StudentAttendanceReportResponse> GetAttendanceReport(StudentAttendanceReportRequest request);
    }
}
