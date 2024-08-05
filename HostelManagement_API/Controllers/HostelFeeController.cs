using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/HostelFee/[controller]")]
    [ApiController]
    public class HostelFeeController : ControllerBase
    {
        private readonly ICautionDepositService _cautionDepositService;
        private readonly ILogger<HostelFeeController> _logger;

        public HostelFeeController(ICautionDepositService cautionDepositService, ILogger<HostelFeeController> logger)
        {
            _cautionDepositService = cautionDepositService;
            _logger = logger;
        }

        [HttpPost("AddUpdateCautionDeposit")]
        public async Task<IActionResult> AddUpdateCautionDeposit([FromBody] AddUpdateCautionDepositRequest request)
        {
            _logger.LogInformation("AddUpdateCautionDeposit Request Received: {@Request}", request);
            var response = await _cautionDepositService.AddUpdateCautionDeposit(request);
            _logger.LogInformation("AddUpdateCautionDeposit Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllCautionDeposit")]
        public async Task<IActionResult> GetAllCautionDeposits([FromBody] GetAllCautionDepositRequest request)
        {
            _logger.LogInformation("GetAllCautionDeposits Request Received: {@Request}", request);
            var response = await _cautionDepositService.GetAllCautionDeposits(request);
            _logger.LogInformation("GetAllCautionDeposits Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetCautionDeposit/{cautionFeeId}")]
        public async Task<IActionResult> GetCautionDepositById(int cautionFeeId)
        {
            _logger.LogInformation("GetCautionDepositById Request Received for CautionFeeID: {CautionFeeID}", cautionFeeId);
            var response = await _cautionDepositService.GetCautionDepositById(cautionFeeId);
            _logger.LogInformation("GetCautionDepositById Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{cautionFeeId}")]
        public async Task<IActionResult> DeleteCautionDeposit(int cautionFeeId)
        {
            _logger.LogInformation("DeleteCautionDeposit Request Received for CautionFeeID: {CautionFeeID}", cautionFeeId);
            var response = await _cautionDepositService.DeleteCautionDeposit(cautionFeeId);
            _logger.LogInformation("DeleteCautionDeposit Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }
    }
}
