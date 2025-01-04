using Infirmary_API.DTOs.Requests;
using Infirmary_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.DTOs.Responses;

namespace Infirmary_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class StockEntryController : ControllerBase
    {
        private readonly IStockEntryService _stockEntryService;

        public StockEntryController(IStockEntryService stockEntryService)
        {
            _stockEntryService = stockEntryService;
        }

        [HttpPost("AddUpdateStockEntry")]
        public async Task<IActionResult> AddUpdateStockEntry(AddUpdateStockEntryRequest request)
        {
            try
            {
                var data = await _stockEntryService.AddUpdateStockEntry(request);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPost("GetAllStockEntries")]
        public async Task<IActionResult> GetAllStockEntries(GetAllStockEntriesRequest request)
        {
            try
            {
                var data = await _stockEntryService.GetAllStockEntries(request);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpGet("GetStockEntryById/{StockID}")]
        public async Task<IActionResult> GetStockEntryById(int StockID)
        {
            try
            {
                var data = await _stockEntryService.GetStockEntryById(StockID);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPut("DeleteStockEntry/{StockID}")]
        public async Task<IActionResult> DeleteStockEntry(int StockID)
        {
            try
            {
                var data = await _stockEntryService.DeleteStockEntry(StockID);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPost("StockEntries/GetStockEntriesExport")]
        public async Task<IActionResult> GetStockEntriesExport([FromBody] GetStockEntriesExportRequest request)
        {
            var response = await _stockEntryService.ExportStockEntriesData(request);

            if (response.Success)
            {
                string fileName = "StockEntries_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + (request.ExportType == 1 ? ".xlsx" : ".csv");
                string contentType = request.ExportType == 1 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv";

                return File(response.Data, contentType, fileName);
            }

            return BadRequest(response.Message);
        }

        [HttpPost("EnterInfirmaryStockAdjustment")]
        public async Task<ActionResult<ServiceResponse<string>>> EnterInfirmaryStockAdjustment(EnterInfirmaryStockAdjustmentRequest request)
        {
            var response = await _stockEntryService.EnterInfirmaryStockAdjustment(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("StockHistory")]
        public async Task<ActionResult<ServiceResponse<List<StockHistoryResponse>>>> GetStockHistory(StockHistoryRequest request)
        {
            var response = await _stockEntryService.GetStockHistory(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetStockInfo")]
        public async Task<ActionResult<ServiceResponse<GetStockInfoResponse>>> GetStockInfo(GetStockInfoRequest request)
        {
            var response = await _stockEntryService.GetStockInfo(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("GetStockHistoryExport")]
        public async Task<IActionResult> GetStockHistoryExport(GetStockHistoryExportRequest request)
        {
            var response = await _stockEntryService.GetStockHistoryExport(request);

            if (response.Success)
            {
                var fileBytes = response.Data;
                var fileName = request.ExportType == 1 ? "StockHistoryReport.xlsx" : "StockHistoryReport.csv";
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
