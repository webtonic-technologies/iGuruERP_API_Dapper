﻿using Attendance_API.DTOs;
using Attendance_API.Services.Implementations;
using Attendance_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Attendance_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class StudentAttendanceReportController : ControllerBase
    {
        private readonly IStudentAttendanceReportService _studentAttendanceReportService;
        public StudentAttendanceReportController(IStudentAttendanceReportService studentAttendanceReportService)
        {
            _studentAttendanceReportService = studentAttendanceReportService;   
        }

        [HttpPost]
        public async Task<IActionResult> GetStudentAttendanceDatewiseReport(StudentAttendanceDatewiseReportRequestDTO request)
        {
            var data = await _studentAttendanceReportService.GetStudentAttendanceDatewiseReport(request);
            return Ok(data);
        }
    }
}
