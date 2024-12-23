using Infirmary_API.DTOs.Requests;
using Infirmary_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
    }
}
