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

        [HttpPut("Status/{vehicleId}")]
        public async Task<IActionResult> UpdateVehicleStatus(int vehicleId, [FromBody] UpdateVehicleStatusRequest request)
        {
            try
            {
                // Pass the vehicleId and Reason (if any) to the service
                var response = await _vehiclesService.UpdateVehicleStatus(vehicleId, request.Reason);

                if (response.Success)
                {
                    return StatusCode(response.StatusCode, response);
                }
                else
                {
                    return BadRequest(response.Message);
                }
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

        [HttpGet("GetVehicleType")]
        public async Task<IActionResult> GetVehicleType()
        {
            try
            {
                var response = await _vehiclesService.GetVehicleTypes();
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GetFuelType")]
        public async Task<IActionResult> GetFuelType()
        {
            try
            {
                var response = await _vehiclesService.GetFuelTypes();
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GetDriver")]
        public async Task<IActionResult> GetDriver(GetDriverRequest request)
        {
            try
            {
                var response = await _vehiclesService.GetDriver(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        [HttpPost("GetVehicleSetting")]
        public async Task<IActionResult> GetVehicleSetting([FromBody] GetVehicleSettingRequest request)
        {
            try
            {
                var response = await _vehiclesService.GetVehicleSetting(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("AddRemoveVehicleSetting")]
        public async Task<IActionResult> AddRemoveVehicleSetting([FromBody] AddRemoveVehicleSettingRequest request)
        {
            try
            {
                var response = await _vehiclesService.AddRemoveVehicleSetting(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
