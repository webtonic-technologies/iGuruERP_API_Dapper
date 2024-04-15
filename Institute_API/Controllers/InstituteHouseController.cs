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

        [HttpPost]
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

        [HttpGet("InstituteHouse/{id}")]
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

        [HttpGet("InstituteHouseLogo/{id}")]
        public async Task<IActionResult> GetInstituteHouseLogoById(int id)
        {
            try
            {
                var data = await _instituteHouseServices.GetInstituteHouseLogoById(id);
                return File(data.Data, "image/*");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("HouseFile")]
        public async Task<IActionResult> AddUpdateInstituteHouseFile([FromForm] HoueseFile request)
        {
            try
            {
                var data = await _instituteHouseServices.AddUpdateHouseFile(request);
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

    }
}
