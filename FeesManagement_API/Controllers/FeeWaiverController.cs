using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Services.Interfaces;

namespace FeesManagement_API.Controllers
{
    [ApiController]
    [Route("iGuru/FeeCollection/CollectFee")]
    public class FeeWaiverController : ControllerBase
    {
        private readonly IFeeWaiverService _feeWaiverService;

        public FeeWaiverController(IFeeWaiverService feeWaiverService)
        {
            _feeWaiverService = feeWaiverService;
        }

        [HttpPost("SubmitFeeWaiver")]
        public ActionResult<ServiceResponse<bool>> SubmitFeeWaiver([FromBody] SubmitFeeWaiverRequest request)
        {
            var response = _feeWaiverService.SubmitFeeWaiver(request);
            return Ok(response);
        }
    }
}
