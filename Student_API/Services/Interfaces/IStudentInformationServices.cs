using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Services.Interfaces
{
    public interface IStudentInformationServices
    {
        Task<ServiceResponse<int>> AddUpdateStudent(StudentDTO request);
        Task<ServiceResponse<int>> AddUpdateStudentInformation(StudentMasters request);
        Task<ServiceResponse<StudentInformationDTO>> GetStudentDetailsById(int studentId);
        Task<ServiceResponse<List<StudentDetailsDTO>>> GetAllStudentDetails(int Institute_id, string sortField = "Student_Name", string sortDirection = "ASC", int? pageNumber = null, int? pageSize = null);
        Task<ServiceResponse<int>> ChangeStudentStatus(StudentStatusDTO statusDTO);
        Task<ServiceResponse<int>> AddUpdateStudentOtherInfo(StudentOtherInfos request);
        Task<ServiceResponse<int>> AddUpdateStudentParentInfo(StudentParentInfo request);
        Task<ServiceResponse<int>> AddOrUpdateStudentSiblings(StudentSibling sibling);
        Task<ServiceResponse<int>> AddOrUpdateStudentPreviousSchool(StudentPreviousSchools previousSchool);
        Task<ServiceResponse<int>> AddOrUpdateStudentHealthInfo(StudentHealthInfos healthInfo);
        Task<ServiceResponse<int>> AddUpdateStudentDocuments(StudentDocumentsDTO request);
        Task<ServiceResponse<int>> DeleteStudentDocument(int Student_Documents_id);
    }
}
