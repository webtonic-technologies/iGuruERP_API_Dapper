using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;

namespace StudentManagement_API.Repository.Interfaces
{
    public interface ICertificatesRepository
    {
        Task<int> CreateCertificateTemplateAsync(CreateCertificateTemplateRequest request);
        Task<IEnumerable<GetCertificateTemplateResponse>> GetCertificateTemplateAsync(GetCertificateTemplateRequest request);

    }
}
