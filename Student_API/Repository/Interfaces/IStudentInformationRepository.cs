
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Repository.Interfaces
{
    public interface IStudentInformationRepository
    {
        Task<ServiceResponse<StudentInformationDTO>> GetStudentDetailsById(int studentId);
        Task<ServiceResponse<int>> AddUpdateStudentInformation(StudentMasterDTO request);
        Task<ServiceResponse<int>> AddUpdateStudentOtherInfo(StudentOtherInfoDTO request);
        Task<ServiceResponse<int>> AddUpdateStudentParentInfo(StudentParentInfoDTO request);
        Task<ServiceResponse<int>> AddOrUpdateStudentSiblings(StudentSiblings sibling);
        Task<ServiceResponse<int>> AddOrUpdateStudentPreviousSchool(StudentPreviousSchool previousSchool);
        Task<ServiceResponse<int>> AddOrUpdateStudentHealthInfo(StudentHealthInfo healthInfo);
        Task<ServiceResponse<List<StudentDetailsDTO>>> GetAllStudentDetails(int Institute_id,  string sortField , string sortDirection , int? pageNumber = null, int? pageSize = null);
        Task<ServiceResponse<int>> ChangeStudentStatus(StudentStatusDTO statusDTO);
        Task<ServiceResponse<int>> AddUpdateStudentDocuments(StudentDocumentListDTO request, int Student_id);
        Task<ServiceResponse<string>> GetStudentInfoImageById(int studentId);
        Task<ServiceResponse<string>> GetStudentparentImageById(int Student_Parent_Info_id);
        Task<ServiceResponse<int>> DeleteStudentDocument(int Student_Documents_id);
    }
}
