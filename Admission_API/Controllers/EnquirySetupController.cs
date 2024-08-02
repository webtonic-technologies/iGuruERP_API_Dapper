﻿using Admission_API.DTOs.Requests;
using Admission_API.Models;
using Admission_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Admission_API.Controllers
{
    [Route("iGuru/Configuration/EnquirySetup")]
    [ApiController]
    public class EnquirySetupController : ControllerBase
    {
        private readonly IEnquirySetupService _enquirySetupService;

        public EnquirySetupController(IEnquirySetupService enquirySetupService)
        {
            _enquirySetupService = enquirySetupService;
        }

        [HttpPost("AddUpdateEnquirySetup")]
        public async Task<IActionResult> AddUpdateEnquirySetup(EnquirySetup request)
        {
            var result = await _enquirySetupService.AddUpdateEnquirySetup(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetAllEnquirySetup")]
        public async Task<IActionResult> GetAllEnquirySetups(GetAllRequest request)
        {
            var result = await _enquirySetupService.GetAllEnquirySetups(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Delete/{EnquirySetupID}")]
        public async Task<IActionResult> DeleteEnquirySetup(int EnquirySetupID)
        {
            var result = await _enquirySetupService.DeleteEnquirySetup(EnquirySetupID);
            return StatusCode(result.StatusCode, result);
        }
    }
}
