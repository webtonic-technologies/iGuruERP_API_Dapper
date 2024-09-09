using Institute_API.DTOs;
using Institute_API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Institute_API.Controllers
{
    [Route("iGuru/Institute/[controller]")]
    [ApiController]
    public class AcademicConfigController : ControllerBase
    {
        private readonly IAcademicConfigServices _academicConfigServices;

        public AcademicConfigController(IAcademicConfigServices academicConfigServices)
        {
            _academicConfigServices = academicConfigServices;
        }

        [HttpPost("AddCourseClass")]
        public async Task<IActionResult> AddUpdateAcademicConfig([FromBody] Class request)
        {
            try
            {
                var data = await _academicConfigServices.AddUpdateAcademicConfig(request);
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

        [HttpGet("AcademicConfigCourseClass/{id}")]
        public async Task<IActionResult> GetAcademicConfigById(int id)
        {
            try
            {
                var data = await _academicConfigServices.GetAcademicConfigById(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetAllCourseClassList")]
        public async Task<IActionResult> GetAcademicConfigList(GetAllCourseClassRequest request)
        {
            try
            {
                var data = await _academicConfigServices.GetAcademicConfigList(request);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteCourseClass/{id}")]
        public async Task<IActionResult> DeleteAcademicConfig(int id)
        {
            try
            {
                var data = await _academicConfigServices.DeleteAcademicConfig(id);
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
        [HttpGet("DownloadExcel/{instituteId}")]
        public async Task<IActionResult> DownloadExcelSheet(int instituteId)
        {
            var response = await _academicConfigServices.DownloadExcelSheet(instituteId);
            if (response.Success)
            {
                return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Class/Course.xlsx");
            }
            return StatusCode(response.StatusCode, response.Message);
        }
    }
}
