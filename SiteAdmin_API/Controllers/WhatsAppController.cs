using Microsoft.AspNetCore.Mvc;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.Services.Interfaces;
using System.Threading.Tasks;

namespace SiteAdmin_API.Controllers
{
    [Route("iGuru/ControlPanel")]
    [ApiController]
    public class WhatsAppController : ControllerBase
    {
        private readonly IWhatsAppService _whatsAppService;

        public WhatsAppController(IWhatsAppService whatsAppService)
        {
            _whatsAppService = whatsAppService;
        }

        [HttpPost("WhatsApp/AddUpdateWhatsAppVendor")]
        public async Task<IActionResult> AddUpdateWhatsAppVendor([FromBody] AddUpdateWhatsAppVendorRequest request)
        {
            var response = await _whatsAppService.AddUpdateWhatsAppVendor(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("WhatsApp/GetAllWhatsAppVendor")]
        public async Task<IActionResult> GetAllWhatsAppVendor()
        {
            var response = await _whatsAppService.GetAllWhatsAppVendor();
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("WhatsApp/GetWhatsAppVendorByID/{WhatsAppVendorID}")]
        public async Task<IActionResult> GetWhatsAppVendorByID([FromRoute] int WhatsAppVendorID)
        {
            var response = await _whatsAppService.GetWhatsAppVendorByID(WhatsAppVendorID);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("WhatsApp/DeleteWhatsAppVendor")]
        public async Task<IActionResult> DeleteWhatsAppVendor([FromBody] DeleteWhatsAppVendorRequest request)
        {
            var response = await _whatsAppService.DeleteWhatsAppVendor(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("WhatsApp/AddUpdateWhatsAppPlan")]
        public async Task<IActionResult> AddUpdateWhatsAppPlan([FromBody] AddUpdateWhatsAppPlanRequest request)
        {
            var response = await _whatsAppService.AddUpdateWhatsAppPlan(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        } 

        [HttpPost("WhatsApp/GetAllWhatsAppPlan")]
        public async Task<IActionResult> GetAllWhatsAppPlan([FromBody] GetAllWhatsAppPlanRequest request)
        {
            var response = await _whatsAppService.GetAllWhatsAppPlan(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("WhatsApp/GetWhatsAppPlanByID/{WhatsAppVendorID}")]
        public async Task<IActionResult> GetWhatsAppPlanByID([FromRoute] int WhatsAppVendorID)
        {
            var response = await _whatsAppService.GetWhatsAppPlanByID(WhatsAppVendorID);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("WhatsApp/DeleteWhatsAppPlan")]
        public async Task<IActionResult> DeleteWhatsAppPlan([FromBody] DeleteWhatsAppPlanRequest request)
        {
            var response = await _whatsAppService.DeleteWhatsAppPlan(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("WhatsApp/CreateWhatsAppOrder")]
        public async Task<IActionResult> CreateWhatsAppOrder([FromBody] CreateWhatsAppOrderRequest request)
        {
            var response = await _whatsAppService.CreateWhatsAppOrder(request);
            if (response.Success)
            {
                return Ok(response); // Returning success response
            }
            return BadRequest(response); // Returning failure response
        }

        [HttpPost("WhatsApp/GetWhatsAppOrder")]
        public async Task<IActionResult> GetWhatsAppOrder([FromBody] GetWhatsAppOrderRequest request)
        {
            var response = await _whatsAppService.GetWhatsAppOrder(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
