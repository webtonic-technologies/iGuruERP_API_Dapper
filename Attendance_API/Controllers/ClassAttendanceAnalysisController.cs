using Microsoft.AspNetCore.Mvc;
using Attendance_API.Services.Interfaces;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class ClassAttendanceAnalysisController : ControllerBase
    {
        private readonly IClassAttendanceAnalysisService _classAttendanceAnalysisService;

        public ClassAttendanceAnalysisController(IClassAttendanceAnalysisService classAttendanceAnalysisService)
        {
            _classAttendanceAnalysisService = classAttendanceAnalysisService;
        }

        [HttpPost("GetAttendanceStatistics")]
        public async Task<IActionResult> GetAttendanceStatistics([FromBody] AttendanceFilterParams filterParams)
        {
            try
            {
                var result = await _classAttendanceAnalysisService.GetAttendanceStatistics(filterParams.AcademicYearId, filterParams.ClassId, filterParams.SectionId, filterParams.InstituteId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<AttendanceStatisticsDTO>(false, $"Error: {ex.Message}", null, 500));
            }
        }

        [HttpPost("GetStudentAttendanceAnalysis")]
        public async Task<IActionResult> GetStudentAttendanceAnalysis([FromBody] AttendanceFilterParams filterParams)
        {
            try
            {
                var result = await _classAttendanceAnalysisService.GetStudentAttendanceAnalysis(filterParams.AcademicYearId, filterParams.ClassId, filterParams.SectionId, filterParams.InstituteId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<List<StudentAttendanceAnalysisDTO>>(false, $"Error: {ex.Message}", null, 500));
            }
        }

        [HttpPost("GetStudentDayWiseAttendanceAnalysis")]
        public async Task<IActionResult> GetStudentDayWiseAttendanceAnalysis([FromBody] AttendanceFilterParams filterParams)
        {
            try
            {
                var result = await _classAttendanceAnalysisService.GetStudentDayWiseAttendanceAnalysis(filterParams.AcademicYearId, filterParams.ClassId, filterParams.SectionId, filterParams.InstituteId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<List<StudentDayWiseAttendanceDTO>>(false, $"Error: {ex.Message}", null, 500));
            }
        }

        [HttpPost("GetMonthlyAttendanceAnalysis")]
        public async Task<IActionResult> GetMonthlyAttendanceAnalysis([FromBody] AttendanceFilterParams filterParams)
        {
            try
            {
                var result = await _classAttendanceAnalysisService.GetMonthlyAttendanceAnalysis(filterParams.AcademicYearId, filterParams.ClassId, filterParams.SectionId, filterParams.InstituteId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<List<MonthlyAttendanceAnalysisDTO>>(false, $"Error: {ex.Message}", null, 500));
            }
        }

        [HttpPost("GetAttendanceRangeAnalysis")]
        public async Task<IActionResult> GetAttendanceRangeAnalysis([FromBody] AttendanceFilterParams filterParams)
        {
            try
            {
                var result = await _classAttendanceAnalysisService.GetAttendanceRangeAnalysis(filterParams.AcademicYearId, filterParams.ClassId, filterParams.SectionId, filterParams.InstituteId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<List<AttendanceRangeDTO>>(false, $"Error: {ex.Message}", null, 500));
            }
        }
    }

    
}
