using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VisitorManagement_API.Models;
using VisitorManagement_API.Services.Interfaces;
using VisitorManagement_API.DTOs.Requests;

namespace VisitorManagement_API.Controllers
{
    [Route("iGuru/Appointments")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost("Appointments/AddUpdateAppointments")]
        public async Task<IActionResult> AddUpdateAppointment(Appointment appointment)
        {
            var response = await _appointmentService.AddUpdateAppointment(appointment);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("Appointments/GetAllAppointments")]
        public async Task<IActionResult> GetAllAppointments(GetAllAppointmentsRequest request)
        {
            var response = await _appointmentService.GetAllAppointments(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("Appointments/GetAppointments/{appointmentId}")]
        public async Task<IActionResult> GetAppointmentById(int appointmentId)
        {
            var response = await _appointmentService.GetAppointmentById(appointmentId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("Appointments/Status/{appointmentId}")]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId)
        {
            var response = await _appointmentService.UpdateAppointmentStatus(appointmentId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetAppointmentsExport")]
        public async Task<IActionResult> GetAppointmentsExport([FromBody] GetAppointmentsExportRequest request)
        {
            var response = await _appointmentService.GetAppointmentsExport(request);
            if (response.Success)
            {
                return File(response.Data,
                    response.StatusCode == 200 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv",
                    "AppointmentsExport" + (request.ExportType == 1 ? ".xlsx" : ".csv"));
            }

            return BadRequest(response);
        }
    }
}
