using System.Threading.Tasks;
using Attendance_API.DTOs.Requests;
using Attendance_API.DTOs.Response;

namespace Attendance_API.Repository.Interfaces
{
    public interface IStudentAttendanceReportRepository
    {
        Task<StudentAttendanceReportResponse> GetAttendanceReport(StudentAttendanceReportRequest request);
    }
}
