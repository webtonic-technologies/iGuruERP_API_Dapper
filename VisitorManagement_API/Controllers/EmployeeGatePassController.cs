using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VisitorManagement_API.Models;
using VisitorManagement_API.Services.Interfaces;
using VisitorManagement_API.DTOs.Requests;

namespace VisitorManagement_API.Controllers
{
    [Route("iGuru/EmployeeGatePass")]
    [ApiController]
    public class EmployeeGatePassController : ControllerBase
    {
        private readonly IEmployeeGatePassService _employeeGatePassService;

        public EmployeeGatePassController(IEmployeeGatePassService employeeGatePassService)
        {
            _employeeGatePassService = employeeGatePassService;
        }

        [HttpPost("EmployeeGatePass/AddUpdateEmployeeGatePass")]
        public async Task<IActionResult> AddUpdateEmployeeGatePass(EmployeeGatePass employeeGatePass)
        {
            var response = await _employeeGatePassService.AddUpdateEmployeeGatePass(employeeGatePass);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("EmployeeGatePass/GetAllEmployeeGatePass")]
        public async Task<IActionResult> GetAllEmployeeGatePass(GetAllEmployeeGatePassRequest request)
        {
            var response = await _employeeGatePassService.GetAllEmployeeGatePass(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("EmployeeGatePass/GetEmployeeGatePass/{gatePassId}")]
        public async Task<IActionResult> GetEmployeeGatePassById(int gatePassId)
        {
            var response = await _employeeGatePassService.GetEmployeeGatePassById(gatePassId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("EmployeeGatePass/Delete/{gatePassId}")]
        public async Task<IActionResult> UpdateEmployeeGatePassStatus(int gatePassId)
        {
            var response = await _employeeGatePassService.UpdateEmployeeGatePassStatus(gatePassId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("GetVisitedFor")]
        public async Task<IActionResult> GetAllVisitedForReason()
        {
            var response = await _employeeGatePassService.GetAllVisitedForReason();
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("EmployeeGatePass/GetVisitorForDDL")]
        public async Task<IActionResult> GetVisitorForDDL()
        {
            var response = await _employeeGatePassService.GetVisitorForDDL();
            if (response != null)
            {
                return Ok(new { Success = true, Data = response });
            }
            return BadRequest(new { Success = false, Message = "Failed to retrieve visitor types." });
        }

        [HttpGet("EmployeeGatePass/GetGatePassSlip")]
        public async Task<IActionResult> GetGatePassSlip([FromQuery] GetGatePassSlipRequest request)
        {
            var response = await _employeeGatePassService.GetGatePassSlip(request.GatePassID, request.InstituteID);
            if (response != null)
            {
                return Ok(new { Success = true, Data = response });
            }
            return BadRequest(new { Success = false, Message = "Failed to retrieve GatePass Slip." });
        } 


        [HttpPost("EmployeeGatePass/GetEmployeeGatePassExport")]
        public async Task<IActionResult> GetEmployeeGatePassExport([FromBody] GetEmployeeGatePassExportRequest request)
        {
            var response = await _employeeGatePassService.GetEmployeeGatePassExport(request);
            if (response.Success)
            {
                return File(response.Data, response.StatusCode == 200 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv", "EmployeeGatePassExport" + (request.ExportType == 1 ? ".xlsx" : ".csv"));
            }

            return BadRequest(response);
        }

    }
}
