using Microsoft.AspNetCore.Mvc;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Requests.Transport_API.DTOs.Requests;
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

        [HttpGet("GetVehicleExpenseType")]
        public async Task<IActionResult> GetVehicleExpenseType()
        {
            try
            {
                var response = await _vehicleMaintenanceService.GetVehicleExpenseType();
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //[HttpPost("GetAllExpenseExport")]
        //public async Task<IActionResult> GetAllExpenseExport([FromBody] GetAllExpenseExportRequest request)
        //{
        //    // Call the service to generate the Excel file
        //    var response = await _vehicleMaintenanceService.GetAllExpenseExport(request);

        //    if (response.Success)
        //    {
        //        // Return the file as a downloadable Excel
        //        return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "VehicleExpenses.xlsx");
        //    }
        //    else
        //    {
        //        return StatusCode(response.StatusCode, response.Message);
        //    }
        //}

        [HttpPost("GetAllExpenseExport")]
        public async Task<IActionResult> GetAllExpenseExport([FromBody] GetAllExpenseExportRequest request)
        {
            // Call the service to generate the file
            var response = await _vehicleMaintenanceService.GetAllExpenseExport(request);

            if (response.Success)
            {
                // If export type is 1 (Excel)
                if (request.ExportType == 1)
                {
                    return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "VehicleExpenses.xlsx");
                }
                // If export type is 2 (CSV)
                else if (request.ExportType == 2)
                {
                    return File(response.Data, "text/csv", "VehicleExpenses.csv");
                }
                else
                {
                    return BadRequest("Invalid Export Type");
                }
            }
            else
            {
                return StatusCode(response.StatusCode, response.Message);
            }
        }

        [HttpPost("AddFuelExpense")]
        public async Task<IActionResult> AddFuelExpense(AddFuelExpenseRequest request)
        {
            try
            {
                var response = await _vehicleMaintenanceService.AddFuelExpense(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GetFuelExpense")]
        public async Task<IActionResult> GetFuelExpense(GetFuelExpenseRequest request)
        {
            try
            {
                var response = await _vehicleMaintenanceService.GetFuelExpense(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("GetFuelExpenseExport")]
        public async Task<IActionResult> GetFuelExpenseExport([FromBody] GetFuelExpenseExportRequest request)
        {
            try
            {
                var response = await _vehicleMaintenanceService.GetFuelExpenseExport(request);

                if (response.Success)
                {
                    // If export type is 1 (Excel)
                    if (request.ExportType == 1)
                    {
                        return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FuelExpenses.xlsx");
                    }
                    // If export type is 2 (CSV)
                    else if (request.ExportType == 2)
                    {
                        return File(response.Data, "text/csv", "FuelExpenses.csv");
                    }
                    else
                    {
                        return BadRequest("Invalid Export Type");
                    }
                }
                else
                {
                    return StatusCode(response.StatusCode, response.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
