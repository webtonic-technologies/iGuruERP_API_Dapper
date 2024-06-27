using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Attendance_API.Services.Implementations
{
    public class EmployeeAttendanceService : IEmployeeAttendanceService
    {
        private readonly IEmployeeAttendanceRepository _employeeAttendanceRepository;

        public EmployeeAttendanceService(IEmployeeAttendanceRepository employeeAttendanceRepository)
        {
            _employeeAttendanceRepository = employeeAttendanceRepository;
        }

        public async Task<ServiceResponse<EmployeeAttendanceMasterResponseDTO>> GetEmployeeAttendanceMasterList(EmployeeAttendanceMasterRequestDTO request)
        {
            return await _employeeAttendanceRepository.GetEmployeeAttendanceMasterList(request);
        }

        public async Task<ServiceResponse<EmployeeAttendanceMasterDTO>> InsertOrUpdateEmployeeAttendanceMaster(EmployeeAttendanceMasterDTO employeeAttendanceMaster)
        {
            return await _employeeAttendanceRepository.InsertOrUpdateEmployeeAttendanceMaster(employeeAttendanceMaster);
        }

        public async Task<ServiceResponse<bool>> DeleteEmployeeAttendanceMaster(int employeeAttendanceId)
        {
            return await _employeeAttendanceRepository.DeleteEmployeeAttendanceMaster(employeeAttendanceId);
        }
    }
}
