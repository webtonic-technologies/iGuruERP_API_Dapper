using Institute_API.DTOs;
using Institute_API.Repository.Interfaces;
using Institute_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Institute_API.Controllers
{
    [Route("iGuru/Institute/[controller]")]
    [ApiController]
    public class AcaConfigSubStudentController : ControllerBase
    {
        private readonly IAcaConfigSubStudentServices _acaConfigSubStudentServices;

        public AcaConfigSubStudentController(IAcaConfigSubStudentServices acaConfigSubStudentServices)
        {
            _acaConfigSubStudentServices = acaConfigSubStudentServices;
        }

        [HttpPost("AddUpdateSubjectStudentMapping")]
        public async Task<IActionResult> AddUpdateSubjectStudentMapping(AcaConfigSubStudentRequest request)
        {
            try
            {
                var data = await _acaConfigSubStudentServices.AddUpdateSubjectStudentMapping(request);
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

        [HttpPost("GetSubjectStudentMappingList")]
        public async Task<IActionResult> GetSubjectStudentMappingList(MappingListRequest request)
        {
            try
            {
                var data = await _acaConfigSubStudentServices.GetSubjectStudentMappingList(request);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetInstituteStudentsList")]
        public async Task<IActionResult> GetInstituteStudentsList(StudentListRequest request)
        {
            try
            {
                var data = await _acaConfigSubStudentServices.GetInstituteStudentsList(request);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetInstituteSubjectsList/{SubjectType}")]
        public async Task<IActionResult> GetInstituteSubjectsList(int SubjectType)
        {
            try
            {
                var data = await _acaConfigSubStudentServices.GetInstituteSubjectsList(SubjectType);
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
