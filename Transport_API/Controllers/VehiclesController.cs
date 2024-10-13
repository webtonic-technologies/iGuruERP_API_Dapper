using Microsoft.AspNetCore.Mvc;
using Transport_API.DTOs.Requests;
using Transport_API.Services.Interfaces;

namespace Transport_API.Controllers
{
    [Route("iGuru/Configuration/Vehicles")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehiclesService _vehiclesService;

        public VehiclesController(IVehiclesService vehiclesService)
        {
            _vehiclesService = vehiclesService;
        }

        [HttpPost("AddUpdateVehicles")]
        public async Task<IActionResult> AddUpdateVehicles(VehicleRequest request)
        {
            try
            {
                var response = await _vehiclesService.AddUpdateVehicle(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GetAllVehicles")]
        public async Task<IActionResult> GetAllVehicles(GetAllVehiclesRequest request)
        {
            try
            {
                var response = await _vehiclesService.GetAllVehicles(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetVehicle/{VehicleId}")]
        public async Task<IActionResult> GetVehicle(int VehicleId)
        {
            try
            {
                var response = await _vehiclesService.GetVehicleById(VehicleId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("Status/{VehicleId}")]
        public async Task<IActionResult> UpdateVehicleStatus(int VehicleId)
        {
            try
            {
                var response = await _vehiclesService.UpdateVehicleStatus(VehicleId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("ExportExcel")]
        public async Task<IActionResult> ExportExcel(GetAllExportVehiclesRequest request)
        {
            var response = await _vehiclesService.ExportExcel(request);
            return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Vehicles.xlsx");
        }

        [HttpPost("ExportCSV")]
        public async Task<IActionResult> ExportCSV(GetAllExportVehiclesRequest request)
        {
            var response = await _vehiclesService.ExportCSV(request);
            return File(response.Data, "text/csv", "Vehicles.csv");
        }


    }
}
