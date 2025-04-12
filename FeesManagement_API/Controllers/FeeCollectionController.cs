using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Services.Implementations;

namespace FeesManagement_API.Controllers
{
    [ApiController]
    [Route("iGuru/FeeCollection")]
    public class FeeCollectionController : ControllerBase
    {
        private readonly IFeeCollectionService _feeCollectionService;
        private readonly IStudentInformationService _studentInformationService;

        public FeeCollectionController(IFeeCollectionService feeCollectionService, IStudentInformationService studentInformationService)
        {
            _feeCollectionService = feeCollectionService;
            _studentInformationService = studentInformationService;
        }

        [HttpPost("CollectFee/GetFee")]
        public ActionResult<ServiceResponse<GetFeeResponse>> GetFee([FromBody] GetFeeRequest request)
        {
            var response = _feeCollectionService.GetFee(request);
            return Ok(response);
        }

        //[HttpPost("CollectFee/GetStudentInformation")]
        //public ActionResult<ServiceResponse<StudentInformationResponse>> GetStudentInformation([FromBody] StudentInformationRequest request)
        //{
        //    var response = _studentInformationService.GetStudentInformation(request);
        //    return Ok(response);
        //}

        [HttpPost("CollectFee/GetStudentInformation")]
        public ActionResult<ServiceResponse<List<StudentInformationResponse>>> GetStudentInformation([FromBody] StudentInformationRequest request)
        {
            var response = _studentInformationService.GetStudentInformation(request);
            return Ok(response);
        }


        [HttpPost("CollectFee/SubmitPayment")]
        public ActionResult<ServiceResponse<bool>> SubmitPayment([FromBody] SubmitPaymentRequest request)
        {
            var response = _feeCollectionService.SubmitPayment(request);
            return Ok(response);
        }
         

        [HttpPost("CollectFee/SubmitFeeWaiver")]
        public ActionResult<ServiceResponse<bool>> SubmitFeeWaiver([FromBody] SubmitFeeWaiverRequest request)
        {
            var response = _feeCollectionService.SubmitFeeWaiver(request);
            return Ok(response);
        }

        [HttpPost("CollectFee/ApplyFeeDiscount")]
        public IActionResult ApplyFeeDiscount([FromBody] SubmitFeeDiscountRequest request)
        {
            // Validate the request
            if (request == null || request.FeeDiscounts == null || !request.FeeDiscounts.Any())
            {
                return BadRequest("Invalid request: Fee discounts are required.");
            }

            // Call the service to apply the fee discounts
            var response = _feeCollectionService.ApplyDiscount(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }

        [HttpPost("CollectFee/GetWaiverSummary")]
        public ActionResult<ServiceResponse<GetWaiverSummaryResponse>> GetWaiverSummary([FromBody] GetWaiverSummaryRequest request)
        {
            var response = _feeCollectionService.GetWaiverSummary(request);
            return Ok(response);
        }


        [HttpPost("CollectFee/GetDiscountSummary")]
        public ActionResult<ServiceResponse<GetDiscountSummaryResponse>> GetDiscountSummary([FromBody] GetDiscountSummaryRequest request)
        {
            var response = _feeCollectionService.GetDiscountSummary(request);
            return Ok(response);
        }

        [HttpPost("CollectFee/GetCollectFee")]
        public ActionResult<ServiceResponse<GetCollectFeeResponse>> GetCollectFee([FromBody] GetCollectFeeRequest request)
        {
            var response = _feeCollectionService.GetCollectFee(request);
            return Ok(response);
        }
    }
}
