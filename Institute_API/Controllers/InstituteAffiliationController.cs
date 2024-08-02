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

        [HttpGet("GetInstituteAffiliationById/{Instituteid}")]
        public async Task<IActionResult> GetInstituteAffiliationById(int Instituteid)
        {
            try
            {
                var data = await _instituteAffiliationServices.GetAffiliationInfoById(Instituteid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
