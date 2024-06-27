using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Services.Interfaces
{
    public interface IDocumentManagerService
    {
        Task<ServiceResponse<List<StudentDocumentInfo>>> GetStudentDocuments(int classId, int sectionId, string sortColumn, string sortDirection, int? pageSize, int? pageNumber);
        Task<ServiceResponse<bool>> UpdateStudentDocumentStatuses(List<DocumentUpdateRequest> updates);

    }
}
