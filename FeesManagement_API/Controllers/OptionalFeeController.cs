using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;  // Make sure this is added
using FeesManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FeesManagement_API.Controllers
{
    [ApiController]
    [Route("iGuru/Configuration/OptionalFee")]
    public class OptionalFeeController : ControllerBase
    {
        private readonly IOptionalFeeService _optionalFeeService;

        public OptionalFeeController(IOptionalFeeService optionalFeeService)
        {
            _optionalFeeService = optionalFeeService;
        }

        [HttpPost("AddUpdateOptionalFee")]
        public async Task<IActionResult> AddUpdateOptionalFee([FromBody] AddUpdateOptionalFeeRequest request)
        {
            var result = await _optionalFeeService.AddUpdateOptionalFee(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        //[HttpPost("GetAllOptionalFees")]
        //public async Task<IActionResult> GetAllOptionalFees([FromBody] GetAllOptionalFeesRequest request)
        //{
        //    var response = await _optionalFeeService.GetAllOptionalFees(request);
        //    if (response.Success)
        //    {
        //        return Ok(response);
        //    }
        //    return BadRequest(response);
        //}

        [HttpPost("GetAllOptionalFees")]
        public async Task<IActionResult> GetAllOptionalFees([FromBody] GetAllOptionalFeesRequest request)
        {
            var response = await _optionalFeeService.GetAllOptionalFees(request);
            if (response.Success)
            {
                return Ok(response); // Return the entire ServiceResponse
            }
            return BadRequest(response);
        }



        [HttpGet("GetOptionalFee/{optionalFeeID}")]
        public async Task<IActionResult> GetOptionalFee(int optionalFeeID)
        {
            var result = await _optionalFeeService.GetOptionalFeeById(optionalFeeID);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpPut("Delete/{optionalFeeID}")]
        public async Task<IActionResult> DeleteOptionalFee(int optionalFeeID)
        {
            var result = await _optionalFeeService.UpdateOptionalFeeStatus(optionalFeeID);
            if (result > 0)
            {
                return Ok(new ServiceResponse<bool>(true, "Optional Fee status updated successfully.", true, 200));
            }
            return BadRequest(new ServiceResponse<bool>(false, "Failed to update optional fee status.", false, 400));
        }
    }
}
