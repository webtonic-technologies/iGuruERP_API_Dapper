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

        [HttpPost]
        public async Task<IActionResult> AddUpdateInstituteDetails([FromBody]InstituteDetailsDTO request)
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

        [HttpGet("InstituteDetails/{id}")]
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

        [HttpGet("InstituteLogo/{id}")]
        public async Task<IActionResult> GetInstituteLogoById(int id)
        {
            try
            {
                var data = await _instituteDetailsServices.GetInstituteLogoById(id);
                return File(data.Data, "image/*");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("InstituteDigitalStamp/{id}")]
        public async Task<IActionResult> GetInstituteDigitalStampById(int id)
        {
            try
            {
                var data = await _instituteDetailsServices.GetInstituteDigitalStampById(id);
                return File(data.Data, "image/*");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("InstituteDigitalSignatory/{id}")]
        public async Task<IActionResult> GetInstituteDigitalSignatoryById(int id)
        {
            try
            {
                var data = await _instituteDetailsServices.GetInstituteDigitalSignatoryById(id);
                return File(data.Data, "image/*");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("InstitutePrincipalSignatory/{id}")]
        public async Task<IActionResult> GetInstitutePrincipalSignatoryById(int id)
        {
            try
            {
                var data = await _instituteDetailsServices.GetInstitutePrincipalSignatoryById(id);
                return File(data.Data, "image/*");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
