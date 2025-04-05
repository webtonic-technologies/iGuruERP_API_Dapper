using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;

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
        public async Task<IActionResult> AddWalletAmount([FromBody] AddWalletAmountRequest request)
        {
            var response = await _walletService.AddWalletAmount(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
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


        [HttpPost("GetWalletHistory")]
        public IActionResult GetWalletHistory([FromBody] GetWalletHistoryRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request: StudentID and InstituteID are required.");
            }

            var response = _walletService.GetWalletHistory(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpPost("GetWalletHistoryExport")]
        public IActionResult GetWalletHistoryExport([FromBody] GetWalletHistoryExportRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request: StudentID, InstituteID, and ExportType are required.");
            }

            try
            {
                var fileData = _walletService.GetWalletHistoryExport(request);

                string contentType;
                string fileName;
                switch (request.ExportType)
                {
                    case 1:
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        fileName = "WalletHistoryExport.xlsx";
                        break;
                    case 2:
                        contentType = "text/csv";
                        fileName = "WalletHistoryExport.csv";
                        break;
                    default:
                        return BadRequest("Invalid ExportType");
                }

                return File(fileData, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
