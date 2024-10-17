using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.Services.Interfaces;

namespace TimeTable_API.Controllers
{
    [Route("iGuru/TimeTable/ClassWise")]
    [ApiController]
    public class ClassWiseController : ControllerBase
    {
        private readonly IClassWiseService _classWiseService;

        public ClassWiseController(IClassWiseService classWiseService)
        {
            _classWiseService = classWiseService;
        }

        [HttpPost("Get")]
        public async Task<IActionResult> GetClassWiseTimeTables([FromBody] ClassWiseRequest request)
        {
            var result = await _classWiseService.GetClassWiseTimeTables(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetTimeTables")]
        public async Task<IActionResult> GetClassWiseTimeTables([FromBody] GetClassWiseTimeTablesRequest request)
        {
            var result = await _classWiseService.GetClassWiseTimeTables(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}
