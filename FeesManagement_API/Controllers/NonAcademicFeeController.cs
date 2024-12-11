using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.DTOs.Responses;
using System.Collections.Generic;

namespace FeesManagement_API.Controllers
{
    //[Route("iGuru/FeeAssignments/NonAcademicFee")]
    [Route("iGuru/FeeCollection/NonAcademicFee")]
    [ApiController]
    public class NonAcademicFeeController : ControllerBase
    {
        private readonly INonAcademicFeeService _service;

        public NonAcademicFeeController(INonAcademicFeeService service)
        {
            _service = service;
        }

        [HttpPost("AddNonAcademicFee")]
        public ActionResult<ServiceResponse<string>> AddNonAcademicFee([FromBody] AddUpdateNonAcademicFeeRequest request)
        {
            var response = _service.AddNonAcademicFee(request);
            return Ok(response);
        }

        [HttpPost("GetNonAcademicFee")]
        public ActionResult<ServiceResponse<List<GetNonAcademicFeeResponse>>> GetNonAcademicFee([FromBody] GetNonAcademicFeeRequest request)
        {
            var response = _service.GetNonAcademicFee(request);
            return Ok(response);
        }

        //[HttpDelete("Delete/{NonAcademicFeesID}")]
        //public ActionResult<ServiceResponse<string>> DeleteNonAcademicFee(int nonAcademicFeesID)
        //{
        //    var response = _service.DeleteNonAcademicFee(nonAcademicFeesID);
        //    return Ok(response);
        //}

        [HttpDelete("Delete/{nonAcademicFeesID}")]
        public ActionResult<ServiceResponse<string>> DeleteNonAcademicFee(int nonAcademicFeesID)
        {
            var response = _service.DeleteNonAcademicFee(nonAcademicFeesID);
            return Ok(response);
        }

        [HttpPost("GetNonAcademicFeeExport")]
        public IActionResult GetNonAcademicFeeExport([FromBody] GetNonAcademicFeeExportRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request: Details are required.");
            }

            try
            {
                var fileData = _service.GetNonAcademicFeeExport(request);

                var contentType = request.ExportType == 1
                    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    : "text/csv";
                var fileName = request.ExportType == 1
                    ? "NonAcademicFeeExport.xlsx"
                    : "NonAcademicFeeExport.csv";

                return File(fileData, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
