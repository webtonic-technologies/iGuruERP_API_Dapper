using Microsoft.AspNetCore.Mvc;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.Services.Interfaces;
using System.Threading.Tasks;

namespace StudentManagement_API.Controllers
{
    [Route("iGuru/StudentManagement/StudentLogins")]
    [ApiController]
    public class StudentLoginsController : ControllerBase
    {
        private readonly IStudentLoginsService _studentLoginsService;

        public StudentLoginsController(IStudentLoginsService studentLoginsService)
        {
            _studentLoginsService = studentLoginsService;
        }

        [HttpPost("GetLoginCredentials")]
        public async Task<IActionResult> GetLoginCredentials([FromBody] GetLoginCredentialsRequest request)
        {
            var response = await _studentLoginsService.GetLoginCredentialsAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetLoginCredentialsExport")]
        public async Task<IActionResult> GetLoginCredentialsExport([FromBody] GetLoginCredentialsExportRequest request)
        {
            var response = await _studentLoginsService.GetLoginCredentialsExportAsync(request);
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
                return File(fileBytes,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "LoginCredentialsExport.xlsx");
            }
            else if (request.ExportType == 2)
            {
                return File(fileBytes,
                            "text/csv",
                            "LoginCredentialsExport.csv");
            }
            else
            {
                return BadRequest("Invalid ExportType.");
            }
        }

        [HttpPost("GetNonAppUsers")]
        public async Task<IActionResult> GetNonAppUsers([FromBody] GetNonAppUsersRequest request)
        {
            var response = await _studentLoginsService.GetNonAppUsersAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetNonAppUsersExport")]
        public async Task<IActionResult> GetNonAppUsersExport([FromBody] GetNonAppUsersExportRequest request)
        {
            var response = await _studentLoginsService.GetNonAppUsersExportAsync(request);
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
                return File(fileBytes,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "NonAppUsersExport.xlsx");
            }
            else if (request.ExportType == 2)
            {
                return File(fileBytes,
                            "text/csv",
                            "NonAppUsersExport.csv");
            }
            else
            {
                return BadRequest("Invalid ExportType.");
            }
        }
    }
}
