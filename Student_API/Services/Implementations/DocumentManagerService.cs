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

        public async Task<ServiceResponse<List<StudentDocumentInfo>>> GetStudentDocuments(int classId, int sectionId, int? pageSize, int? pageNumber)
        {
            return await _documentManagerRepository.GetStudentDocuments(classId, sectionId, pageSize, pageNumber);
        }

        public async Task<ServiceResponse<bool>> UpdateStudentDocumentStatuses(List<DocumentUpdateRequest> updates)
        {
            return await _documentManagerRepository.UpdateStudentDocumentStatuses(updates);
        }
    }
}
