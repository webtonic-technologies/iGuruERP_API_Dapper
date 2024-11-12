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

        [HttpPost("GetConcessionList_Excel")]
        public async Task<IActionResult> GetConcessionList_Excel([FromBody] GetAllConcessionMappingRequest request)
        {
            var response = await _concessionMappingService.GetConcessionListExcel(request);  // Await the service call

            if (!response.Success)  // Check if the service call was successful
            {
                return BadRequest(response);  // Return a BadRequest if the result was not successful
            }

            var fileBytes = response.Data;  // Access the Data (byte array) from the response
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ConcessionList.xlsx");  // Return the file
        }


        [HttpPost("GetConcessionList_CSV")]
        public async Task<IActionResult> GetConcessionList_CSV([FromBody] GetAllConcessionMappingRequest request)
        {
            var response = await _concessionMappingService.GetConcessionListCsv(request);  // Await the service call

            if (!response.Success)  // Check if the service call was successful
            {
                return BadRequest(response);  // Return a BadRequest if the result was not successful
            }

            var fileBytes = response.Data;  // Access the Data (byte array) from the response
            return File(fileBytes, "text/csv", "ConcessionList.csv");  // Return the file
        }



        [HttpPut("Status/{StudentConcessionID}")]
        public ActionResult<ServiceResponse<string>> UpdateStatus(int StudentConcessionID)
        {
            var response = _concessionMappingService.UpdateStatus(StudentConcessionID);
            return Ok(response);
        }

        [HttpPost("GetConcessionList")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<ConcessionListResponse>>>> GetConcessionList([FromBody] ConcessionListRequest request)
        {
            var response = await _concessionMappingService.GetConcessionList(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
