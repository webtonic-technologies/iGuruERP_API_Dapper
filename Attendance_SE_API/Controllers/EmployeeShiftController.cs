using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Attendance_SE_API.Controllers
{
    [Route("iGuru/Employee/Configuration")]
    [ApiController]
    public class EmployeeShiftController : ControllerBase
    {
        private readonly IEmployeeShiftService _employeeShiftService;

        public EmployeeShiftController(IEmployeeShiftService employeeShiftService)
        {
            _employeeShiftService = employeeShiftService;
        }

        //[HttpPost("AddUpdateShift")]
        //public async Task<IActionResult> AddUpdateShift([FromBody] ShiftRequest request)
        //{
        //    //if (request.Designations == null || !request.Designations.Any())
        //    //{
        //    //    return BadRequest("Designations list is required.");
        //    //}

        //    var response = await _employeeShiftService.AddUpdateShift(request);
        //    return StatusCode(response.StatusCode, response);
        //}

        [HttpPost("AddUpdateShift")]
        public async Task<IActionResult> AddUpdateShift([FromBody] List<ShiftRequest> requests)
        {
            if (requests == null || !requests.Any())
            {
                return BadRequest("Shift list is required.");
            }

            var response = await _employeeShiftService.AddUpdateShift(requests);  // Pass the list of shifts to the service
            return StatusCode(response.StatusCode, response);
        }




        [HttpPost("GetAllShifts")]
        public async Task<IActionResult> GetAllShifts([FromBody] GetAllShiftsRequest request)
        {
            var response = await _employeeShiftService.GetAllShifts(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetShifts/{ShiftID}")]
        public async Task<IActionResult> GetShifts(int ShiftID)
        {
            var response = await _employeeShiftService.GetShiftById(ShiftID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("DeleteShifts/{ShiftID}")]
        public async Task<IActionResult> DeleteShifts(int ShiftID)
        {
            var response = await _employeeShiftService.DeleteShift(ShiftID);
            return StatusCode(response.StatusCode, response);
        }
    }
}
