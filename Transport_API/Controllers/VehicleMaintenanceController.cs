using Microsoft.AspNetCore.Mvc;
using Transport_API.DTOs.Requests;
using Transport_API.Services.Interfaces;

namespace Transport_API.Controllers
{
    [Route("iGuru/VehicleMaintenance/VehicleMaintenance")]
    [ApiController]
    public class VehicleMaintenanceController : ControllerBase
    {
        private readonly IVehicleMaintenanceService _vehicleMaintenanceService;

        public VehicleMaintenanceController(IVehicleMaintenanceService vehicleMaintenanceService)
        {
            _vehicleMaintenanceService = vehicleMaintenanceService;
        }

        [HttpPost("AddExpense")]
        public async Task<IActionResult> AddExpense(VehicleExpenseRequest request)
        {
            try
            {
                var response = await _vehicleMaintenanceService.AddUpdateVehicleExpense(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GetAllExpense")]
        public async Task<IActionResult> GetAllExpense(GetAllExpenseRequest request)
        {
            try
            {
                var response = await _vehicleMaintenanceService.GetAllVehicleExpenses(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetExpense/{VehicleId}")]
        public async Task<IActionResult> GetExpense(int VehicleId)
        {
            try
            {
                var response = await _vehicleMaintenanceService.GetVehicleExpenseById(VehicleId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("Delete/{VehicleExpenseId}")]
        public async Task<IActionResult> DeleteVehicleExpense(int VehicleExpenseId)
        {
            try
            {
                var response = await _vehicleMaintenanceService.DeleteVehicleExpense(VehicleExpenseId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
