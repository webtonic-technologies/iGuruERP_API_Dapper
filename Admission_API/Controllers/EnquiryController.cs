using Admission_API.DTOs.Requests;
using Admission_API.Models;
using Admission_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Admission_API.Controllers
{
    [Route("iGuru/Enquiry/Enquiry")]
    [ApiController]
    public class EnquiryController : ControllerBase
    {
        private readonly IEnquiryService _enquiryService;

        public EnquiryController(IEnquiryService enquiryService)
        {
            _enquiryService = enquiryService;
        }

        [HttpPost("AddEnquiry")]
        public async Task<IActionResult> AddEnquiry(List<EnquiryRequest> requests, int leadStageID, int InstituteID)
        {
            var result = await _enquiryService.AddEnquiry(requests, leadStageID, InstituteID);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetAllEnquiry")]
        public async Task<IActionResult> GetAllEnquiries(GetAllRequest request)
        {
            var result = await _enquiryService.GetAllEnquiries(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetEnquiryList")]
        public async Task<IActionResult> GetEnquiryList(GetEnqueryListRequest request)
        {
            var result = await _enquiryService.GetEnquiryList(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetLeadInformation")]
        public async Task<IActionResult> GetLeadInformation(GetLeadInformationRequest request)
        {
            var result = await _enquiryService.GetLeadInformation(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("SendEnquiryMessage/{EnquiryID}")]
        public async Task<IActionResult> SendEnquiryMessage(SendEnquiryMessageRequest request)
        {
            var result = await _enquiryService.SendEnquiryMessage(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("AddLeadComment")]
        public async Task<IActionResult> AddLeadComment(AddLeadCommentRequest request)
        {
            var result = await _enquiryService.AddLeadComment(request);
            return StatusCode(result.StatusCode, result);
        }

    }

    [Route("iGuru/Enquiry/SMSReport")]
    [ApiController]
    public class SMSReportController : ControllerBase
    {
        private readonly IEnquiryService _enquiryService;

        public SMSReportController(IEnquiryService enquiryService)
        {
            _enquiryService = enquiryService;
        }

        [HttpGet("GetSMSReport")]
        public async Task<IActionResult> GetSMSReport()
        {
            var result = await _enquiryService.GetSMSReport();
            return StatusCode(result.StatusCode, result);
        }
    }
}
