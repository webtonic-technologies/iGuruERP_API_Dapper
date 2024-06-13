using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs;
using Student_API.Services.Interfaces;

namespace Student_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class DocumentConfigController : ControllerBase
    {
        private readonly IDocumentConfigService _documentConfigService; 
        public DocumentConfigController(IDocumentConfigService documentConfigService)
        {
            _documentConfigService = documentConfigService;
        }

        [HttpPost]
        public async Task<IActionResult> AddUpdateStudentDocument(StudentDocumentConfigDTO studentDocumentDto)
        {
            try
            {
                var response = await _documentConfigService.AddUpdateStudentDocument(studentDocumentDto);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{documentConfigId}")]
        public async Task<IActionResult> GetStudentDocumentConfigById(int documentConfigId)
        {
            try
            {
                var response = await _documentConfigService.GetStudentDocumentConfigById(documentConfigId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{studentDocumentId}")]
        public async Task<IActionResult> DeleteStudentDocument(int studentDocumentId)
        {
            try
            {
                var response = await _documentConfigService.DeleteStudentDocument(studentDocumentId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudentDocuments([FromQuery] int? pageSize = null, [FromQuery] int? pageNumber = null)
        {
            try
            {
                var response = await _documentConfigService.GetAllStudentDocuments(pageSize, pageNumber);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
