using Institute_API.DTOs;
using Institute_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Institute_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class InstituteAffiliationController : ControllerBase
    {
        private readonly IInstituteAffiliationServices _instituteAffiliationServices;

        public InstituteAffiliationController(IInstituteAffiliationServices instituteAffiliationServices)
        {
            _instituteAffiliationServices = instituteAffiliationServices;
        }

        [HttpPost("AddAffiliation")]
        public async Task<IActionResult> AddUpdateInstituteAffiliation([FromBody] AffiliationDTO request)
        {
            try
            {
                var data = await _instituteAffiliationServices.AddUpdateInstituteAffiliation(request);
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

        [HttpGet("GetInstituteAffiliationById/{id}")]
        public async Task<IActionResult> GetInstituteAffiliationById(int id)
        {
            try
            {
                var data = await _instituteAffiliationServices.GetAffiliationInfoById(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetInstituteAffiliationLogoById/{id}")]
        public async Task<IActionResult> GetInstituteAffiliationLogoById(int id)
        {
            try
            {
                var data = await _instituteAffiliationServices.GetAffiliationLogoById(id);
                return File(data.Data, "image/*");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddAffiliationLogo")]
        public async Task<IActionResult> AddUpdateLogo([FromForm] AffiliationLogoDTO request)
        {
            try
            {
                var data = await _instituteAffiliationServices.AddUpdateLogo(request);
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
