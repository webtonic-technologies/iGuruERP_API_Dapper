using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs;
using Student_API.Services.Interfaces;

namespace Student_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class StudentInformationController : ControllerBase
    {
        private readonly IStudentInformationServices _studentInformationService;
        public StudentInformationController(IStudentInformationServices studentInformationServices)
        {
            _studentInformationService = studentInformationServices;
        }

        [HttpPost]
        [Route("AddUpdateStudentInformation")]
        public async Task<IActionResult> AddUpdateStudentInformation([FromBody] StudentMasterDTO studentMasterDTO)
        {
            try
            {
                var data = await _studentInformationService.AddUpdateStudentInformation(studentMasterDTO);
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

        [HttpGet]
        [Route("GetStudentDetailsById")]
        public async Task<IActionResult> GetStudentDetailsById(int studentId)
        {
            try
            {
                var data = await _studentInformationService.GetStudentDetailsById(studentId);
                if (data.Success)
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
        [HttpGet]
        [Route("GetAllStudentDetails")]
        public async Task<IActionResult> GetAllStudentDetails()
        {
            try
            {
                var data = await _studentInformationService.GetAllStudentDetails();
                if (data.Success)
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

        [HttpPost]
        [Route("ChangeStudentStatus")]
        public async Task<IActionResult> ChangeStudentStatus([FromBody] StudentStatusDTO statusDTO)
        {
            try
            {
                var data = await _studentInformationService.ChangeStudentStatus(statusDTO);
                if (data.Success)
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
