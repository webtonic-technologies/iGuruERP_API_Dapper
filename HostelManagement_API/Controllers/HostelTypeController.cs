using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Hostel/Hostel/[controller]")]
    [ApiController]
    public class HostelTypeController : ControllerBase
    {
        private readonly IHostelTypeService _hostelTypeService;
        private readonly ILogger<HostelTypeController> _logger;

        public HostelTypeController(IHostelTypeService hostelTypeService, ILogger<HostelTypeController> logger)
        {
            _hostelTypeService = hostelTypeService;
            _logger = logger;
        }

        [HttpPost("GetAllHostelType")]
        public async Task<IActionResult> GetAllHostelTypes()
        {
            _logger.LogInformation("GetAllHostelType Request Received");
            var hostelTypes = await _hostelTypeService.GetAllHostelTypes();
            _logger.LogInformation("GetAllHostelType Response: {@Response}", hostelTypes);
            return Ok(new { Success = true, Message = "Hostel types retrieved successfully", Data = hostelTypes });
        }
    }
}
