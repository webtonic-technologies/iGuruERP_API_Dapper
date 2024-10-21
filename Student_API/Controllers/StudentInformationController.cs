using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.Services.Implementations;
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
        [Route("StudentInformation/AddUpdateStudent")]
        public async Task<IActionResult> AddUpdateStudent([FromBody] StudentDTO studentDTO)
        {
            try
            {
                var data = await _studentInformationService.AddUpdateStudent(studentDTO);
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

        [HttpPost]
        [Route("StudentInformation/AddUpdateStudentInformation")]
        public async Task<IActionResult> AddUpdateStudentInformation([FromBody] StudentMasters studentMasterDTO)
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

        [HttpPost]
        [Route("StudentDetails/GetStudentDetailsById")]
        public async Task<IActionResult> GetStudentDetailsById(GetCommonIdRequestModel obj)
        {
            try
            {
                var data = await _studentInformationService.GetStudentDetailsById(obj.id);
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
        [Route("StudentDetails/GetAllStudentDetails")]
        public async Task<IActionResult> GetAllStudentDetails(GetStudentRequestModel obj)
        {
            try
            {
                obj.sortField = obj.sortField ?? "Student_Name";
                obj.sortDirection = obj.sortDirection?? "ASC";
                var data = await _studentInformationService.GetAllStudentDetails(obj);
				return Ok(data);
			}
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("StudentDetails/GetAllStudentDetailsData")]
        public async Task<IActionResult> GetAllStudentDetailsData(GetStudentRequestModel obj)
        {
            try
            {
                obj.sortField = obj.sortField ?? "First_Name";
                obj.sortDirection = obj.sortDirection ?? "ASC";
                var data = await _studentInformationService.GetAllStudentDetailsData(obj);
                return Ok(data);
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("StudentDetails/GetAllStudentDetailsData1")]
        public async Task<IActionResult> GetAllStudentDetailsData1(GetStudentRequestModel obj)
        {
            try
            {
                obj.sortField = obj.sortField ?? "First_Name";
                obj.sortDirection = obj.sortDirection ?? "ASC";
                var data = await _studentInformationService.GetAllStudentDetailsData1(obj);
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
        public async Task<IActionResult> AddUpdateStudentOtherInfo([FromBody] StudentOtherInfos otherInfoDTO)
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
        public async Task<IActionResult> AddUpdateStudentParentInfo([FromBody] StudentParentInfo parentInfoDTO)
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
        public async Task<IActionResult> AddOrUpdateStudentSiblings([FromBody] StudentSibling siblingsDTO)
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
        public async Task<IActionResult> AddOrUpdateStudentPreviousSchool([FromBody] StudentPreviousSchools previousSchoolDTO)
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
        public async Task<IActionResult> AddOrUpdateStudentHealthInfo([FromBody] StudentHealthInfos healthInfoDTO)
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

        [HttpPost("DownloadStudentDetails")]
        public async Task<IActionResult> DownloadStudentDetails(getStudentRequest obj)
        {
            var response = await _studentInformationService.GetAllStudentDetailsAsExcel(obj);

            if (response.Success)
            {
                var filePath = response.Data;
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                // Return the Excel file for download
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(filePath));
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
        [HttpPost("AddUpdateStudentSetting")]
        public async Task<IActionResult> AddUpdateStudentSetting(StudentSettingDTO studentSettingDto)
        {
            try
            {
                var response = await _studentInformationService.AddUpdateStudentSetting(studentSettingDto);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetStudentSettingByInstituteId/{instituteId}")]
        public async Task<IActionResult> GetStudentSettingByInstituteId(int instituteId)
        {
            try
            {
                var response = await _studentInformationService.GetStudentSettingByInstituteId(instituteId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}