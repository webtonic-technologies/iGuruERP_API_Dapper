using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_API.Services.Interfaces
{
    public interface IEmployeeAttendanceService
    {
        Task<ServiceResponse<EmployeeAttendanceMasterResponseDTO>> GetEmployeeAttendanceMasterList(EmployeeAttendanceMasterRequestDTO request);
        Task<ServiceResponse<EmployeeAttendanceMasterDTO>> InsertOrUpdateEmployeeAttendanceMaster(EmployeeAttendanceMasterDTO employeeAttendanceMaster);
        Task<ServiceResponse<bool>> DeleteEmployeeAttendanceMaster(int employeeAttendanceId);
        Task<ServiceResponse<dynamic>> GetEmployeeAttendanceReport(EmployeeAttendanceReportRequestDTO request);
        Task<ServiceResponse<string>> ExportEmployeeAttendanceReportToExcel(EmployeeAttendanceReportRequestDTO request);
    }
}

