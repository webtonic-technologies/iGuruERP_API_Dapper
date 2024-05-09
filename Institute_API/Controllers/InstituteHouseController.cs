using Institute_API.DTOs;
using Institute_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Institute_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class InstituteHouseController : ControllerBase
    {
        private readonly IInstituteHouseServices _instituteHouseServices;

        public InstituteHouseController(IInstituteHouseServices instituteHouseServices)
        {
            _instituteHouseServices = instituteHouseServices;
        }

        [HttpPost("AddInstituteHouse")]
        public async Task<IActionResult> AddUpdateInstituteHouse([FromBody] InstituteHouseDTO request)
        {
            try
            {
                var data = await _instituteHouseServices.AddUpdateInstituteHouse(request);
                if (data != null)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest("Bad Request");
                }

            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }

        }

        [HttpGet("GetInstituteHouseById/{id}")]
        public async Task<IActionResult> GetInstituteHouseList(int id)
        {
            try
            {
                var data = await _instituteHouseServices.GetInstituteHouseList(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
