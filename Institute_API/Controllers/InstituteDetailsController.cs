using Institute_API.DTOs;
using Institute_API.Models;
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
        [HttpGet("GetInstituteDetails/{AcademicYear}")]
        public async Task<IActionResult> GetInstituteDetails(int AcademicYear)
        {
            try
            {
                var data = await _instituteDetailsServices.GetAllInstituteDetailsList(AcademicYear);
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
        [HttpGet("GetCitiesByDistrictIdAsync/{districtId}")]
        public async Task<IActionResult> GetCitiesByDistrictIdAsync(int districtId)
        {
            try
            {
                var data = await _instituteDetailsServices.GetCitiesByDistrictIdAsync(districtId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetDistrictsByStateIdAsync/{stateId}")]
        public async Task<IActionResult> GetDistrictsByStateIdAsync(int stateId)
        {
            try
            {
                var data = await _instituteDetailsServices.GetDistrictsByStateIdAsync(stateId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("AcademicYear/{InstituteId}")]
        public async Task<IActionResult> GetAcademicYearList(int InstituteId)
        {
            try
            {
                var data = await _instituteDetailsServices.GetAcademicYearList(InstituteId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("ActiveAcademicYear/{InstituteId}/{AcaInfoYearCode}")]
        public async Task<IActionResult> ActiveAcademicYear(string AcaInfoYearCode, int InstituteId)
        {
            try
            {
                var data = await _instituteDetailsServices.ActiveAcademicYear(AcaInfoYearCode, InstituteId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddOrUpdateAcademicYear")]
        public async Task<IActionResult> AddOrUpdateAcademicYear(AcademicInfo request)
        {
            try
            {
                var data = await _instituteDetailsServices.AddOrUpdateAcademicYear(request);
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
