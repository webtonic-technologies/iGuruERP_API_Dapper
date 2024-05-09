using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Repository.Interfaces
{
    public interface IStudentAttendanceMasterRepository
    {
        Task<ServiceResponse<List<StudentAttendanceMasterResponseDTO>>> GetStudentAttendanceMasterList(StudentAttendanceMasterRequestDTO request);
        Task<ServiceResponse<StudentAttendanceMasterDTO>> InsertOrUpdateStudentAttendanceMaster(StudentAttendanceMasterDTO studentAttendanceMaster);
        Task<ServiceResponse<bool>> DeleteStudentAttendanceMaster(int studentAttendanceId);
    }
}
