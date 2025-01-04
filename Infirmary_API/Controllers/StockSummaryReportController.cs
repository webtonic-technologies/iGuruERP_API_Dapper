using Microsoft.AspNetCore.Mvc;
using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Responses;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Services.Interfaces;

namespace Infirmary_API.Controllers
{
    [Route("iGuru/StockSummaryReport/StockSummaryReport")]
    [ApiController]
    public class StockSummaryReportController : ControllerBase
    {
        private readonly IStockSummaryReportService _stockSummaryReportService;

        public StockSummaryReportController(IStockSummaryReportService stockSummaryReportService)
        {
            _stockSummaryReportService = stockSummaryReportService;
        }

        [HttpPost("GetStockSummaryReport")]
        public async Task<ActionResult<ServiceResponse<List<GetStockSummaryReportResponse>>>> GetStockSummaryReport(GetStockSummaryReportRequest request)
        {
            var response = await _stockSummaryReportService.GetStockSummaryReport(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetStockSummaryReportExport")]
        public async Task<IActionResult> GetStockSummaryReportExport(GetStockSummaryReportExportRequest request)
        {
            var response = await _stockSummaryReportService.GetStockSummaryReportExport(request);

            if (response.Success)
            {
                // Return file based on export type
                var fileBytes = response.Data;
                var fileName = request.ExportType == 1 ? "StockSummaryReport.xlsx" : "StockSummaryReport.csv";
                var contentType = request.ExportType == 1 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv";

                return File(fileBytes, contentType, fileName);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
    }
}
