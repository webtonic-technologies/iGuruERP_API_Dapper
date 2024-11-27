using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.DTOs.Responses;

namespace Attendance_SE_API.Repository.Interfaces
{
    public interface IStudentAttendanceReportRepository
    {
        Task<StudentAttendanceReportResponse> GetAttendanceReport(StudentAttendanceReportRequest request);
        Task<StudentAttendanceReportPeriodWiseResponse> GetAttendanceReportPeriodWise(StudentAttendanceReportPeriodWiseRequest request);
        Task<byte[]> GetAttendanceReportExport(StudentAttendanceReportRequest request);
        Task<byte[]> GetAttendanceReportPeriodWiseExport(StudentAttendanceReportPeriodWiseRequest request);

    }
}
