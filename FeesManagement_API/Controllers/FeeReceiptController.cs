using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FeesManagement_API.Controllers
{
    [ApiController]
    [Route("iGuru/Configuration/Receipt")]
    public class FeeReceiptController : ControllerBase
    {
        private readonly IFeeReceiptService _feeReceiptService;

        public FeeReceiptController(IFeeReceiptService feeReceiptService)
        {
            _feeReceiptService = feeReceiptService;
        }
         

        [HttpPost("GetReceiptComponent")]
        public async Task<IActionResult> GetReceiptComponent([FromBody] GetReceiptComponentRequest request)
        {
            var result = await _feeReceiptService.GetReceiptComponents(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPost("GetReceiptProperty")]
        public async Task<IActionResult> GetReceiptProperty([FromBody] GetReceiptPropertyRequest request)
        {
            var result = await _feeReceiptService.GetReceiptProperties(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetReceiptLayout")]
        public async Task<IActionResult> GetReceiptLayout()
        {
            var result = await _feeReceiptService.GetReceiptLayouts();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPost("GetReceiptType")]
        public async Task<IActionResult> GetReceiptType()
        {
            var result = await _feeReceiptService.GetReceiptTypes();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPost("ConfigureReceipt")]
        public async Task<IActionResult> ConfigureReceipt([FromBody] ConfigureReceiptRequest request)
        {
            var result = await _feeReceiptService.ConfigureReceipt(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

    }
}
