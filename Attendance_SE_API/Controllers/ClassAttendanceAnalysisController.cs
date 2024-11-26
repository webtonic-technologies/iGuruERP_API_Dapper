using Microsoft.AspNetCore.Mvc;
using Attendance_SE_API.Services.Interfaces;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.DTOs.Requests;
using System.Threading.Tasks;
using Attendance_SE_API.ServiceResponse;


namespace Attendance_SE_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class ClassAttendanceAnalysisController : ControllerBase
    {
        private readonly IClassAttendanceAnalysisService _attendanceAnalysisService;

        public ClassAttendanceAnalysisController(IClassAttendanceAnalysisService attendanceAnalysisService)
        {
            _attendanceAnalysisService = attendanceAnalysisService;
        }

        [HttpPost("GetStudentAttendanceStatistics")]
        public async Task<ActionResult<ServiceResponse<AttendanceStatisticsResponse>>> GetStudentAttendanceStatistics([FromBody] ClassAttendanceAnalysisRequest request)
        {
            var response = await _attendanceAnalysisService.GetStudentAttendanceStatistics(request);
            return Ok(response);
        }

        [HttpPost("GetMonthlyAttendanceAnalysis")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<MonthlyAttendanceAnalysisResponse>>>> GetMonthlyAttendanceAnalysis([FromBody] ClassAttendanceAnalysisRequest request)
        {
            var response = await _attendanceAnalysisService.GetMonthlyAttendanceAnalysis(request);
            return Ok(response);
        }

        [HttpPost("GetAttendanceRangeAnalysis")]
        public async Task<ActionResult<ServiceResponse<AttendanceRangeAnalysisResponse>>> GetAttendanceRangeAnalysis([FromBody] ClassAttendanceAnalysisRequest request)
        {
            var response = await _attendanceAnalysisService.GetAttendanceRangeAnalysis(request);
            return Ok(response);
        }

        [HttpPost("GetStudentDayWiseAttendance")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<StudentDayWiseAttendanceResponse>>>> GetStudentDayWiseAttendance([FromBody] ClassAttendanceAnalysisRequest request)
        {
            var response = await _attendanceAnalysisService.GetStudentDayWiseAttendance(request);
            return Ok(response);
        }

        [HttpPost("GetStudentsAttendanceAnalysis")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<StudentAttendanceAnalysisResponse>>>> GetStudentsAttendanceAnalysis([FromBody] ClassAttendanceAnalysisRequest request)
        {
            var response = await _attendanceAnalysisService.GetStudentsAttendanceAnalysis(request);
            return Ok(response);
        }

    }
}
