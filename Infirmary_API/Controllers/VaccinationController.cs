using Infirmary_API.DTOs.Requests;
using Infirmary_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Infirmary_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class VaccinationController : ControllerBase
    {
        private readonly IVaccinationService _vaccinationService;

        public VaccinationController(IVaccinationService vaccinationService)
        {
            _vaccinationService = vaccinationService;
        }

        [HttpPost("AddUpdateVaccination")]
        public async Task<IActionResult> AddUpdateVaccination(AddUpdateVaccinationRequest request)
        {
            try
            {
                var data = await _vaccinationService.AddUpdateVaccination(request);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPost("GetAllVaccinations")]
        public async Task<IActionResult> GetAllVaccinations(GetAllVaccinationsRequest request)
        {
            try
            {
                var data = await _vaccinationService.GetAllVaccinations(request);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpGet("GetVaccinationById/{VaccinationID}")]
        public async Task<IActionResult> GetVaccinationById(int VaccinationID)
        {
            try
            {
                var data = await _vaccinationService.GetVaccinationById(VaccinationID);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPut("DeleteVaccination/{VaccinationID}")]
        public async Task<IActionResult> DeleteVaccination(int VaccinationID)
        {
            try
            {
                var data = await _vaccinationService.DeleteVaccination(VaccinationID);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
    }
}
