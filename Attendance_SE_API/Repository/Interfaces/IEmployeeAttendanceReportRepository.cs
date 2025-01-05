using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_SE_API.Repository.Interfaces
{
    public interface IEmployeeAttendanceReportRepository
    {
        Task<EmployeeAttendanceReportResponse> GetAttendanceReport(EmployeeAttendanceReportRequest request);
        Task<IEnumerable<GetAttendanceGeoFencingReportResponse>> GetAttendanceGeoFencingReport(GetAttendanceGeoFencingReportRequest request);
        Task<MemoryStream> GenerateExcelReport(GetAttendanceGeoFencingReportRequest request); // Add this line
        Task<List<AttendanceMode>> GetAttendanceModes();
        Task<IEnumerable<GetAttendanceBioMericReportResponse>> GetAttendanceBioMericReport(GetAttendanceBioMericReportRequest request);
        Task<MemoryStream> GenerateBioMetricExcelReport(GetAttendanceBioMericReportRequest request); // Add this line

    }
}
