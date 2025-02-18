using StudentManagement_API.DTOs.Requests; 
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;

namespace StudentManagement_API.Services.Interfaces
{
    public interface IUpdateStudentDocumentService
    { 
        Task<ServiceResponse<List<int>>> AddUpdateDocumentAsync(AddUpdateDocumentRequest request);
        Task<ServiceResponse<IEnumerable<GetDocumentsResponse>>> GetDocumentsAsync(GetDocumentsRequest request);
        Task<ServiceResponse<bool>> DeleteDocumentAsync(DeleteDocumentRequest request);
        Task<ServiceResponse<bool>> SetDocumentManagerAsync(List<SetDocumentManagerRequest> requests);
        Task<ServiceResponse<IEnumerable<GetDocumentManagerResponse>>> GetDocumentManagerAsync(GetDocumentManagerRequest request);
        Task<ServiceResponse<string>> GetDocumentManagerExportAsync(GetDocumentManagerExportRequest request);

    }
}
