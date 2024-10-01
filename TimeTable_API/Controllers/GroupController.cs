using Microsoft.AspNetCore.Mvc;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.Services.Interfaces;

namespace TimeTable_API.Controllers
{
    [Route("iGuru/Configuration/Group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost("AddUpdateGroup")]
        public async Task<IActionResult> AddUpdateGroup([FromBody] AddUpdateGroupRequest request)
        {
            var response = await _groupService.AddUpdateGroup(request);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }


        [HttpPost("GetAllGroups")]
        public async Task<IActionResult> GetAllGroups([FromBody] GetAllGroupsRequest request)
        {
            var response = await _groupService.GetAllGroups(request);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("GetGroupById/{GroupID}")]
        public async Task<IActionResult> GetGroupById(int GroupID)
        {
            var response = await _groupService.GetGroupById(GroupID);
            if (response.Success)
                return Ok(response);
            return NotFound(response);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            var response = await _groupService.DeleteGroup(groupId);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
