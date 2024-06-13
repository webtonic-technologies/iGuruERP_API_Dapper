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
        public async Task<IActionResult> GetAllStudentDetails(int Institute_id, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                var data = await _studentInformationService.GetAllStudentDetails(Institute_id,pageNumber,pageSize);
				return Ok(data);
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

        [HttpPost]
        [Route("AddUpdateStudentOtherInfo")]
        public async Task<IActionResult> AddUpdateStudentOtherInfo([FromBody] StudentOtherInfoDTO otherInfoDTO)
        {
            try
            {
                var data = await _studentInformationService.AddUpdateStudentOtherInfo(otherInfoDTO);
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
        [Route("AddUpdateStudentParentInfo")]
        public async Task<IActionResult> AddUpdateStudentParentInfo([FromBody] StudentParentInfoDTO parentInfoDTO)
        {
            try
            {
                var data = await _studentInformationService.AddUpdateStudentParentInfo(parentInfoDTO);
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
        [Route("AddOrUpdateStudentSiblings")]
        public async Task<IActionResult> AddOrUpdateStudentSiblings([FromBody] StudentSiblings siblingsDTO)
        {
            try
            {
                var data = await _studentInformationService.AddOrUpdateStudentSiblings(siblingsDTO);
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
        [Route("AddOrUpdateStudentPreviousSchool")]
        public async Task<IActionResult> AddOrUpdateStudentPreviousSchool([FromBody] StudentPreviousSchool previousSchoolDTO)
        {
            try
            {
                var data = await _studentInformationService.AddOrUpdateStudentPreviousSchool(previousSchoolDTO);
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
        [Route("AddOrUpdateStudentHealthInfo")]
        public async Task<IActionResult> AddOrUpdateStudentHealthInfo([FromBody] StudentHealthInfo healthInfoDTO)
        {
            try
            {
                var data = await _studentInformationService.AddOrUpdateStudentHealthInfo(healthInfoDTO);
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
        [Route("AddUpdateStudentDocuments")]
        public async Task<IActionResult> AddUpdateStudentDocuments([FromBody] StudentDocumentsDTO documentsDTO)
        {
            try
            {
                var data = await _studentInformationService.AddUpdateStudentDocuments(documentsDTO);
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