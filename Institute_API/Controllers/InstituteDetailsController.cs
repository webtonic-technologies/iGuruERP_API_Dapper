using Institute_API.DTOs;
using Institute_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Institute_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class InstituteDetailsController : ControllerBase
    {
        private readonly IInstituteDetailsServices _instituteDetailsServices;

        public InstituteDetailsController(IInstituteDetailsServices instituteDetailsServices)
        {
            _instituteDetailsServices = instituteDetailsServices;
        }

        [HttpPost("AddInstituteDetails")]
        public async Task<IActionResult> AddUpdateInstituteDetails([FromBody] InstituteDetailsDTO request)
        {
            try
            {
                var data = await _instituteDetailsServices.AddUpdateInstititeDetails(request);
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

        [HttpGet("GetInstituteDetailsById/{id}")]
        public async Task<IActionResult> GetInstituteDetailsById(int id)
        {
            try
            {
                var data = await _instituteDetailsServices.GetInstituteDetailsById(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("DeleteImage")]
        public async Task<IActionResult> DeleteImage(DeleteImageRequest request)
        {
            try
            {
                var data = await _instituteDetailsServices.DeleteImage(request);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
