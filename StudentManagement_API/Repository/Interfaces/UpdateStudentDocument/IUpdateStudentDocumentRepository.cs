using StudentManagement_API.DTOs.Requests; 
using StudentManagement_API.DTOs.Responses;

namespace StudentManagement_API.Repository.Interfaces
{
    public interface IUpdateStudentDocumentRepository
    { 
        Task<List<int>> AddDocumentAsync(AddUpdateDocumentRequest request); 
        Task<IEnumerable<GetDocumentsResponse>> GetDocumentsAsync(GetDocumentsRequest request);
        Task<bool> DeleteDocumentAsync(DeleteDocumentRequest request);
        Task<bool> SetDocumentManagerAsync(List<SetDocumentManagerRequest> requests);
        Task<IEnumerable<GetDocumentManagerResponse>> GetDocumentManagerAsync(GetDocumentManagerRequest request); 
        Task<IEnumerable<GetDocumentManagerExportResponse>> GetDocumentManagerExportAsync(GetDocumentManagerExportRequest request);

    }
}
