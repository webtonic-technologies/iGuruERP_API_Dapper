using System.Threading.Tasks;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/Dashboard/FeesDashboard")]
    [ApiController]
    public class FeesDashboardController : ControllerBase
    {
        private readonly IFeesDashboardService _feesDashboardService;

        public FeesDashboardController(IFeesDashboardService feesDashboardService)
        {
            _feesDashboardService = feesDashboardService;
        }

        [HttpPost("GetFeeStatistics")]
        public async Task<IActionResult> GetFeeStatistics([FromBody] GetFeeStatisticsRequest request)
        {
            var result = await _feesDashboardService.GetFeeStatisticsAsync(request.InstituteID);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetHeadWiseCollectedAmount")]
        public async Task<IActionResult> GetHeadWiseCollectedAmount([FromBody] GetHeadWiseCollectedAmountRequest request)
        {
            var result = await _feesDashboardService.GetHeadWiseCollectedAmountAsync(request.InstituteID);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetDayWiseFees")]
        public async Task<IActionResult> GetDayWiseFees([FromBody] GetDayWiseFeesRequest request)
        {
            var result = await _feesDashboardService.GetDayWiseFeesAsync(request.InstituteID);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetClassSectionWise")]
        public async Task<IActionResult> GetClassSectionWise([FromBody] GetClassSectionWiseRequest request)
        {
            var result = await _feesDashboardService.GetClassSectionWiseAsync(request.InstituteID);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetTypeWiseCollection")]
        public async Task<IActionResult> GetTypeWiseCollection([FromBody] GetTypeWiseCollectionRequest request)
        {
            var result = await _feesDashboardService.GetTypeWiseCollectionAsync(request.InstituteID);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetModeWiseCollection")]
        public async Task<IActionResult> GetModeWiseCollection([FromBody] GetModeWiseCollectionRequest request)
        {
            var result = await _feesDashboardService.GetModeWiseCollectionAsync(request.InstituteID, request.Month, request.Year);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetCollectionAnalysis")]
        public async Task<IActionResult> GetCollectionAnalysis([FromBody] GetCollectionAnalysisRequest request)
        {
            var result = await _feesDashboardService.GetCollectionAnalysisAsync(request.InstituteID);
            return StatusCode(result.StatusCode, result);
        }
    }
} 