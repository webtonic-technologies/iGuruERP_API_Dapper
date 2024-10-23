using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;

namespace Attendance_SE_API.Repository.Interfaces
{
    public interface IStudentAttendanceReportRepository
    {
        Task<StudentAttendanceReportResponse> GetAttendanceReport(StudentAttendanceReportRequest request);
    }
}
