using Attendance_API.Models;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;
using Attendance_API.ServiceResponse;
using Attendance_API.DTOs.Requests;

namespace Attendance_API.Services.Implementations
{
    public class StudentAttendanceStatusService : IStudentAttendanceStatusService
    {
        private readonly IStudentAttendanceStatusRepository _repository;

        public StudentAttendanceStatusService(IStudentAttendanceStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<string>> AddUpdateAttendanceStatus(List<AttendanceStatus> attendanceStatuses)
        {
            foreach (var status in attendanceStatuses)
            {
                // Assuming there's a method in the repository to handle individual updates
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
