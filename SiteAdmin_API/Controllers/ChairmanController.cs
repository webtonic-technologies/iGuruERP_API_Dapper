using Microsoft.AspNetCore.Mvc;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Controllers
{
    [Route("iGuru/ControlPanel")]
    [ApiController]
    public class ChairmanController : ControllerBase
    {
        private readonly IChairmanService _chairmanService;

        public ChairmanController(IChairmanService chairmanService)
        {
            _chairmanService = chairmanService;
        }

        [HttpPost("Chairman/AddUpdateChairman")]
        public async Task<IActionResult> AddUpdateChairman([FromBody] AddUpdateChairmanRequest request)
        {
            var response = await _chairmanService.AddUpdateChairman(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("Chairman/GetAllChairman")]
        public async Task<IActionResult> GetAllChairman()
        {
            var response = await _chairmanService.GetAllChairman();
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("Chairman/CreateUserLoginInfo")]
        public async Task<IActionResult> CreateUserLoginInfo(CreateUserRequest request)
        {
            var response = await _chairmanService.CreateUserLoginInfo(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpDelete("Chairman/DeleteChairman")]
        public async Task<IActionResult> DeleteChairman([FromBody] DeleteChairmanRequest request)
        {
            var response = await _chairmanService.DeleteChairman(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetInstitutesDDL")]
        public async Task<IActionResult> GetInstitutesDDL()
        {
            var response = await _chairmanService.GetInstitutesDDL();
            return StatusCode(response.StatusCode, response);
        }
    }
}
