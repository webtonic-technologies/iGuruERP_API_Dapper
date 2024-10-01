using System.Threading.Tasks;
using Attendance_API.DTOs.Requests;
using Attendance_API.DTOs.Response;

namespace Attendance_API.Services.Interfaces
{
    public interface IStudentAttendanceReportService
    {
        Task<StudentAttendanceReportResponse> GetAttendanceReport(StudentAttendanceReportRequest request);
    }
}
