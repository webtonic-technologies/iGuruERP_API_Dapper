using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VisitorManagement_API.Models;
using VisitorManagement_API.Services.Interfaces;
using VisitorManagement_API.DTOs.Requests;

namespace VisitorManagement_API.Controllers
{
    [Route("iGuru/Configuration")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly ISourceService _sourceService;
        private readonly IPurposeTypeService _purposeTypeService;

        public ConfigurationController(ISourceService sourceService, IPurposeTypeService purposeTypeService)
        {
            _sourceService = sourceService;
            _purposeTypeService = purposeTypeService;
        }

        // Source Endpoints
        [HttpPost("Sources/AddUpdateSources")]
        public async Task<IActionResult> AddUpdateSource(Source source)
        {
            var response = await _sourceService.AddUpdateSource(source);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("Sources/GetAllSources")]
        public async Task<IActionResult> GetAllSources(GetAllSourcesRequest request)
        {
            var response = await _sourceService.GetAllSources(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("Sources/GetSources/{sourceId}")]
        public async Task<IActionResult> GetSourceById(int sourceId)
        {
            var response = await _sourceService.GetSourceById(sourceId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("Sources/Status/{sourceId}")]
        public async Task<IActionResult> UpdateSourceStatus(int sourceId)
        {
            var response = await _sourceService.UpdateSourceStatus(sourceId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // Purpose Type Endpoints
        [HttpPost("PurposeType/AddUpdatePurposeType")]
        public async Task<IActionResult> AddUpdatePurposeType(PurposeType purposeType)
        {
            var response = await _purposeTypeService.AddUpdatePurposeType(purposeType);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("PurposeType/GetAllPurposeType")]
        public async Task<IActionResult> GetAllPurposeTypes(GetAllPurposeTypeRequest request)
        {
            var response = await _purposeTypeService.GetAllPurposeTypes(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("PurposeType/GetPurposeType/{purposeTypeId}")]
        public async Task<IActionResult> GetPurposeTypeById(int purposeTypeId)
        {
            var response = await _purposeTypeService.GetPurposeTypeById(purposeTypeId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("PurposeType/Status/{purposeTypeId}")]
        public async Task<IActionResult> UpdatePurposeTypeStatus(int purposeTypeId)
        {
            var response = await _purposeTypeService.UpdatePurposeTypeStatus(purposeTypeId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
