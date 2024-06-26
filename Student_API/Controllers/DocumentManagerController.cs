﻿using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs;
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
        [HttpGet("GetAllStudentDocumentsList")]
        public async Task<IActionResult> GetStudentDocuments(int classId, int sectionId, string sortColumn = "Student_Name", string sortDirection = "ASC", int? pageSize = null, int? pageNumber = null)
        {
            var response = await _documentManagerService.GetStudentDocuments(classId, sectionId, sortColumn, sortDirection, pageSize, pageNumber);
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
