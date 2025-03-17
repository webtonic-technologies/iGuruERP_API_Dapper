using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/Configuration/Concession")]
    [ApiController]
    public class ConcessionController : ControllerBase
    {
        private readonly IConcessionService _concessionService;

        public ConcessionController(IConcessionService concessionService)
        {
            _concessionService = concessionService;
        }

        [HttpPost("AddUpdateConcession")]
        public async Task<IActionResult> AddUpdateConcession([FromBody] AddUpdateConcessionRequest request)
        {
            var response = await _concessionService.AddUpdateConcession(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetAllConcessions")]
        public async Task<IActionResult> GetAllConcessions([FromBody] GetAllConcessionRequest request)
        {
            var response = await _concessionService.GetAllConcessions(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetConcessionById/{ConcessionGroupID}")]
        public async Task<IActionResult> GetConcessionById(int ConcessionGroupID)
        {
            var response = await _concessionService.GetConcessionById(ConcessionGroupID);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        //[HttpPut("DeleteConcession/{concessionGroupID}")]
        //public async Task<IActionResult> UpdateConcessionStatus(int concessionGroupID)
        //{
        //    var result = await _concessionService.UpdateConcessionGroupStatus(concessionGroupID);
        //    if (result > 0)
        //    {
        //        return Ok(new { Success = true, Message = "Concession group status updated successfully." });
        //    }
        //    return BadRequest(new { Success = false, Message = "Failed to update concession group status." });
        //}

        //[HttpPut("DeleteConcession")]
        [HttpPut("Status")]
        public async Task<IActionResult> UpdateConcessionStatus([FromBody] ConcessionUpdateRequest request)
        {
            // Use request.ConcessionGroupID and request.InActiveReason
            var result = await _concessionService.UpdateConcessionGroupStatus(request.ConcessionGroupID, request.InActiveReason);

            if (result > 0)
            {
                return Ok(new { Success = true, Message = "Concession group status updated successfully." });
            }
            return BadRequest(new { Success = false, Message = "Failed to update concession group status." });
        }




    }
}
