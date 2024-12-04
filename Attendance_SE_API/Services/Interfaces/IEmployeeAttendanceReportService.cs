using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_SE_API.Services.Interfaces
{
    public interface IEmployeeAttendanceReportService
    {
        Task<EmployeeAttendanceReportResponse> GetAttendanceReport(EmployeeAttendanceReportRequest request);
        Task<ServiceResponse<IEnumerable<GetAttendanceGeoFencingReportResponse>>> GetAttendanceGeoFencingReport(GetAttendanceGeoFencingReportRequest request);
        Task<MemoryStream> GenerateExcelReport(GetAttendanceGeoFencingReportRequest request);
        Task<MemoryStream> GenerateCSVReport(GetAttendanceGeoFencingReportRequest request);
        Task<List<AttendanceMode>> GetAttendanceMode();

    }
}
