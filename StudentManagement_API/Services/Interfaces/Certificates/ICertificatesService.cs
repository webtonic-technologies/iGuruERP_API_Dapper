using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;

namespace StudentManagement_API.Services.Interfaces
{
    public interface ICertificatesService
    {
        Task<ServiceResponse<int>> CreateCertificateTemplateAsync(CreateCertificateTemplateRequest request);
        Task<ServiceResponse<IEnumerable<GetCertificateTemplateResponse>>> GetCertificateTemplateAsync(GetCertificateTemplateRequest request);
        //Task<ServiceResponse<int>> GenerateCertificateAsync(GenerateCertificateRequest request);
        //Task<ServiceResponse<List<int>>> GenerateCertificatesAsync(GenerateCertificateRequest request);
        Task<ServiceResponse<GenerateCertificateResponse>> GenerateCertificatesAsync(GenerateCertificateRequest request); 
        Task<ServiceResponse<GetCertificateTemplateByIDResponse>> GetCertificateTemplateByIDAsync(int templateId); 
        Task<ServiceResponse<IEnumerable<GetStudentsResponse>>> GetStudentsAsync(GetStudentsRequest request);
        Task<ServiceResponse<IEnumerable<GetCertificateReportResponse>>> GetCertificateReportAsync(GetCertificateReportRequest request);
        Task<ServiceResponse<string>> GetCertificateReportExportAsync(GetCertificateReportExportRequest request);
        Task<ServiceResponse<IEnumerable<GetCertificateInstituteTagsResponse>>> GetCertificateInstituteTagsAsync();
        Task<ServiceResponse<Dictionary<string, List<GetCertificateStudentTagsResponse>>>> GetCertificateStudentTagsAsync();
        Task<ServiceResponse<GetCertificateTagValueResponse>> GetCertificateTagValue(GetCertificateTagValueRequest request);
        //Task<ServiceResponse<int>> AttachCertificatewithStudent(AttachCertificatewithStudentRequest request);
        Task<ServiceResponse<int>> AttachCertificatewithStudent(AttachCertificateWithStudentsRequest request);
        Task<ServiceResponse<List<GetCertificateTemplatesListResponse>>> GetCertificateTemplatesList(GetCertificateTemplatesListRequest request);
        Task<ServiceResponse<int>> UpdateCertificateTemplate(UpdateCertificateTemplateRequest request);
        Task<ServiceResponse<int>> DeleteCertificateTemplate(DeleteCertificateTemplateRequest request);

    }
}
