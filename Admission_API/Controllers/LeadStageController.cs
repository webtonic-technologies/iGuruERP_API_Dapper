using Admission_API.DTOs.Requests;
using Admission_API.Models;
using Admission_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Admission_API.Controllers
{
    [Route("iGuru/Configuration/LeadStages")]
    [ApiController]
    public class LeadStageController : ControllerBase
    {
        private readonly ILeadStageService _leadStageService;

        public LeadStageController(ILeadStageService leadStageService)
        {
            _leadStageService = leadStageService;
        }

        [HttpPost("AddUpdateLeadStages")]
        public async Task<IActionResult> AddUpdateLeadStages([FromBody] List<LeadStage> request)
        {
            var result = await _leadStageService.AddUpdateLeadStage(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost("GetAllLeadStages")]
        public async Task<IActionResult> GetAllLeadStages(GetAllRequest request)
        {
            var result = await _leadStageService.GetAllLeadStages(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetLeadStage/{LeadStageID}")]
        public async Task<IActionResult> GetLeadStageById(int LeadStageID)
        {
            var result = await _leadStageService.GetLeadStageById(LeadStageID);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Status/{LeadStageID}")]
        public async Task<IActionResult> UpdateLeadStageStatus(int LeadStageID)
        {
            var result = await _leadStageService.UpdateLeadStageStatus(LeadStageID);
            return StatusCode(result.StatusCode, result);
        }
    }
}
