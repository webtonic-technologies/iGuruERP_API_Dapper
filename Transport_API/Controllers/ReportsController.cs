using Microsoft.AspNetCore.Mvc;
using Transport_API.DTOs.Requests;
using Transport_API.Services.Interfaces;

namespace Transport_API.Controllers
{
    [Route("iGuru/Reports/Reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _reportsService;

        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        [HttpGet("GetTransportPendingFeeReport")]
        public async Task<IActionResult> GetTransportPendingFeeReport(GetReportsRequest request)
        {
            try
            {
                var response = await _reportsService.GetTransportPendingFeeReport(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetEmployeeTransportationReport")]
        public async Task<IActionResult> GetEmployeeTransportationReport(GetReportsRequest request)
        {
            try
            {
                var response = await _reportsService.GetEmployeeTransportationReport(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetStudentTransportReport")]
        public async Task<IActionResult> GetStudentTransportReport(GetReportsRequest request)
        {
            try
            {
                var response = await _reportsService.GetStudentTransportReport(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetDeAllocationReport")]
        public async Task<IActionResult> GetDeAllocationReport(GetReportsRequest request)
        {
            try
            {
                var response = await _reportsService.GetDeAllocationReport(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
