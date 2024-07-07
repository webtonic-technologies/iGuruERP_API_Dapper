using Communication_API.DTOs.Requests.Configuration;
using Communication_API.Services.Interfaces.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Communication_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public ConfigurationController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost("AddUpdateGroup")]
        public async Task<IActionResult> AddUpdateGroup([FromBody] AddUpdateGroupRequest request)
        {
            var response = await _groupService.AddUpdateGroup(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllGroup")]
        public async Task<IActionResult> GetAllGroup([FromBody] GetAllGroupRequest request)
        {
            var response = await _groupService.GetAllGroup(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetGroup/{GroupID}")]
        public async Task<IActionResult> GetGroup(int GroupID)
        {
            var response = await _groupService.GetGroup(GroupID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("DeleteGroup/{GroupID}")]
        public async Task<IActionResult> DeleteGroup(int GroupID)
        {
            var response = await _groupService.DeleteGroup(GroupID);
            return StatusCode(response.StatusCode, response);
        }
    }
}
