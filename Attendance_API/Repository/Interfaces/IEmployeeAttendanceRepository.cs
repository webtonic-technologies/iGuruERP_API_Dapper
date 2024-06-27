using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Repository.Interfaces
{
    public interface IEmployeeAttendanceRepository
    {
        Task<ServiceResponse<EmployeeAttendanceMasterResponseDTO>> GetEmployeeAttendanceMasterList(EmployeeAttendanceMasterRequestDTO request);
        Task<ServiceResponse<EmployeeAttendanceMasterDTO>> InsertOrUpdateEmployeeAttendanceMaster(EmployeeAttendanceMasterDTO employeeAttendanceMaster);
        Task<ServiceResponse<bool>> DeleteEmployeeAttendanceMaster(int employeeAttendanceId);
    }
}