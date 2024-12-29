using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lesson_API.Controllers
{
    [Route("iGuru/TeacherTracking/TeacherTracking")]
    [ApiController]
    public class TeacherTrackingController : ControllerBase
    {
        private readonly ITeacherTrackingService _teacherTrackingService;

        public TeacherTrackingController(ITeacherTrackingService teacherTrackingService)
        {
            _teacherTrackingService = teacherTrackingService;
        }

        [HttpPost("GetTeacherTracking")]
        public async Task<IActionResult> GetTeacherTracking([FromBody] GetTeacherTrackingRequest request)
        {
            var response = await _teacherTrackingService.GetTeacherTrackingAsync(request.InstituteID, request.EmployeeID);
            return Ok(response);
        }

        [HttpPost("GetTeacherClassSectionSubject")]
        public async Task<IActionResult> GetTeacherClassSectionSubject([FromBody] GetTeacherClassSectionSubjectRequest request)
        {
            var response = await _teacherTrackingService.GetTeacherClassSectionSubjectAsync(request.EmployeeID);
            return Ok(response);
        }

        [HttpPost("GetChapters")]
        public async Task<IActionResult> GetChapters([FromBody] GetChaptersRequest request)
        {
            var response = await _teacherTrackingService.GetChaptersAsync(request.ClassID, request.SubjectID, request.InstituteID);
            return Ok(response);
        }

        [HttpPost("GetTeacherInfo")]
        public async Task<IActionResult> GetTeacherInfo([FromBody] GetTeacherInfoRequest request)
        {
            var response = await _teacherTrackingService.GetTeacherInfoAsync(request.EmployeeID);
            return Ok(response);
        }
    }
}
