using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Hostel/BedInformation")]
    [ApiController]
    public class BedInformationController : ControllerBase
    {
        private readonly IBedInformationService _bedInformationService;

        public BedInformationController(IBedInformationService bedInformationService)
        {
            _bedInformationService = bedInformationService;
        }

        [HttpPost("GetBedInformation")]
        public async Task<IActionResult> GetBedInformation([FromBody] GetBedInformationRequest request)
        {
            var response = await _bedInformationService.GetBedInformation(request);

            return Ok(new
            {
                Success = true,
                Message = "Bed Information Retrieved Successfully",
                Data = response
            });
        }
    }
}
