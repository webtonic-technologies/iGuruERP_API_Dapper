using Infirmary_API.DTOs.Requests;
using Infirmary_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Infirmary_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class StudentVaccinationController : ControllerBase
    {
        private readonly IStudentVaccinationService _studentVaccinationService;

        public StudentVaccinationController(IStudentVaccinationService studentVaccinationService)
        {
            _studentVaccinationService = studentVaccinationService;
        }

        [HttpPost("AddUpdateStudentVaccination")]
        public async Task<IActionResult> AddUpdateStudentVaccination(AddUpdateStudentVaccinationRequest request)
        {
            try
            {
                var data = await _studentVaccinationService.AddUpdateStudentVaccination(request);
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

        [HttpPost("GetAllStudentVaccinations")]
        public async Task<IActionResult> GetAllStudentVaccinations(GetAllStudentVaccinationsRequest request)
        {
            try
            {
                var data = await _studentVaccinationService.GetAllStudentVaccinations(request);
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

        [HttpGet("GetStudentVaccinationById/{StudentVaccinationID}")]
        public async Task<IActionResult> GetStudentVaccinationById(int StudentVaccinationID)
        {
            try
            {
                var data = await _studentVaccinationService.GetStudentVaccinationById(StudentVaccinationID);
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

        [HttpPut("DeleteStudentVaccination/{StudentVaccinationID}")]
        public async Task<IActionResult> DeleteStudentVaccination(int StudentVaccinationID)
        {
            try
            {
                var data = await _studentVaccinationService.DeleteStudentVaccination(StudentVaccinationID);
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
