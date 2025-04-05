using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Services.Implementations;

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
        [HttpPost("GetChequeTrackingPending")]
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

        [HttpPost("GetChequeTrackingBounced")]
        public IActionResult GetChequeTrackingBounced([FromBody] GetChequeTrackingBouncedRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request: Details are required.");
            }

            var response = _chequeTrackingService.GetChequeTrackingBounced(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }

        [HttpPost("GetChequeTrackingCleared")]
        public IActionResult GetChequeTrackingCleared([FromBody] GetChequeTrackingClearedRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request: Details are required.");
            }

            var response = _chequeTrackingService.GetChequeTrackingCleared(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }


        [HttpPost("GetChequeTrackingStatus")]
        public IActionResult GetChequeTrackingStatus()
        {
            var response = _chequeTrackingService.GetChequeTrackingStatus();
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }

        [HttpPost("GetChequeTrackingPendingExport")]
        public IActionResult GetChequeTrackingExport([FromBody] ChequeTrackingExportRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request: Details are required.");
            }

            try
            {
                var fileData = _chequeTrackingService.GetChequeTrackingExport(request);

                var contentType = request.ExportType == 1 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv";
                var fileName = request.ExportType == 1 ? "ChequeTracking.xlsx" : "ChequeTracking.csv";

                return File(fileData, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("GetChequeTrackingBouncedExport")]
        public IActionResult GetChequeTrackingBouncedExport([FromBody] ChequeTrackingExportBouncedRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request: Details are required.");
            }

            try
            {
                var fileData = _chequeTrackingService.GetChequeTrackingBouncedExport(request);

                var contentType = request.ExportType == 1 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv";
                var fileName = request.ExportType == 1 ? "ChequeTrackingBounced.xlsx" : "ChequeTrackingBounced.csv";

                return File(fileData, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("GetChequeTrackingClearedExport")]
        public IActionResult GetChequeTrackingClearedExport([FromBody] ChequeTrackingExportClearedRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request: Details are required.");
            }

            try
            {
                var fileData = _chequeTrackingService.GetChequeTrackingClearedExport(request);

                var contentType = request.ExportType == 1 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv";
                var fileName = request.ExportType == 1 ? "ChequeTrackingCleared.xlsx" : "ChequeTrackingCleared.csv";

                return File(fileData, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("AddChequeBounced")]
        public IActionResult AddChequeBounced([FromBody] SubmitChequeBounceRequest request)
        {
            // Validate the request
            if (request == null)
            {
                return BadRequest("Invalid request: Details are required.");
            }

            // Call the service to add the cheque bounce
            var response = _chequeTrackingService.AddChequeBounce(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }

        [HttpPost("AddChequeClearance")]
        public IActionResult AddChequeClearance([FromBody] SubmitChequeClearanceRequest request)
        {
            if (request == null || request.TransactionID <= 0)
            {
                return BadRequest("Invalid request: Transaction ID is required.");
            }

            var response = _chequeTrackingService.AddChequeClearance(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }

    }
}
