using Microsoft.AspNetCore.Mvc;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.Services.Interfaces;
using System.Threading.Tasks;

namespace SiteAdmin_API.Controllers
{
    [Route("iGuru/ControlPanel")]
    [ApiController]
    public class SMSController : ControllerBase
    {
        private readonly ISMSService _smsService;

        public SMSController(ISMSService smsService)
        {
            _smsService = smsService;
        }

        [HttpPost("SMS/AddUpdateSMSVendor")]
        public async Task<IActionResult> AddUpdateSMSVendor([FromBody] AddUpdateSMSVendorRequest request)
        {
            var response = await _smsService.AddUpdateSMSVendor(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("SMS/GetAllSMSVendor")]
        public async Task<IActionResult> GetAllSMSVendor()
        {
            var response = await _smsService.GetAllSMSVendor();
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("SMS/GetSMSVendorByID/{SMSVendorID}")]
        public async Task<IActionResult> GetSMSVendorByID([FromRoute] int SMSVendorID)
        {
            var response = await _smsService.GetSMSVendorByID(SMSVendorID);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("SMS/DeleteSMSVendor")]
        public async Task<IActionResult> DeleteSMSVendor([FromBody] DeleteSMSVendorRequest request)
        {
            var response = await _smsService.DeleteSMSVendor(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("SMS/AddUpdateSMSPlan")]
        public async Task<IActionResult> AddUpdateSMSPlan([FromBody] AddUpdateSMSPlanRequest request)
        {
            var response = await _smsService.AddUpdateSMSPlan(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("SMS/GetAllSMSPlan")]
        public async Task<IActionResult> GetAllSMSPlan([FromBody] GetAllSMSPlanRequest request)
        {
            var response = await _smsService.GetAllSMSPlan(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetSMSPlanByID/{SMSVendorID}")]
        public async Task<IActionResult> GetSMSPlanByID(int SMSVendorID)
        {
            var response = await _smsService.GetSMSPlanByID(SMSVendorID);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("DeleteSMSPlan")]
        public async Task<IActionResult> DeleteSMSPlan([FromBody] DeleteSMSPlanRequest request)
        {
            var response = await _smsService.DeleteSMSPlan(request.RateID);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("CreateSMSOrder")]
        public async Task<IActionResult> CreateSMSOrder([FromBody] CreateSMSOrderRequest request)
        {
            var response = await _smsService.CreateSMSOrder(request);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("GetSMSOrder")]
        public async Task<IActionResult> GetSMSOrder([FromBody] GetSMSOrderRequest request)
        {
            var response = await _smsService.GetSMSOrder(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
