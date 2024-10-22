using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.DTOs.Responses;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/FeeAssignments/ConcessionMapping")]
    [ApiController]
    public class ConcessionMappingController : ControllerBase
    {
        private readonly IConcessionMappingService _concessionMappingService;

        public ConcessionMappingController(IConcessionMappingService concessionMappingService)
        {
            _concessionMappingService = concessionMappingService;
        }

        [HttpPost("AddUpdateConcession")]
        public ActionResult<ServiceResponse<string>> AddUpdateConcession([FromBody] AddUpdateConcessionMappingRequest request) // Update to use the correct request type
        {
            var response = _concessionMappingService.AddUpdateConcession(request);
            return Ok(response);
        }

        [HttpPost("GetAllConcessionMapping")]
        public ActionResult<ServiceResponse<List<GetAllConcessionMappingResponse>>> GetAllConcessionMapping([FromBody] GetAllConcessionMappingRequest request)
        {
            var response = _concessionMappingService.GetAllConcessionMapping(request);
            return Ok(response);
        }

        [HttpPut("Status/{StudentConcessionID}")]
        public ActionResult<ServiceResponse<string>> UpdateStatus(int StudentConcessionID)
        {
            var response = _concessionMappingService.UpdateStatus(StudentConcessionID);
            return Ok(response);
        }
    }
}
