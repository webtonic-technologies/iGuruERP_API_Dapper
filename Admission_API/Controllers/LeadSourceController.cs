using Admission_API.DTOs.Requests;
using Admission_API.Models;
using Admission_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Admission_API.Controllers
{
    [Route("iGuru/Configuration/LeadSource")]
    [ApiController]
    public class LeadSourceController : ControllerBase
    {
        private readonly ILeadSourceService _leadSourceService;

        public LeadSourceController(ILeadSourceService leadSourceService)
        {
            _leadSourceService = leadSourceService;
        }

        [HttpPost("AddUpdateLeadSources")]
        public async Task<IActionResult> AddUpdateLeadSources([FromBody] List<LeadSource> request)
        {
            var result = await _leadSourceService.AddUpdateLeadSource(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost("GetAllLeadSource")]
        public async Task<IActionResult> GetAllLeadSources(GetAllRequest request)
        {
            var result = await _leadSourceService.GetAllLeadSources(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetLeadSource/{LeadSourceID}")]
        public async Task<IActionResult> GetLeadSourceById(int LeadSourceID)
        {
            var result = await _leadSourceService.GetLeadSourceById(LeadSourceID);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Status/{LeadSourceID}")]
        public async Task<IActionResult> UpdateLeadSourceStatus(int LeadSourceID)
        {
            var result = await _leadSourceService.UpdateLeadSourceStatus(LeadSourceID);
            return StatusCode(result.StatusCode, result);
        }
    }
}
