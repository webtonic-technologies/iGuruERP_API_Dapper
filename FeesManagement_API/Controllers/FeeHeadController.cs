using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class FeeHeadController : ControllerBase
    {
        private readonly IFeeHeadService _feeHeadService;

        public FeeHeadController(IFeeHeadService feeHeadService)
        {
            _feeHeadService = feeHeadService;
        }

        [HttpPost("AddUpdateFeeHead")]
        public async Task<IActionResult> AddUpdateFeeHead(AddUpdateFeeHeadRequest request)
        {
            var response = await _feeHeadService.AddUpdateFeeHead(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetAllFeeHead")]
        public async Task<IActionResult> GetAllFeeHead(GetAllFeeHeadRequest request)
        {
            var response = await _feeHeadService.GetAllFeeHead(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return NoContent();
        }

        [HttpGet("GetFeeHead/{FeeHeadID}")]
        public async Task<IActionResult> GetFeeHead(int FeeHeadID)
        {
            var response = await _feeHeadService.GetFeeHeadById(FeeHeadID);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("DeleteFeeHead/{FeeHeadID}")]
        public async Task<IActionResult> DeleteFeeHead(int FeeHeadID)
        {
            var response = await _feeHeadService.DeleteFeeHead(FeeHeadID);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
