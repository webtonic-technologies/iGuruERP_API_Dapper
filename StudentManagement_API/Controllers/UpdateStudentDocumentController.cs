using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using StudentManagement_API.DTOs.Requests; 
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Services.Interfaces;
using System.Globalization;

namespace StudentManagement_API.Controllers
{
    [Route("iGuru/UpdateStudentDocument/Configuration")]
    [ApiController]
    public class UpdateStudentDocumentController : ControllerBase
    {
        private readonly IUpdateStudentDocumentService _updateStudentDocumentService;

        public UpdateStudentDocumentController(IUpdateStudentDocumentService updateStudentDocumentService)
        {
            _updateStudentDocumentService = updateStudentDocumentService;
        }
          
        [HttpPost("AddUpdateDocument")]
        public async Task<IActionResult> AddUpdateDocument([FromBody] AddUpdateDocumentRequest request)
        {
            var response = await _updateStudentDocumentService.AddUpdateDocumentAsync(request);
            return StatusCode(response.StatusCode, response);
        }
         
        [HttpPost("GetDocuments")]
        public async Task<IActionResult> GetDocuments([FromBody] GetDocumentsRequest request)
        {
            var response = await _updateStudentDocumentService.GetDocumentsAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("DeleteDocument")]
        public async Task<IActionResult> DeleteDocument([FromBody] DeleteDocumentRequest request)
        {
            var response = await _updateStudentDocumentService.DeleteDocumentAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("DocManager/SetDocumentManager")]
        public async Task<IActionResult> SetDocumentManager([FromBody] List<SetDocumentManagerRequest> requests)
        {
            var response = await _updateStudentDocumentService.SetDocumentManagerAsync(requests);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("DocManager/GetDocumentManager")]
        public async Task<IActionResult> GetDocumentManager([FromBody] GetDocumentManagerRequest request)
        {
            var response = await _updateStudentDocumentService.GetDocumentManagerAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("DocManager/GetDocumentManagerExport")]
        public async Task<IActionResult> GetDocumentManagerExport([FromBody] GetDocumentManagerExportRequest request)
        {
            var response = await _updateStudentDocumentService.GetDocumentManagerExportAsync(request);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            string filePath = response.Data;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            // Read file bytes
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            // Return as Excel or CSV based on ExportType
            if (request.ExportType == 1)
            {
                return File(fileBytes,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "DocumentManagerExport.xlsx");
            }
            else if (request.ExportType == 2)
            {
                return File(fileBytes,
                            "text/csv",
                            "DocumentManagerExport.csv");
            }
            else
            {
                return BadRequest("Invalid ExportType.");
            }
        }

    }
}
