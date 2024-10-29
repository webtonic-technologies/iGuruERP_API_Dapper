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

        [HttpDelete("Delete/{NonAcademicFeesID}")]
        public ActionResult<ServiceResponse<string>> DeleteNonAcademicFee(int nonAcademicFeesID)
        {
            var response = _service.DeleteNonAcademicFee(nonAcademicFeesID);
            return Ok(response);
        }
    }
}
