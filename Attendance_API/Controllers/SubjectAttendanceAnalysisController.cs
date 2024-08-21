using Microsoft.AspNetCore.Mvc;
using Attendance_API.Services.Interfaces;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Controllers
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

        [HttpPost("GetAttendanceStatistics")]
        public async Task<IActionResult> GetAttendanceStatistics([FromBody] SubjectAttendanceFilterParams filterParams)
        {
            try
            {
                var result = await _subjectAttendanceAnalysisService.GetAttendanceStatistics(filterParams.AcademicYearId, filterParams.ClassId, filterParams.SectionId, filterParams.SubjectId, filterParams.InstituteId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<AttendanceStatisticsDTO>(false, $"Error: {ex.Message}", null, 500));
            }
        }

        [HttpPost("GetMonthlyAttendanceAnalysis")]
        public async Task<IActionResult> GetMonthlyAttendanceAnalysis([FromBody] SubjectAttendanceFilterParams filterParams)
        {
            try
            {
                var result = await _subjectAttendanceAnalysisService.GetMonthlyAttendanceAnalysis(filterParams.AcademicYearId, filterParams.ClassId, filterParams.SectionId, filterParams.SubjectId, filterParams.InstituteId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<List<MonthlyAttendanceAnalysisDTO>>(false, $"Error: {ex.Message}", null, 500));
            }
        }

        [HttpPost("GetAttendanceRangeAnalysis")]
        public async Task<IActionResult> GetAttendanceRangeAnalysis([FromBody] SubjectAttendanceFilterParams filterParams)
        {
            try
            {
                var result = await _subjectAttendanceAnalysisService.GetAttendanceRangeAnalysis(filterParams.AcademicYearId, filterParams.ClassId, filterParams.SectionId, filterParams.SubjectId, filterParams.InstituteId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<List<AttendanceRangeDTO>>(false, $"Error: {ex.Message}", null, 500));
            }
        }

        [HttpPost("GetStudentAttendanceAnalysis")]
        public async Task<IActionResult> GetStudentAttendanceAnalysis([FromBody] SubjectAttendanceFilterParams filterParams)
        {
            try
            {
                var result = await _subjectAttendanceAnalysisService.GetStudentAttendanceAnalysis(filterParams.AcademicYearId, filterParams.ClassId, filterParams.SectionId, filterParams.SubjectId, filterParams.InstituteId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<List<StudentAttendanceAnalysisDTO>>(false, $"Error: {ex.Message}", null, 500));
            }
        }

        [HttpPost("GetStudentDayWiseAttendanceAnalysis")]
        public async Task<IActionResult> GetStudentDayWiseAttendanceAnalysis([FromBody] SubjectAttendanceFilterParams filterParams)
        {
            try
            {
                var result = await _subjectAttendanceAnalysisService.GetStudentDayWiseAttendanceAnalysis(filterParams.AcademicYearId, filterParams.ClassId, filterParams.SectionId, filterParams.SubjectId, filterParams.InstituteId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<List<StudentDayWiseAttendanceDTO>>(false, $"Error: {ex.Message}", null, 500));
            }
        }
    }
}
