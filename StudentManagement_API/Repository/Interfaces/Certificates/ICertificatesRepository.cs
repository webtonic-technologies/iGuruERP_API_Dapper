using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;

namespace StudentManagement_API.Repository.Interfaces
{
    public interface ICertificatesRepository
    {
        Task<int> CreateCertificateTemplateAsync(CreateCertificateTemplateRequest request);
        Task<IEnumerable<GetCertificateTemplateResponse>> GetCertificateTemplateAsync(GetCertificateTemplateRequest request);
        //Task<int> GenerateCertificateAsync(GenerateCertificateRequest request);
        Task<List<int>> GenerateCertificatesAsync(GenerateCertificateRequest request);
        Task<IEnumerable<GetStudentsResponse>> GetStudentsAsync(GetStudentsRequest request);
        Task<IEnumerable<GetCertificateReportResponse>> GetCertificateReportAsync(GetCertificateReportRequest request);
        Task<IEnumerable<GetCertificateReportExportResponse>> GetCertificateReportExportAsync(GetCertificateReportExportRequest request);

    }
}
