using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_API.Services.Interfaces
{
    public interface IEmployeeAttendanceService
    {
        Task<ServiceResponse<List<EmployeeAttendanceMasterResponseDTO>>> GetEmployeeAttendanceMasterList(EmployeeAttendanceMasterRequestDTO request);
        Task<ServiceResponse<EmployeeAttendanceMasterDTO>> InsertOrUpdateEmployeeAttendanceMaster(EmployeeAttendanceMasterDTO employeeAttendanceMaster);
        Task<ServiceResponse<bool>> DeleteEmployeeAttendanceMaster(int employeeAttendanceId);
    }
}

