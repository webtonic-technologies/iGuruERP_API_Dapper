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

        [HttpPost("ExportStudentDocuments")]
        public async Task<IActionResult> ExportStudentDocuments(ExportStudentDocumentRequestModel obj)
        {
           
            var response = await _documentManagerService.ExportStudentDocuments(obj.Institute_id, obj.classId, obj.sectionId, "Student_Name", "ASC", int.MaxValue,1, obj.exportFormat);
            if (response.Success)
            {
                var filePath = response.Data;
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                string contentType;
                string fileExtension = Path.GetExtension(filePath).ToLower();

                // Determine the content type based on file extension (Excel, CSV, or PDF)
                switch (fileExtension)
                {
                    case ".xlsx": // Excel
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                    case ".csv": // CSV
                        contentType = "text/csv";
                        break;
                    case ".pdf": // PDF
                        contentType = "application/pdf";
                        break;
                    default:
                        return BadRequest("Unsupported file format.");
                }

                // Return the file for download with the appropriate content type
                return File(fileBytes, contentType, Path.GetFileName(filePath));
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
    }
}
