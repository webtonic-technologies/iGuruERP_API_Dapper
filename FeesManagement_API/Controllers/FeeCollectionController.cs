using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;

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

        [HttpPost("CollectFee/GetStudentInformation")]
        public ActionResult<ServiceResponse<StudentInformationResponse>> GetStudentInformation([FromBody] StudentInformationRequest request)
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

    }
}
