using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.Services.Interfaces;
using System.Net.Mime;

namespace Student_API.Controllers
{
    [Route("iGuru/Approvals/[controller]")]
    [ApiController]
    public class PermissionSlipController : ControllerBase
    {
        private readonly IPermissionSlipService _permissionSlipService;
       

        public PermissionSlipController(IPermissionSlipService permissionSlipService )
        {
            _permissionSlipService = permissionSlipService;
          
        }

        [HttpPost("GetAllPermissionSlips")]
        public async Task<IActionResult> GetAllPermissionSlips(GetAllPermissionSlips obj)
        {
            var response = await _permissionSlipService.GetAllPermissionSlips(obj.Institute_id, obj.classId, obj.sectionId, obj.pageNumber, obj.pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("UpdatePermissionSlipStatus")]
        public async Task<IActionResult> UpdatePermissionSlipStatus([FromBody] UpdatePermissionSlipStatusRequest request)
        {
            var response = await _permissionSlipService.UpdatePermissionSlipStatus(request.PermissionSlipId, request.IsApproved);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("GetAllStudentApprovedList")]
        public async Task<IActionResult> GetAllStudentApprovedList(GetAllPermissionSlipsByStatus obj)
        {
            var response = await _permissionSlipService.GetPermissionSlips(obj.Institute_id, obj.classId, obj.sectionId, obj.startDate, obj.endDate, true, obj.pageNumber, obj.pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("GetAllStudentRejectedList")]
        public async Task<IActionResult> GetAllStudentRejectedList(GetAllPermissionSlipsByStatus obj)
        {
            var response = await _permissionSlipService.GetPermissionSlips(obj.Institute_id, obj.classId, obj.sectionId, obj.startDate, obj.endDate, false, obj.pageNumber, obj.pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("AddPermissionSlip")]
        public async Task<IActionResult> AddPermissionSlip(PermissionSlip obj)
        {
            var response = await _permissionSlipService.AddPermissionSlip(obj);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetPermissionSlipById")]
        public async Task<IActionResult> GetPermissionSlipById(GetSinglePermissionSlip obj)
        {
            var response = await _permissionSlipService.GetPermissionSlipById(obj.permissionSlipId , obj.Institute_id);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("ExportPermissionSlipsToExcel")]
        public async Task<IActionResult> ExportPermissionSlipsToExcel(GetAllPermissionSlip obj)
        {
            var response = await _permissionSlipService.ExportPermissionSlipsToExcel(obj.Institute_id, obj.classId, obj.sectionId , obj.exportFormat);

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

        [HttpPost("ExportStudentApprovedToExcel")]
        public async Task<IActionResult> ExportStudentApprovedToExcel(GetAllPermissionSlipsByStatusExport obj)
        {
            var response = await _permissionSlipService.ExportPermissionSlipsToExcel(obj.Institute_id, obj.classId, obj.sectionId, obj.startDate, obj.endDate, true, obj.exportFormat);
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
        [HttpPost("ExportStudentRejectedToExcel")]
        public async Task<IActionResult> ExportStudentRejectedToExcel(GetAllPermissionSlipsByStatusExport obj)
        {
            var response = await _permissionSlipService.ExportPermissionSlipsToExcel(obj.Institute_id, obj.classId, obj.sectionId, obj.startDate, obj.endDate, false, obj.exportFormat);
            if (response.Success)
            {
                var filePath = response.Data;
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                // Return the Excel file for download
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(filePath));
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

    }
}
