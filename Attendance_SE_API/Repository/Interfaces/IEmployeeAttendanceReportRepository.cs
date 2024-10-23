using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_SE_API.Repository.Interfaces
{
    public interface IEmployeeAttendanceReportRepository
    {
        Task<EmployeeAttendanceReportResponse> GetAttendanceReport(EmployeeAttendanceReportRequest request);

         
    }
}
