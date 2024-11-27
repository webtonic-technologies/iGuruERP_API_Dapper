using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/FeeCollection/ChequeTracking")]
    [ApiController]
    public class ChequeTrackingController : ControllerBase
    {
        private readonly IChequeTrackingService _chequeTrackingService;

        public ChequeTrackingController(IChequeTrackingService chequeTrackingService)
        {
            _chequeTrackingService = chequeTrackingService;
        }

        /// <summary>
        /// Retrieves cheque tracking information.
        /// </summary>
        /// <param name="request">The request containing tracking details.</param>
        /// <returns>A service response with cheque tracking data.</returns>
        [HttpPost("GetChequeTracking")]
        public IActionResult GetChequeTracking([FromBody] GetChequeTrackingRequest request)
        {
            // Validate the request
            if (request == null)
            {
                return BadRequest("Invalid request: Details are required.");
            }

            // Call the service to retrieve cheque tracking
            var response = _chequeTrackingService.GetChequeTracking(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }
    }
}
