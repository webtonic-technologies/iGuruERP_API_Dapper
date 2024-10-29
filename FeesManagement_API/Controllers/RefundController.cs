using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.DTOs.Responses;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/FeeCollection/Refund")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly IRefundService _refundService;

        public RefundController(IRefundService refundService)
        {
            _refundService = refundService;
        }

        [HttpPost("AddRefund")]
        public ActionResult<ServiceResponse<string>> AddRefund([FromBody] AddRefundRequest request)
        {
            var result = _refundService.AddRefund(request);
            return Ok(new ServiceResponse<string>(true, "Refund added successfully", result, 200));
        }

        [HttpPost("GetRefund")]
        public ActionResult<ServiceResponse<List<GetRefundResponse>>> GetRefund([FromBody] GetRefundRequest request)
        {
            var result = _refundService.GetRefund(request);
            return Ok(new ServiceResponse<List<GetRefundResponse>>(true, "Refunds retrieved successfully", result, 200));
        }

        [HttpDelete("DeleteRefund/{refundID}")]
        public ActionResult<ServiceResponse<string>> DeleteRefund(int refundID)
        {
            var response = _refundService.DeleteRefund(refundID);
            return Ok(response);
        }
    }
}
