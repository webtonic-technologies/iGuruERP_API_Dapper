using Microsoft.AspNetCore.Mvc;
using Attendance_SE_API.Services.Interfaces;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;
using System.Threading.Tasks;

namespace Attendance_SE_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class SubjectAttendanceAnalysisController : ControllerBase
    {
        private readonly ISubjectAttendanceAnalysisService _subjectAttendanceAnalysisService;

        public SubjectAttendanceAnalysisController(ISubjectAttendanceAnalysisService subjectAttendanceAnalysisService)
        {
            _subjectAttendanceAnalysisService = subjectAttendanceAnalysisService;
        }

        [HttpPost("GetStudentAttendanceStatisticsForSubject")]
        public async Task<ActionResult<ServiceResponse<SubjectAttendanceStatisticsResponse>>> GetStudentAttendanceStatisticsForSubject([FromBody] SubjectAttendanceAnalysisRequest request)
        {
            var response = await _subjectAttendanceAnalysisService.GetStudentAttendanceStatisticsForSubject(request);
            return Ok(response);
        }

        [HttpPost("GetMonthlyAttendanceAnalysisForSubject")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<MonthlyAttendanceSubjectAnalysisResponse>>>> GetMonthlyAttendanceAnalysisForSubject([FromBody] SubjectAttendanceAnalysisRequest request)
        {
            var response = await _subjectAttendanceAnalysisService.GetMonthlyAttendanceAnalysisForSubject(request);
            return Ok(response);
        }

        [HttpPost("GetSubjectsAttendanceAnalysis")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<SubjectsAttendanceAnalysisResponse>>>> GetSubjectsAttendanceAnalysis([FromBody] SubjectAttendanceAnalysisRequest1 request)
        {
            var response = await _subjectAttendanceAnalysisService.GetSubjectsAttendanceAnalysis(request);
            return Ok(response);
        }
         

    }
}
