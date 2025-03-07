using Microsoft.AspNetCore.Mvc;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;
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


        [HttpPost("GenerateCertificate")]
        public async Task<IActionResult> GenerateCertificate([FromBody] GenerateCertificateRequest request)
        {
            var response = await _certificatesService.GenerateCertificatesAsync(request);
            return StatusCode(response.StatusCode, response);
        }


        //[HttpPost("GenerateCertificate")]
        //public async Task<IActionResult> GenerateCertificate([FromBody] GenerateCertificateRequest request)
        //{
        //    var response = await _certificatesService.GenerateCertificatesAsync(request);
        //    return StatusCode(response.StatusCode, response);
        //}


        [HttpPost("GetStudents")]
        public async Task<IActionResult> GetStudents([FromBody] GetStudentsRequest request)
        {
            var response = await _certificatesService.GetStudentsAsync(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("GetCertificateReport")]
        public async Task<IActionResult> GetCertificateReport([FromBody] GetCertificateReportRequest request)
        {
            var response = await _certificatesService.GetCertificateReportAsync(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("GetCertificateReportExport")]
        public async Task<IActionResult> GetCertificateReportExport([FromBody] GetCertificateReportExportRequest request)
        {
            var response = await _certificatesService.GetCertificateReportExportAsync(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            string filePath = response.Data;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Export file not found.");
            }

            // Read file bytes
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            // Return file based on ExportType
            if (request.ExportType == 1)
            {
                return File(fileBytes,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "CertificateReportExport.xlsx");
            }
            else if (request.ExportType == 2)
            {
                return File(fileBytes,
                            "text/csv",
                            "CertificateReportExport.csv");
            }
            else
            {
                return BadRequest("Invalid ExportType.");
            }
        }


        [HttpPost("GetCertificateInstituteTags")]
        public async Task<IActionResult> GetCertificateInstituteTags()
        {
            var response = await _certificatesService.GetCertificateInstituteTagsAsync();
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("GetCertificateStudentTags")]
        public async Task<IActionResult> GetCertificateStudentTags()
        {
            var response = await _certificatesService.GetCertificateStudentTagsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetCertificateTagValue")]
        public async Task<ActionResult<ServiceResponse<GetCertificateTagValueResponse>>> GetCertificateTagValue([FromBody] GetCertificateTagValueRequest request)
        {
            var response = await _certificatesService.GetCertificateTagValue(request);
            return StatusCode(response.StatusCode, response);
        }


        //[HttpPost("AttachCertificatewithStudent")]
        //public async Task<ActionResult<ServiceResponse<int>>> AttachCertificatewithStudent([FromBody] AttachCertificatewithStudentRequest request)
        //{
        //    var response = await _certificatesService.AttachCertificatewithStudent(request);
        //    return StatusCode(response.StatusCode, response);
        //} 

        [HttpPost("AttachCertificatewithStudent")]
        public async Task<ActionResult<ServiceResponse<int>>> AttachCertificatewithStudent([FromBody] AttachCertificateWithStudentsRequest request)
        {
            var response = await _certificatesService.AttachCertificatewithStudent(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("GetCertificateTemplatesList")]
        public async Task<IActionResult> GetCertificateTemplatesList([FromBody] GetCertificateTemplatesListRequest request)
        {
            var response = await _certificatesService.GetCertificateTemplatesList(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("UpdateCertificateTemplate")]
        public async Task<IActionResult> UpdateCertificateTemplate([FromBody] UpdateCertificateTemplateRequest request)
        {
            var response = await _certificatesService.UpdateCertificateTemplate(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpDelete("DeleteCertificateTemplate")]
        public async Task<IActionResult> DeleteCertificateTemplate([FromBody] DeleteCertificateTemplateRequest request)
        {
            var response = await _certificatesService.DeleteCertificateTemplate(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
