using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using Attendance_SE_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_SE_API.Services.Implementations
{
    public class MarkAttendanceService : IMarkAttendanceService
    {
        private readonly IMarkAttendanceRepository _repository;

        public MarkAttendanceService(IMarkAttendanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<List<AttendanceTypeResponse>>> GetAttendanceType()
        {
            return await _repository.GetAttendanceType();
        }

        public async Task<ServiceResponse<List<TimeSlotTypeResponse>>> GetTimeSlotType()
        {
            return await _repository.GetTimeSlotType();
        }

        public async Task<ServiceResponse<List<AttendanceSubjectsResponse>>> GetAttendanceSubjects(AttendanceSubjectsRequest request)
        {
            return await _repository.GetAttendanceSubjects(request);
        }
    }
}
