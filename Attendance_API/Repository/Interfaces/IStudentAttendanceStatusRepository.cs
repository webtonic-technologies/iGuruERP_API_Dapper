using System.Threading.Tasks;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Repository.Interfaces
{
    public interface IStudentAttendanceStatusRepository
    {
        Task<ServiceResponse<List<StudentAttendanceStatusDTO>>> GetStudentAttendanceStatusList(int InstituteId);
        Task<ServiceResponse<StudentAttendanceStatusDTO>> GetStudentAttendanceStatusById(int Student_Attendance_Status_id);
        Task<ServiceResponse<string>> SaveStudentAttendanceStatus(List<StudentAttendanceStatusDTO> request);
        //Task<ServiceResponse<string>> UpdateStudentAttendanceStatus(StudentAttendanceStatusDTO request);
        Task<ServiceResponse<string>> DeleteStudentAttendanceStatus(int Student_Attendance_Status_id);
    }
}
