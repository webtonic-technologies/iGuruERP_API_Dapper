using Microsoft.AspNetCore.Mvc;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.Services.Interfaces;

namespace StudentManagement_API.Controllers
{
    [Route("iGuru/Approvals/PermissionSlip")]
    [ApiController]
    public class ApprovalsController : ControllerBase
    {
        private readonly IApprovalsService _approvalsService;

        public ApprovalsController(IApprovalsService approvalsService)
        {
            _approvalsService = approvalsService;
        }

        [HttpPost("CreatePermissionSlip")]
        public async Task<IActionResult> CreatePermissionSlip([FromBody] CreatePermissionSlipRequest request)
        {
            var response = await _approvalsService.CreatePermissionSlipAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetPermissionSlip")]
        public async Task<IActionResult> GetPermissionSlip([FromBody] GetPermissionSlipRequest request)
        {
            var response = await _approvalsService.GetPermissionSlipAsync(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("ChangePermissionSlipStatus")]
        public async Task<IActionResult> ChangePermissionSlipStatus([FromBody] ChangePermissionSlipStatusRequest request)
        {
            var response = await _approvalsService.ChangePermissionSlipStatusAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetApprovedHistory")]
        public async Task<IActionResult> GetApprovedHistory([FromBody] GetApprovedHistoryRequest request)
        {
            var response = await _approvalsService.GetApprovedHistoryAsync(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("GetRejectedHistory")]
        public async Task<IActionResult> GetRejectedHistory([FromBody] GetRejectedHistoryRequest request)
        {
            var response = await _approvalsService.GetRejectedHistoryAsync(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("GetPermissionSlipExport")]
        public async Task<IActionResult> GetPermissionSlipExport([FromBody] GetPermissionSlipExportRequest request)
        {
            var response = await _approvalsService.GetPermissionSlipExportAsync(request);
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
                            "PermissionSlipExport.xlsx");
            }
            else if (request.ExportType == 2)
            {
                return File(fileBytes,
                            "text/csv",
                            "PermissionSlipExport.csv");
            }
            else
            {
                return BadRequest("Invalid ExportType.");
            }
        }

        [HttpPost("GetApprovedHistoryExport")]
        public async Task<IActionResult> GetApprovedHistoryExport([FromBody] GetApprovedHistoryExportRequest request)
        {
            var response = await _approvalsService.GetApprovedHistoryExportAsync(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            string filePath = response.Data;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Export file not found.");
            }

            // Read file bytes from disk
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            if (request.ExportType == 1)
            {
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ApprovedHistoryExport.xlsx");
            }
            else if (request.ExportType == 2)
            {
                return File(fileBytes, "text/csv", "ApprovedHistoryExport.csv");
            }
            else
            {
                return BadRequest("Invalid ExportType.");
            }
        }


        [HttpPost("GetRejectedHistoryExport")]
        public async Task<IActionResult> GetRejectedHistoryExport([FromBody] GetRejectedHistoryExportRequest request)
        {
            var response = await _approvalsService.GetRejectedHistoryExportAsync(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            string filePath = response.Data;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Export file not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            if (request.ExportType == 1)
            {
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RejectedHistoryExport.xlsx");
            }
            else if (request.ExportType == 2)
            {
                return File(fileBytes, "text/csv", "RejectedHistoryExport.csv");
            }
            else
            {
                return BadRequest("Invalid ExportType.");
            }
        }
    }
}
