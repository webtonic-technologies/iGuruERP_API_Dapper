using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Repository.Interfaces;
using StudentManagement_API.Services.Interfaces;

namespace StudentManagement_API.Services.Implementations
{
    public class CertificatesService : ICertificatesService
    {
        private readonly ICertificatesRepository _certificatesRepository;

        public CertificatesService(ICertificatesRepository certificatesRepository)
        {
            _certificatesRepository = certificatesRepository;
        }

        public async Task<ServiceResponse<int>> CreateCertificateTemplateAsync(CreateCertificateTemplateRequest request)
        {
            try
            {
                int id = await _certificatesRepository.CreateCertificateTemplateAsync(request);
                return new ServiceResponse<int>(true, "Certificate template created successfully.", id, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetCertificateTemplateResponse>>> GetCertificateTemplateAsync(GetCertificateTemplateRequest request)
        {
            try
            {
                var data = await _certificatesRepository.GetCertificateTemplateAsync(request);
                return new ServiceResponse<IEnumerable<GetCertificateTemplateResponse>>(
                    true,
                    "Certificate template retrieved successfully.",
                    data,
                    200,
                    data.Count()
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetCertificateTemplateResponse>>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }
    }
}
