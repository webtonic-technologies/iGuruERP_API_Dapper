using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;

namespace Attendance_API.Services.Implementations
{
    public class StudentAttendanceMasterService : IStudentAttendanceMasterService
    {
        private readonly IStudentAttendanceMasterRepository _studentAttendanceMasterRepository;

        public StudentAttendanceMasterService(IStudentAttendanceMasterRepository studentAttendanceMasterRepository)
        {
            _studentAttendanceMasterRepository = studentAttendanceMasterRepository;
        }

        public async Task<ServiceResponse<StudentAttendanceMasterResponseDTO>> GetStudentAttendanceMasterList(StudentAttendanceMasterRequestDTO request)
        {
            return await _studentAttendanceMasterRepository.GetStudentAttendanceMasterList(request);
        }

        public async Task<ServiceResponse<StudentAttendanceMasterDTO>> InsertOrUpdateStudentAttendanceMaster(StudentAttendanceMasterDTO studentAttendanceMaster)
        {
            return await _studentAttendanceMasterRepository.InsertOrUpdateStudentAttendanceMaster(studentAttendanceMaster);
        }

        public async Task<ServiceResponse<bool>> DeleteStudentAttendanceMaster(int studentAttendanceId)
        {
            return await _studentAttendanceMasterRepository.DeleteStudentAttendanceMaster(studentAttendanceId);
        }

        public async Task<ServiceResponse<IEnumerable<TimeSlotDTO>>> GetTimeSlotsForDropdown()
        {
            return await _studentAttendanceMasterRepository.GetTimeSlotsForDropdown();
        }
    }
}
