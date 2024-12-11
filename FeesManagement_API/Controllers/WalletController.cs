using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FeesManagement_API.Controllers
{
    [ApiController]
    [Route("iGuru/FeeCollection/Wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost("AddWalletAmount")]
        public IActionResult AddWalletAmount([FromBody] AddWalletAmountRequest request)
        {
            var result = _walletService.AddWalletAmount(request);
            return Ok(new { message = result });
        }

        [HttpPost("GetWallet")]
        public IActionResult GetWallet([FromBody] GetWalletRequest request)
        {
            var result = _walletService.GetWallet(request);
            return Ok(result);
        }

        [HttpPost("GetWalletExport")]
        public IActionResult GetWalletExport([FromBody] GetWalletExportRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request: Details are required.");
            }

            try
            {
                var fileData = _walletService.GetWalletExport(request);

                var contentType = request.ExportType == 1 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv";
                var fileName = request.ExportType == 1 ? "WalletExport.xlsx" : "WalletExport.csv";

                return File(fileData, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
