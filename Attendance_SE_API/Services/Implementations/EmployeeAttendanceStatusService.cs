using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.ServiceResponse;
using Attendance_SE_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attendance_SE_API.Models;
using Attendance_SE_API.Repository.Implementations;
using Attendance_SE_API.Services.Interfaces;

namespace Attendance_SE_API.Services.Implementations
{
    public class EmployeeAttendanceStatusService : IEmployeeAttendanceStatusService
    {
        private readonly IEmployeeAttendanceStatusRepository _repository;

        public EmployeeAttendanceStatusService(IEmployeeAttendanceStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<string>> AddUpdateAttendanceStatus(List<AttendanceStatus> attendanceStatuses)
        {
            foreach (var status in attendanceStatuses)
            {
                var response = await _repository.AddUpdateAttendanceStatus(status);
                // Handle responses as needed
            }
            return new ServiceResponse<string>(true, "All statuses processed successfully.", null, 200);
        }

        public async Task<ServiceResponse<List<AttendanceStatus>>> GetAllAttendanceStatuses(GetAllAttendanceStatusRequest request)
        {
            return await _repository.GetAllAttendanceStatuses(request);
        }

        public async Task<ServiceResponse<AttendanceStatus>> GetAttendanceStatusById(int statusId)
        {
            return await _repository.GetAttendanceStatusById(statusId);
        }

        public async Task<ServiceResponse<bool>> DeleteStatus(int statusId)
        {
            return await _repository.DeleteStatus(statusId);
        }
    }
}
