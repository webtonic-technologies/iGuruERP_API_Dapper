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
        [Route("StudentInformation/AddUpdateStudentInformation")]
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
        [Route("StudentDetails/GetStudentDetailsById")]
        public async Task<IActionResult> GetStudentDetailsById(int id)
        {
            try
            {
                var data = await _studentInformationService.GetStudentDetailsById(id);
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
        [Route("StudentDetails/GetAllStudentDetails")]
        public async Task<IActionResult> GetAllStudentDetails(int Institute_id, string? sortField = "Student_Name", string? sortDirection = "ASC", int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                var data = await _studentInformationService.GetAllStudentDetails(Institute_id, sortField, sortDirection, pageNumber,pageSize);
				return Ok(data);
			}
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("StudentStatus/ChangeStudentStatus")]
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
        [Route("StudentOtherInfo/AddUpdateStudentOtherInfo")]
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
        [Route("StudentParentInfo/AddUpdateStudentParentInfo")]
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
        [Route("StudentSiblings/AddUpdateStudentSiblings")]
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
        [Route("StudentPreviousSchool/AddUpdateStudentPreviousSchool")]
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
        [Route("StudentHealthInfo/AddUpdateStudentHealthInfo")]
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
        [Route("StudentDocuments/AddUpdateStudentDocuments")]
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
        [HttpDelete("StudentDocuments/DeleteStudentDocument/{Student_Documents_id}")]
        public async Task<IActionResult> DeleteStudentDocument(int Student_Documents_id)
        {
            try
            {
                var data = await _studentInformationService.DeleteStudentDocument(Student_Documents_id);
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