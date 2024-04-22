using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Services.Interfaces
{
    public interface IStudentInformationServices
    {
        Task<ServiceResponse<int>> AddUpdateStudentInformation(StudentMasterDTO request);
        Task<ServiceResponse<StudentMasterDTO>> GetStudentDetailsById(int studentId);
        Task<ServiceResponse<List<StudentDetailsDTO>>> GetAllStudentDetails();
        Task<ServiceResponse<int>> ChangeStudentStatus(StudentStatusDTO statusDTO);
    }
}
