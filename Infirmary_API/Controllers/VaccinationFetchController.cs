using Infirmary_API.DTOs.Requests;
using Infirmary_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Infirmary_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class VaccinationFetchController : ControllerBase
    {
        private readonly IVaccinationFetchService _vaccinationFetchService;

        public VaccinationFetchController(IVaccinationFetchService vaccinationFetchService)
        {
            _vaccinationFetchService = vaccinationFetchService;
        }

        [HttpPost("GetAllVaccinations_Fetch")]
        public async Task<IActionResult> GetAllVaccinationsFetch(GetAllVaccinationsFetchRequest request)
        {
            try
            {
                var data = await _vaccinationFetchService.GetAllVaccinationsFetch(request);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return NotFound(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
    }
}
