using Microsoft.AspNetCore.Mvc;
using Infirmary_API.Services.Interfaces;
using Infirmary_API.DTOs.Response;

namespace Infirmary_API.Controllers
{
    [ApiController]
    [Route("api/StudentVaccination")]
    public class StudentVaccinationFetchController : ControllerBase
    {
        private readonly IStudentVaccinationFetchService _service;

        public StudentVaccinationFetchController(IStudentVaccinationFetchService service)
        {
            _service = service;
        }

        [HttpGet("GetAcademicYearFetch")]
        public async Task<IActionResult> GetAcademicYearFetch()
        {
            var response = await _service.GetAcademicYearFetch();
            if (response.Success)
                return Ok(response);
            return StatusCode(response.StatusCode, response.Message);
        }

        [HttpGet("GetClassSectionFetch")]
        public async Task<IActionResult> GetClassSectionFetch()
        {
            var response = await _service.GetClassSectionFetch();
            if (response.Success)
                return Ok(response);
            return StatusCode(response.StatusCode, response.Message);
        }
    }
}
