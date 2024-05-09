using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Models;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;

namespace Attendance_API.Services.Implementations
{
    public class EmployeeAttendanceStatusMasterService : IEmployeeAttendanceStatusMasterService
    {
        private readonly IEmployeeAttendanceStatusMasterRepository _employeeAttendanceStatusMasterRepository;

        public EmployeeAttendanceStatusMasterService(IEmployeeAttendanceStatusMasterRepository employeeAttendanceStatusMasterRepository)
        {
            _employeeAttendanceStatusMasterRepository = employeeAttendanceStatusMasterRepository;
        }

        public async Task<ServiceResponse<List<EmployeeAttendanceStatusMasterDTO>>> GetEmployeeAttendanceStatusMasterList()
        {
            try
            {
                return await _employeeAttendanceStatusMasterRepository.GetEmployeeAttendanceStatusMasterList();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeAttendanceStatusMasterDTO>>(false, ex.Message, new List<EmployeeAttendanceStatusMasterDTO>(), 500);
            }
        }

        public async Task<ServiceResponse<EmployeeAttendanceStatusMasterDTO>> GetEmployeeAttendanceStatusMasterById(int Employee_Attendance_Status_id)
        {
            try
            {
                return await _employeeAttendanceStatusMasterRepository.GetEmployeeAttendanceStatusMasterById(Employee_Attendance_Status_id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeAttendanceStatusMasterDTO>(false, ex.Message, new EmployeeAttendanceStatusMasterDTO(), 500);
            }
        }

        public async Task<ServiceResponse<string>> AddEmployeeAttendanceStatusMaster(EmployeeAttendanceStatusMasterDTO request)
        {
            try
            {
                return await _employeeAttendanceStatusMasterRepository.AddEmployeeAttendanceStatusMaster(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<string>> UpdateEmployeeAttendanceStatusMaster(EmployeeAttendanceStatusMasterDTO request)
        {
            try
            {
                return await _employeeAttendanceStatusMasterRepository.UpdateEmployeeAttendanceStatusMaster(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteEmployeeAttendanceStatusMaster(int Employee_Attendance_Status_id)
        {
            try
            {
                return await _employeeAttendanceStatusMasterRepository.DeleteEmployeeAttendanceStatusMaster(Employee_Attendance_Status_id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }
    }
}
