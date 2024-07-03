using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.Services.Interfaces;

namespace Student_API.Controllers
{
    [Route("iGuru/UpdateStudentDocument/[controller]")]
    [ApiController]
    public class DocumentManagerController : ControllerBase
    {
        private readonly IDocumentManagerService _documentManagerService;
        public DocumentManagerController(IDocumentManagerService documentManagerService)
        {
            _documentManagerService = documentManagerService;
        }
        [HttpPost("GetAllStudentDocumentsList")]
        public async Task<IActionResult> GetStudentDocuments(GetStudentDocumentRequestModel obj)
        {
            obj.sortField = obj.sortField ?? "Student_Name";
            obj.sortDirection = obj.sortDirection ?? "ASC";
            var response = await _documentManagerService.GetStudentDocuments(obj.Institute_id,obj.classId, obj.sectionId, obj.sortField, obj.sortDirection, obj.pageSize, obj.pageNumber);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("ApprovedStudentDocument")]
        public async Task<IActionResult> ApprovedStudentDocument([FromBody] List<DocumentUpdateRequest> updates)
        {
            var response = await _documentManagerService.UpdateStudentDocumentStatuses(updates);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
    }
}
