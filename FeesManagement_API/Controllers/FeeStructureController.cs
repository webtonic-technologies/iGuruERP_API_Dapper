using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Services.Interfaces;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/FeeAssignments/FeeStructure")]
    [ApiController]
    public class FeeStructureController : ControllerBase
    {
        private readonly IFeeStructureService _feeStructureService;

        public FeeStructureController(IFeeStructureService feeStructureService)
        {
            _feeStructureService = feeStructureService;
        }

        [HttpPost("GetFeeStructure")]
        public IActionResult GetFeeStructure([FromBody] FeeStructureRequest request)
        {
            var response = _feeStructureService.GetFeeStructure(request);
            return Ok(response);
        }

        [HttpPost("GetFeeStructure_Excel")]
        public async Task<IActionResult> GetFeeStructure_Excel([FromBody] FeeStructureRequest request)
        {
            var response = await _feeStructureService.GetFeeStructureExcel(request);  // Service call to handle Excel logic

            if (!response.Success)
            {
                return BadRequest(response);
            }

            var fileBytes = response.Data;  // Retrieve the Excel file data as byte array
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FeeStructure.xlsx");
        }

        [HttpPost("GetFeeStructure_CSV")]
        public async Task<IActionResult> GetFeeStructure_CSV([FromBody] FeeStructureRequest request)
        {
            var response = await _feeStructureService.GetFeeStructureCSV(request);  // Service call to handle CSV logic

            if (!response.Success)
            {
                return BadRequest(response);
            }

            var fileBytes = response.Data;  // Retrieve the CSV file data as a byte array
            return File(fileBytes, "text/csv", "FeeStructure.csv");
        }


    }
}
