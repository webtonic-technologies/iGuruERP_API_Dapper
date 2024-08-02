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
        [HttpGet("GetInstituteDetails")]
        public async Task<IActionResult> GetInstituteDetails()
        {
            try
            {
                var data = await _instituteDetailsServices.GetAllInstituteDetailsList();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetCountryList")]
        public async Task<IActionResult> GetCountriesAsync()
        {
            try
            {
                var data = await _instituteDetailsServices.GetCountriesAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetStatesByCountryId/{countryId}")]
        public async Task<IActionResult> GetStatesByCountryIdAsync(int countryId)
        {
            try
            {
                var data = await _instituteDetailsServices.GetStatesByCountryIdAsync(countryId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetCitiesByStateId/{stateId}")]
        public async Task<IActionResult> GetCitiesByStateIdAsync(int stateId)
        {
            try
            {
                var data = await _instituteDetailsServices.GetCitiesByStateIdAsync(stateId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetDistrictsByCityId/{cityId}")]
        public async Task<IActionResult> GetDistrictsByCityIdAsync(int cityId)
        {
            try
            {
                var data = await _instituteDetailsServices.GetDistrictsByCityIdAsync(cityId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
