using System.Collections.Generic;
using System.Threading.Tasks;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Models;
using Attendance_API.DTOs;

namespace Attendance_API.Repository.Interfaces
{
    public interface IEmployeeAttendanceStatusMasterRepository
    {
        Task<ServiceResponse<List<EmployeeAttendanceStatusMasterDTO>>> GetEmployeeAttendanceStatusMasterList(int InstituteId);
        Task<ServiceResponse<EmployeeAttendanceStatusMasterDTO>> GetEmployeeAttendanceStatusMasterById(int Employee_Attendance_Status_id);
        Task<ServiceResponse<string>> SaveEmployeeAttendanceStatusMaster(List<EmployeeAttendanceStatusMasterDTO> request);
        Task<ServiceResponse<string>> DeleteEmployeeAttendanceStatusMaster(int Employee_Attendance_Status_id);
    }
}
