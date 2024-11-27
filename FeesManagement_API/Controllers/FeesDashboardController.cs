using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.ServiceResponse;


namespace FeesManagement_API.Controllers
{
    [Route("iGuru/Dashboard/FeesDashboard/[controller]")]
    [ApiController]
    public class FeesDashboardController : ControllerBase
    {
        private readonly IFeesDashboardService _feesDashboardService;

        public FeesDashboardController(IFeesDashboardService feesDashboardService)
        {
            _feesDashboardService = feesDashboardService;
        }

        [HttpPost("TotalAmountCollected")]
        public async Task<IActionResult> GetTotalAmountCollected([FromBody] TotalAmountCollectedRequest request)
        {
            try
            {
                var response = await _feesDashboardService.GetTotalAmountCollectedAsync(request);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("TotalPendingAmount")]
        public async Task<ActionResult<ServiceResponse<TotalPendingAmountResponse>>> GetTotalPendingAmount([FromBody] TotalPendingAmountRequest request)
        {
            var response = await _feesDashboardService.GetTotalPendingAmountAsync(request);
            return Ok(response);
        }

        [HttpPost("HeadWiseCollectedAmount")]
        public async Task<ActionResult<ServiceResponse<List<HeadWiseCollectedAmountResponse>>>> GetHeadWiseCollectedAmount([FromBody] HeadWiseCollectedAmountRequest request)
        {
            var response = await _feesDashboardService.GetHeadWiseCollectedAmountAsync(request);
            return Ok(response);
        }

        [HttpPost("DayWise")]
        public async Task<ActionResult<ServiceResponse<List<DayWiseResponse>>>> GetDayWiseCollectedAmount([FromBody] DayWiseRequest request)
        {
            var response = await _feesDashboardService.GetDayWiseCollectedAmountAsync(request);
            return Ok(response);
        }

        [HttpPost("FeeCollectionAnalysis")]
        public async Task<ActionResult<ServiceResponse<List<FeeCollectionAnalysisResponse>>>> FeeCollectionAnalysis([FromBody] FeeCollectionAnalysisRequest request)
        {
            var result = await _feesDashboardService.GetFeeCollectionAnalysisAsync(request);
            return Ok(new ServiceResponse<List<FeeCollectionAnalysisResponse>>(true, "Data retrieved successfully", result, 200));
        }
    }
}
