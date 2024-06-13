using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Repository.Interfaces
{
    public interface IDocumentConfigRepository
    {
        Task<ServiceResponse<int>> AddUpdateStudentDocument(StudentDocumentConfigDTO studentDocumentDto);
        Task<ServiceResponse<StudentDocumentConfigDTO>> GetStudentDocumentConfigById(int DocumentConfigtId);
        Task<ServiceResponse<bool>> DeleteStudentDocument(int studentDocumentId);
        Task<ServiceResponse<List<StudentDocumentConfigDTO>>> GetAllStudentDocuments(int? pageSize = null, int? pageNumber = null);
    }
}
