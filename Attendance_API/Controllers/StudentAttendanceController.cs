﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Attendance_API.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Attendance_API.DTOs;

namespace Attendance_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class StudentAttendanceController : ControllerBase
    {
        private readonly IStudentAttendanceMasterService _studentAttendanceMasterService;

        public StudentAttendanceController(IStudentAttendanceMasterService studentAttendanceMasterService)
        {
            _studentAttendanceMasterService = studentAttendanceMasterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentAttendanceMasterList([FromQuery] StudentAttendanceMasterRequestDTO request)
        {
            var data = await _studentAttendanceMasterService.GetStudentAttendanceMasterList(request);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdateStudentAttendanceMaster([FromBody] StudentAttendanceMasterDTO studentAttendanceMaster)
        {
            var data = await _studentAttendanceMasterService.InsertOrUpdateStudentAttendanceMaster(studentAttendanceMaster);
            return Ok(data);
        }

        [HttpDelete("{studentAttendanceId}")]
        public async Task<IActionResult> DeleteStudentAttendanceMaster(int studentAttendanceId)
        {
            var data = await _studentAttendanceMasterService.DeleteStudentAttendanceMaster(studentAttendanceId);
            return Ok(data);
        }
    }
}
