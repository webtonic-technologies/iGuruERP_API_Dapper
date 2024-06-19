using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Services.Interfaces
{
    public interface IStudentInformationServices
    {
        Task<ServiceResponse<int>> AddUpdateStudentInformation(StudentMasterDTO request);
        Task<ServiceResponse<StudentInformationDTO>> GetStudentDetailsById(int studentId);
        Task<ServiceResponse<List<StudentDetailsDTO>>> GetAllStudentDetails(int Institute_id, string sortField = "Student_Name", string sortDirection = "ASC", int? pageNumber = null, int? pageSize = null);
        Task<ServiceResponse<int>> ChangeStudentStatus(StudentStatusDTO statusDTO);
        Task<ServiceResponse<int>> AddUpdateStudentOtherInfo(StudentOtherInfoDTO request);
        Task<ServiceResponse<int>> AddUpdateStudentParentInfo(StudentParentInfoDTO request);
        Task<ServiceResponse<int>> AddOrUpdateStudentSiblings(StudentSiblings sibling);
        Task<ServiceResponse<int>> AddOrUpdateStudentPreviousSchool(StudentPreviousSchool previousSchool);
        Task<ServiceResponse<int>> AddOrUpdateStudentHealthInfo(StudentHealthInfo healthInfo);
        Task<ServiceResponse<int>> AddUpdateStudentDocuments(StudentDocumentsDTO request);
    }
}
