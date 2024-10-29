using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Services.Interfaces;
using System.Linq;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/FeeCollection/CollectFee")]
    [ApiController]
    public class SubmitFeeDiscountController : ControllerBase
    {
        private readonly IFeeDiscountService _feeDiscountService;

        public SubmitFeeDiscountController(IFeeDiscountService feeDiscountService)
        {
            _feeDiscountService = feeDiscountService;
        }

        /// <summary>
        /// Applies fee discounts to students.
        /// </summary>
        /// <param name="request">The request containing fee discount details.</param>
        /// <returns>A service response indicating success or failure.</returns>
        [HttpPost("ApplyFeeDiscount")]
        public IActionResult ApplyFeeDiscount([FromBody] SubmitFeeDiscountRequest request)
        {
            // Validate the request
            if (request == null || request.FeeDiscounts == null || !request.FeeDiscounts.Any())
            {
                return BadRequest("Invalid request: Fee discounts are required.");
            }

            // Call the service to apply the fee discounts
            var response = _feeDiscountService.ApplyDiscount(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }
    }
}
