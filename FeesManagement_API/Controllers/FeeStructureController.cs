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
    }
}
