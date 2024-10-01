using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;
using Attendance_API.DTOs.Requests;
using Attendance_API.DTOs.Response;
using Attendance_API.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_API.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IStudentAttendanceRepository _repository;

        public AttendanceService(IStudentAttendanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<List<StudentAttendanceResponse>>> GetAttendance(GetAttendanceRequest request)
        {
            var attendance = await _repository.GetAttendance(request);
            return new ServiceResponse<List<StudentAttendanceResponse>>(true, "Attendance fetched successfully.", attendance, 200);
        }

        public async Task<ServiceResponse<bool>> SetAttendance(SetAttendanceRequest request)
        {
            var result = await _repository.SetAttendance(request);
            return new ServiceResponse<bool>(result, result ? "Attendance marked successfully." : "Failed to mark attendance.", result, 200);
        }
    }
}
