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
        Task<ServiceResponse<List<int>>> GenerateCertificatesAsync(GenerateCertificateRequest request);
        Task<ServiceResponse<IEnumerable<GetStudentsResponse>>> GetStudentsAsync(GetStudentsRequest request);
        Task<ServiceResponse<IEnumerable<GetCertificateReportResponse>>> GetCertificateReportAsync(GetCertificateReportRequest request);
        Task<ServiceResponse<string>> GetCertificateReportExportAsync(GetCertificateReportExportRequest request);

    }
}
