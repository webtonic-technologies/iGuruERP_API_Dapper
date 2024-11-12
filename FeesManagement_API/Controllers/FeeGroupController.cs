using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks; 


namespace FeesManagement_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class FeeGroupController : ControllerBase
    {
        private readonly IFeeGroupService _feeGroupService;

        public FeeGroupController(IFeeGroupService feeGroupService)
        {
            _feeGroupService = feeGroupService;
        }

        [HttpPost("AddUpdateFeeGroup")]
        public async Task<IActionResult> AddUpdateFeeGroup([FromBody] AddUpdateFeeGroupsRequest request)
        {
            // Assuming no extra validation is needed, or the logic is handled elsewhere
            var response = await _feeGroupService.AddUpdateFeeGroup(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetAllFeeGroup")]
        public async Task<IActionResult> GetAllFeeGroups([FromBody] GetAllFeeGroupRequest request)
        {
            var response = await _feeGroupService.GetAllFeeGroups(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetFeeGroup/{feeGroupID}")]
        public async Task<IActionResult> GetFeeGroup(int feeGroupID)
        {
            var response = await _feeGroupService.GetFeeGroupById(feeGroupID);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("Status/{feeGroupID}")]
        public async Task<IActionResult> UpdateFeeGroupStatus(int feeGroupID, [FromBody] UpdateFeeGroupStatusRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            var response = await _feeGroupService.UpdateFeeGroupStatus(feeGroupID, request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}


