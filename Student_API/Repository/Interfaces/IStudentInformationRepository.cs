
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Repository.Interfaces
{
    public interface IStudentInformationRepository
    {
        Task<ServiceResponse<StudentInformationDTO>> GetStudentDetailsById(int studentId);
        Task<ServiceResponse<int>> AddUpdateStudentInformation(StudentMasters request);
        Task<ServiceResponse<int>> AddUpdateStudentOtherInfo(StudentOtherInfos request);
        Task<ServiceResponse<int>> AddUpdateStudentParentInfo(StudentParentInfo request);
        Task<ServiceResponse<int>> AddOrUpdateStudentSiblings(StudentSibling sibling);
        Task<ServiceResponse<int>> AddOrUpdateStudentPreviousSchool(StudentPreviousSchools previousSchool);
        Task<ServiceResponse<int>> AddOrUpdateStudentHealthInfo(StudentHealthInfos healthInfo);
        Task<ServiceResponse<List<StudentDetailsDTO>>> GetAllStudentDetails(int Institute_id,  string sortField , string sortDirection , int? pageNumber = null, int? pageSize = null);
        Task<ServiceResponse<int>> ChangeStudentStatus(StudentStatusDTO statusDTO);
        Task<ServiceResponse<int>> AddUpdateStudentDocuments(StudentDocumentListDTO request, int Student_id);
        Task<ServiceResponse<string>> GetStudentInfoImageById(int studentId);
        Task<ServiceResponse<string>> GetStudentparentImageById(int Student_Parent_Info_id);
        Task<ServiceResponse<int>> DeleteStudentDocument(int Student_Documents_id);
    }
}
