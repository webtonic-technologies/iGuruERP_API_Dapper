using Microsoft.AspNetCore.Mvc;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.Services.Interfaces;

namespace StudentManagement_API.Controllers
{
    [Route("iGuru/StudentManagement/Certificates")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly ICertificatesService _certificatesService;

        public CertificatesController(ICertificatesService certificatesService)
        {
            _certificatesService = certificatesService;
        }

        [HttpPost("CreateCertificateTemplate")]
        public async Task<IActionResult> CreateCertificateTemplate([FromBody] CreateCertificateTemplateRequest request)
        {
            var response = await _certificatesService.CreateCertificateTemplateAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetCertificateTemplate")]
        public async Task<IActionResult> GetCertificateTemplate([FromBody] GetCertificateTemplateRequest request)
        {
            var response = await _certificatesService.GetCertificateTemplateAsync(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
