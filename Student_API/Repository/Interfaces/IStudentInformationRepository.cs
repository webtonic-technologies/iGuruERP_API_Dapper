
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Repository.Interfaces
{
    public interface IStudentInformationRepository
    {
        Task<ServiceResponse<StudentMasterDTO>> GetStudentDetailsById(int studentId);
        Task<ServiceResponse<int>> AddUpdateStudentInformation(StudentMasterDTO request);
        Task<int> AddUpdateStudentOtherInfo(StudentOtherInfoDTO request, int student_id);
        Task<int> AddUpdateStudentParentInfo(StudentParentInfoDTO request, int student_id);
        Task<int> AddOrUpdateStudentSiblings(StudentSiblings sibling, int student_id);
        Task<int> AddOrUpdateStudentPreviousSchool(StudentPreviousSchool previousSchool, int student_id);
        Task<int> AddOrUpdateStudentHealthInfo(StudentHealthInfo healthInfo, int student_id);
        Task<ServiceResponse<List<StudentDetailsDTO>>> GetAllStudentDetails();
        Task<ServiceResponse<int>> ChangeStudentStatus(StudentStatusDTO statusDTO);
    }
}
