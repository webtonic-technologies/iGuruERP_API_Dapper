using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Services.Interfaces;

namespace StudentManagement_API.Controllers
{
    [Route("iGuru/StudentManagement/StudentInformation")]
    [ApiController]
    public class StudentManagementController : ControllerBase
    {
        private readonly IStudentInformationService _studentInformationService;

        public StudentManagementController(IStudentInformationService studentInformationService)
        {
            _studentInformationService = studentInformationService;
        }

        [HttpPost("AddUpdateStudent")]
        public async Task<IActionResult> AddUpdateStudent(AddUpdateStudentRequest request)
        {
            var response = await _studentInformationService.AddUpdateStudent(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetStudentInformation")]
        public async Task<IActionResult> GetStudentInformation([FromBody] GetStudentInformationRequest request)
        {
            var response = await _studentInformationService.GetStudentInformation(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("SetStudentStatusActivity")]
        public async Task<IActionResult> SetStudentStatusActivity([FromBody] SetStudentStatusActivityRequest request)
        {
            var response = await _studentInformationService.SetStudentStatusActivity(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetStudentStatusActivity")]
        public async Task<IActionResult> GetStudentStatusActivity([FromBody] GetStudentStatusActivityRequest request)
        {
            var response = await _studentInformationService.GetStudentStatusActivity(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("DownloadStudentImportTemplate")]
        public async Task<IActionResult> DownloadStudentImportTemplate([FromBody] DownloadStudentImportTemplateRequest request)
        {
            try
            {
                var response = await _studentInformationService.DownloadStudentImportTemplate(request.InstituteID);

                if (response.Success)
                {
                    return File(response.Data,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "StudentImportTemplate.xlsx");
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        //[HttpPost("StudentInformationImport")]
        //public async Task<IActionResult> StudentInformationImport([FromForm] IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest(new ServiceResponse<string>(false, "No file uploaded", null, 400));

        //    try
        //    {
        //        using (var stream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(stream);
        //            var result = await _studentInformationService.ImportStudentInformation(stream);
        //            return result.Success ? Ok(result) : BadRequest(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ServiceResponse<string>(false, ex.Message, null, 500));
        //    }
        //}

        [HttpPost("StudentInformationImport")]
        public async Task<IActionResult> StudentInformationImport([FromForm] StudentInformationImportRequestDto request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new ServiceResponse<string>(false, "No file uploaded", null, 400));

            try
            {
                using (var stream = new MemoryStream())
                {
                    await request.File.CopyToAsync(stream);

                    // Pass the InstituteID along with the file stream to the service.
                    var result = await _studentInformationService.ImportStudentInformation(request.InstituteID, stream);
                    return result.Success ? Ok(result) : BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponse<string>(false, ex.Message, null, 500));
            }
        }

        [HttpPost("GetStudentSetting")]
        public async Task<IActionResult> GetStudentSetting([FromBody] GetStudentSettingRequest request)
        {
            try
            {
                var response = await _studentInformationService.GetStudentSetting(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddRemoveStudentSetting")]
        public async Task<IActionResult> AddRemoveStudentSetting([FromBody] AddRemoveStudentSettingRequest request)
        {
            try
            {
                var response = await _studentInformationService.AddRemoveStudentSetting(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
