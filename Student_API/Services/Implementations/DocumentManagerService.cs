using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;

namespace Student_API.Services.Implementations
{
    public class DocumentManagerService : IDocumentManagerService
    {
        private readonly IDocumentManagerRepository _documentManagerRepository;

        public DocumentManagerService(IDocumentManagerRepository documentManagerRepository)
        {
            _documentManagerRepository = documentManagerRepository;
        }

        public async Task<ServiceResponse<List<StudentDocumentInfo>>> GetStudentDocuments(int Institute_id, int classId, int sectionId, string sortColumn, string sortDirection, int? pageSize, int? pageNumber)
        {
            return await _documentManagerRepository.GetStudentDocuments(Institute_id,classId, sectionId,sortColumn ,sortDirection,pageSize, pageNumber);
        }

        public async Task<ServiceResponse<bool>> UpdateStudentDocumentStatuses(List<DocumentUpdateRequest> updates)
        {
            return await _documentManagerRepository.UpdateStudentDocumentStatuses(updates);
        }
    }
}
