using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Repository.Interfaces
{
    public interface IDocumentConfigRepository
    {
        Task<ServiceResponse<int>> AddUpdateStudentDocument(List<StudentDocumentConfig> studentDocumentDto);
        Task<ServiceResponse<StudentDocumentConfigDTO>> GetStudentDocumentConfigById(int DocumentConfigtId);
        Task<ServiceResponse<bool>> DeleteStudentDocument(int studentDocumentId);
        Task<ServiceResponse<List<StudentDocumentConfigDTO>>> GetAllStudentDocuments(int Institute_id, string sortColumn, string sortDirection, int? pageSize = null, int? pageNumber = null,string searchQuery = null);
    }
}
