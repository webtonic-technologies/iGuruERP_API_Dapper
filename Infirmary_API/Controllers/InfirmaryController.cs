using Infirmary_API.DTOs.Requests;
using Infirmary_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Infirmary_API.DTOs.ServiceResponse;


namespace Infirmary_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class InfirmaryController : ControllerBase
    {
        private readonly IInfirmaryService _infirmaryService;

        public InfirmaryController(IInfirmaryService infirmaryService)
        {
            _infirmaryService = infirmaryService;
        }

        [HttpPost("AddUpdateInfirmary")]
        public async Task<IActionResult> AddUpdateInfirmary(AddUpdateInfirmaryRequest request)
        {
            try
            {
                var data = await _infirmaryService.AddUpdateInfirmary(request);
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

        [HttpPost("GetAllInfirmary")]
        public async Task<IActionResult> GetAllInfirmary(GetAllInfirmaryRequest request)
        {
            try
            {
                var data = await _infirmaryService.GetAllInfirmary(request);
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

        [HttpGet("GetInfirmary/{InfirmaryID}")]
        public async Task<IActionResult> GetInfirmary(int InfirmaryID)
        {
            try
            {
                var data = await _infirmaryService.GetInfirmaryById(InfirmaryID);
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

        [HttpPut("DeleteInfirmary/{InfirmaryID}")]
        public async Task<IActionResult> DeleteInfirmary(int InfirmaryID)
        {
            try
            {
                var data = await _infirmaryService.DeleteInfirmary(InfirmaryID);
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

        [HttpPost("Infirmary/GetInfirmaryExport")]
        public async Task<IActionResult> GetInfirmaryExport([FromBody] GetInfirmaryExportRequest request)
        {
            var response = await _infirmaryService.ExportInfirmaryData(request);

            if (response.Success)
            {
                string fileName = "InfirmaryData_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + (request.ExportType == 1 ? ".xlsx" : ".csv");
                string contentType = request.ExportType == 1 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv";

                return File(response.Data, contentType, fileName);
            }

            return BadRequest(response.Message);
        }
    }
}
