using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class LateFeeController : ControllerBase
    {
        private readonly ILateFeeService _lateFeeService;

        public LateFeeController(ILateFeeService lateFeeService)
        {
            _lateFeeService = lateFeeService;
        }

        [HttpPost("AddUpdateLateFee")]
        public async Task<IActionResult> AddUpdateLateFee([FromBody] AddUpdateLateFeeRequest request)
        {
            var response = await _lateFeeService.AddUpdateLateFee(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetAllLateFee")]
        public async Task<IActionResult> GetAllLateFee([FromBody] GetAllLateFeeRequest request)
        {
            var response = await _lateFeeService.GetAllLateFee(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetLateFee/{lateFeeRuleID}")]
        public async Task<IActionResult> GetLateFee(int lateFeeRuleID)
        {
            var response = await _lateFeeService.GetLateFeeById(lateFeeRuleID);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("Status/{lateFeeRuleID}")]
        public async Task<IActionResult> UpdateLateFeeStatus(int lateFeeRuleID)
        {
            var response = await _lateFeeService.UpdateLateFeeStatus(lateFeeRuleID);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetFeeTenureDDL")]
        public async Task<IActionResult> GetFeeTenureDDL([FromBody] GetFeeTenureDDLRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            var response = await _lateFeeService.GetFeeTenureDDL(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}
