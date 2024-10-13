using Microsoft.AspNetCore.Mvc;
using Transport_API.DTOs.Requests;
using Transport_API.Services.Interfaces;

namespace Transport_API.Controllers
{
    [Route("iGuru/TransportAttendance/TransportAttendance")]
    [ApiController]
    public class TransportAttendanceController : ControllerBase
    {
        private readonly ITransportAttendanceService _transportAttendanceService;

        public TransportAttendanceController(ITransportAttendanceService transportAttendanceService)
        {
            _transportAttendanceService = transportAttendanceService;
        }

        [HttpPost("UpdateTransportAttendance")]
        public async Task<IActionResult> UpdateTransportAttendance(TransportAttendanceRequest request)
        {
            try
            {
                var response = await _transportAttendanceService.AddUpdateTransportAttendance(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GetTransportAttendance")]
        public async Task<IActionResult> GetTransportAttendance(GetTransportAttendanceRequest request)
        {
            try
            {
                var response = await _transportAttendanceService.GetAllTransportAttendance(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //[HttpGet("GetTransportAttendance/{TransportAttendanceId}")]
        //public async Task<IActionResult> GetTransportAttendance(int TransportAttendanceId)
        //{
        //    try
        //    {
        //        var response = await _transportAttendanceService.GetTransportAttendanceById(TransportAttendanceId);
        //        return StatusCode(response.StatusCode, response);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        //[HttpPut("Status/{TransportAttendanceId}")]
        //public async Task<IActionResult> UpdateTransportAttendanceStatus(int TransportAttendanceId)
        //{
        //    try
        //    {
        //        var response = await _transportAttendanceService.UpdateTransportAttendanceStatus(TransportAttendanceId);
        //        return StatusCode(response.StatusCode, response);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}
    }
}
