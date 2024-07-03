﻿using Communication_API.DTOs.Requests.WhatsApp;
using Communication_API.Services.Interfaces.WhatsApp;
using Microsoft.AspNetCore.Mvc;

namespace Communication_API.Controllers
{
    [Route("iGuru/WhatsApp/[controller]")]
    [ApiController]
    public class WhatsAppController : ControllerBase
    {
        private readonly IWhatsAppService _whatsAppService;

        public WhatsAppController(IWhatsAppService whatsAppService)
        {
            _whatsAppService = whatsAppService;
        }

        [HttpPost("Setup")]
        public async Task<IActionResult> Setup([FromBody] SetupWhatsAppRequest request)
        {
            var response = await _whatsAppService.Setup(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetBalance/{VendorID}")]
        public async Task<IActionResult> GetBalance(int VendorID)
        {
            var response = await _whatsAppService.GetBalance(VendorID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("AddUpdateTemplate")]
        public async Task<IActionResult> AddUpdateTemplate([FromBody] AddUpdateTemplateRequest request)
        {
            var response = await _whatsAppService.AddUpdateTemplate(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetWhatsAppTemplate")]
        public async Task<IActionResult> GetWhatsAppTemplate([FromBody] GetWhatsAppTemplateRequest request)
        {
            var response = await _whatsAppService.GetWhatsAppTemplate(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("Send")]
        public async Task<IActionResult> Send([FromBody] SendWhatsAppRequest request)
        {
            var response = await _whatsAppService.Send(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetWhatsAppReport")]
        public async Task<IActionResult> GetWhatsAppReport([FromBody] GetWhatsAppReportRequest request)
        {
            var response = await _whatsAppService.GetWhatsAppReport(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
