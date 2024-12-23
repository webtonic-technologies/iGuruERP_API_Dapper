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

        [HttpPost("StudentVaccinations/GetStudentVaccinationsExport")]
        public async Task<IActionResult> GetStudentVaccinationsExport([FromBody] GetStudentVaccinationsExportRequest request)
        {
            var response = await _studentVaccinationService.ExportStudentVaccinationsData(request);

            if (response.Success)
            {
                string fileName = "StudentVaccinations_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + (request.ExportType == 1 ? ".xlsx" : ".csv");
                string contentType = request.ExportType == 1 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv";

                return File(response.Data, contentType, fileName);
            }

            return BadRequest(response.Message);
        }
    }
}
