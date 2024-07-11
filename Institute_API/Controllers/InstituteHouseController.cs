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
        [HttpGet("GetInstituteHouse/{id}")]
        public async Task<IActionResult> GetInstituteHouseList(int id, [FromQuery] string searchText)
        {
            try
            {
                var data = await _instituteHouseServices.GetInstituteHouseList(id, searchText);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetInstituteHouseById/{instituteHouseId}")]
        public async Task<IActionResult> GetInstituteHouseById(int instituteHouseId)
        {
            try
            {
                var data = await _instituteHouseServices.GetInstituteHouseById(instituteHouseId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("DeleteInstituteHouse/{instituteHouseId}")]
        public async Task<IActionResult> DeleteInstituteHouse(int instituteHouseId)
        {
            try
            {
                var data = await _instituteHouseServices.SoftDeleteInstituteHouse(instituteHouseId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
