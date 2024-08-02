using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.Services.Interfaces;

namespace Student_API.Controllers
{
    [Route("iGuru/UpdateStudentDocument/[controller]")]
    [ApiController]
    public class DocumentConfigController : ControllerBase
    {
        private readonly IDocumentConfigService _documentConfigService; 
        public DocumentConfigController(IDocumentConfigService documentConfigService)
        {
            _documentConfigService = documentConfigService;
        }

        [HttpPost("AddUpdateDocument")]
        public async Task<IActionResult> AddUpdateStudentDocument(List<StudentDocumentConfigDTO> studentDocumentDto)
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

        [HttpGet("GetByIDStudentDocument/{documentConfigId}")]
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

        [HttpDelete("DeleteByIDStudentDocument/{studentDocumentId}")]
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

        [HttpPost("GetAllStudentDocumentList")]
        public async Task<IActionResult> GetAllStudentDocuments(GetCommonRequestPageListModel obj )
        {
            try
            {
                obj.sortField = obj.sortField?? "Document_Name";
                obj.sortDirection = obj.sortDirection ?? "ASC";
                var response = await _documentConfigService.GetAllStudentDocuments(obj.Institute_id,obj.sortField, obj.sortDirection, obj.pageSize, obj.pageNumber);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}