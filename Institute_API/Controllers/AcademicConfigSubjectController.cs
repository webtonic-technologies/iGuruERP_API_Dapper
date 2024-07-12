using Institute_API.DTOs;
using Institute_API.Repository.Interfaces;
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
    }
}
