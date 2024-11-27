using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/FeeCollection/ChequeTracking")]
    [ApiController]
    public class SubmitChequeClearanceController : ControllerBase
    {
        private readonly IChequeClearanceService _chequeClearanceService;

        public SubmitChequeClearanceController(IChequeClearanceService chequeClearanceService)
        {
            _chequeClearanceService = chequeClearanceService;
        }

        [HttpPost("AddChequeClearance")]
        public IActionResult AddChequeClearance([FromBody] SubmitChequeClearanceRequest request)
        {
            if (request == null || request.TransactionID <= 0)
            {
                return BadRequest("Invalid request: Transaction ID is required.");
            }

            var response = _chequeClearanceService.AddChequeClearance(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }
    }
}
