using Institute_API.DTOs;
using Institute_API.Services.Implementations;
using Institute_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Institute_API.Controllers
{
    [Route("iGuru/Institute/[controller]")]
    [ApiController]
    public class AcademicConfigSubjectController : ControllerBase
    {
        private readonly IAcademicConfigSubjectsServices _academicConfigSubjectsServices;

        public AcademicConfigSubjectController(IAcademicConfigSubjectsServices academicConfigSubjectsServices)
        {
            _academicConfigSubjectsServices = academicConfigSubjectsServices;
        }

        [HttpPost("AddUpdateAcademicConfigSubject")]
        public async Task<IActionResult> AddUpdateAcademicConfigSubject(SubjectRequest request)
        {
            try
            {
                var data = await _academicConfigSubjectsServices.AddUpdateAcademicConfigSubject(request);
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

        [HttpGet("GetAcademicConfigSubjectById/{SubjectId}")]
        public async Task<IActionResult> GetAcademicConfigSubjectById(int SubjectId)
        {
            try
            {
                var data = await _academicConfigSubjectsServices.GetAcademicConfigSubjectById(SubjectId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetAcademicConfigSubjectList")]
        public async Task<IActionResult> GetAcademicConfigSubjectList(GetAllSubjectRequest request)
        {
            try
            {
                var data = await _academicConfigSubjectsServices.GetAcademicConfigSubjectList(request);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteAcademicConfigSubject/{SubjectId}")]
        public async Task<IActionResult> DeleteAcademicConfigSubject(int SubjectId)
        {
            try
            {
                var data = await _academicConfigSubjectsServices.DeleteAcademicConfigSubject(SubjectId);
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
        [HttpPost("AddUpdateSubjectType")]
        public async Task<IActionResult> AddUpdateSubjectType(SubjectType request)
        {
            try
            {
                var data = await _academicConfigSubjectsServices.AddUpdateSubjectType(request);
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
        [HttpGet("GetSubjectTypeList")]
        public async Task<IActionResult> GetSubjectTypeList()
        {
            try
            {
                var data = await _academicConfigSubjectsServices.GetSubjectTypeList();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("GetSubjectTypeById/{subjectTypeId}")]
        public async Task<IActionResult> GetSubjectTypeById(int subjectTypeId)
        {
            try
            {
                var data = await _academicConfigSubjectsServices.GetSubjectTypeById(subjectTypeId);
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
            var response = await _academicConfigSubjectsServices.DownloadExcelSheet(instituteId);
            if (response.Success)
            {
                return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Subjects.xlsx");
            }
            return StatusCode(response.StatusCode, response.Message);
        }
    }
}
