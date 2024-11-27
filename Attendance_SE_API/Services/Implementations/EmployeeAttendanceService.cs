using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.Models;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using Attendance_SE_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_SE_API.Services.Implementations
{
    public class EmployeeAttendanceService : IEmployeeAttendanceService
    {
        private readonly IEmployeeAttendanceRepository _repository;

        public EmployeeAttendanceService(IEmployeeAttendanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<bool>> SetAttendance(EmployeeSetAttendanceRequest request)
        {
            return await _repository.SetAttendance(request);
        }

        public async Task<ServiceResponse<List<EmployeeAttendanceResponse>>> GetAttendance_EMP(GetEmployeeAttendanceRequest request)
        {
            var response = await _repository.GetAttendance_EMP(request);
            return response; // Ensure this matches the expected type in the interface
        }

        public async Task<GetEmployeeDepartmentResponse> GetEmployeeDepartmentAsync(int instituteId)
        {
            var departments = await _repository.GetDepartmentsByInstituteAsync(instituteId);

            return new GetEmployeeDepartmentResponse
            {
                Departments = departments
            };
        }
    }
}
