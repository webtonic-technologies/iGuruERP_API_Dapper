using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/FeeCollection/ChequeTracking")]
    [ApiController]
    public class ChequeBounceController : ControllerBase
    {
        private readonly IChequeBounceService _chequeBounceService;

        public ChequeBounceController(IChequeBounceService chequeBounceService)
        {
            _chequeBounceService = chequeBounceService;
        }

        /// <summary>
        /// Adds a cheque bounce record.
        /// </summary>
        /// <param name="request">The request containing cheque bounce details.</param>
        /// <returns>A service response indicating success or failure.</returns>
        [HttpPost("AddChequeBounced")]
        public IActionResult AddChequeBounced([FromBody] SubmitChequeBounceRequest request)
        {
            // Validate the request
            if (request == null)
            {
                return BadRequest("Invalid request: Details are required.");
            }

            // Call the service to add the cheque bounce
            var response = _chequeBounceService.AddChequeBounce(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }
    }
}
