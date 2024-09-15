using Institute_API.DTOs;
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
        [HttpPost("DownloadExcel")]
        public async Task<IActionResult> DownloadExcelSheet(ExcelDownloadSubStudentMappingRequest request)
        {
            var response = await _acaConfigSubStudentServices.DownloadExcelSheet(request);
            if (response.Success)
            {
                return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Subject Mapping.xlsx");
            }
            return StatusCode(response.StatusCode, response.Message);
        }
        [HttpPost("GetSubjectsAndStudentMappings")]
        public async Task<IActionResult> GetSubjectsAndStudentMappings(MappingListRequest request)
        {
            try
            {
                var data = await _acaConfigSubStudentServices.GetSubjectsAndStudentMappings(request);
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
