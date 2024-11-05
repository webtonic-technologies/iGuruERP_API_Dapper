using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;


namespace FeesManagement_API.Controllers
{
    [Route("iGuru/Report/FeeReports/[controller]")]
    [ApiController]
    public class FeesReportsController : ControllerBase
    {
        private readonly IFeesReportsService _feesReportsService;

        public FeesReportsController(IFeesReportsService feesReportsService)
        {
            _feesReportsService = feesReportsService;
        }

        [HttpPost("DailyPaymentSummaryReport")]
        public async Task<IActionResult> GetDailyPaymentSummary(DailyPaymentSummaryRequest request)
        {
            try
            {
                var data = await _feesReportsService.GetDailyPaymentSummaryAsync(request);
                if (data != null)
                {
                    return Ok(data);
                }
                return BadRequest("Bad Request");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("FeePaymentSummaryReport")]
        public async Task<IActionResult> GetFeePaymentSummaryReport(FeePaymentSummaryRequest request)
        {
            try
            {
                var data = await _feesReportsService.GetFeePaymentSummaryAsync(request);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("PaidFeeReport")]
        public async Task<IActionResult> GetPaidFeeReport([FromBody] PaidFeeRequest request)
        {
            try
            {
                var response = await _feesReportsService.GetPaidFeeReportAsync(request);
                if (response.Success)
                {
                    return Ok(response);
                }
                return BadRequest(response.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("ConcessionTypeReport")]
        public async Task<IActionResult> GetConcessionTypeReport([FromBody] ConcessionTypeRequest request)
        {
            try
            {
                var response = await _feesReportsService.GetConcessionTypeReportAsync(request);
                if (response.Success)
                {
                    return Ok(response);
                }
                return BadRequest(response.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("ClassWiseConcessionReport")]
        public async Task<IActionResult> GetClassWiseConcessionReport([FromBody] ClassWiseConcessionRequest request)
        {
            try
            {
                var response = await _feesReportsService.GetClassWiseConcessionReportAsync(request);
                if (response.Success)
                {
                    return Ok(response);
                }
                return BadRequest(response.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("DiscountSummaryReport")]
        public async Task<ActionResult<ServiceResponse<List<DiscountSummaryResponse>>>> GetDiscountSummaryReport([FromBody] DiscountSummaryRequest request)
        {
            var result = await _feesReportsService.GetDiscountSummaryReportAsync(request);
            return Ok(result);
        }

        [HttpPost("WaiverSummaryReport")]
        public async Task<IActionResult> GetWaiverSummaryReport([FromBody] WaiverSummaryRequest request)
        {
            try
            {
                var data = await _feesReportsService.GetWaiverSummaryReportAsync(request);
                if (data != null)
                {
                    return Ok(data);
                }
                return BadRequest("No data found.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
