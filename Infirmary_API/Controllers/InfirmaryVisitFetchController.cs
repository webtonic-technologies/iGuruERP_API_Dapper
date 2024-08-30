using Infirmary_API.Services.Interfaces;
using InfirmaryVisit_API.DTOs.Requests;
using InfirmaryVisit_API.DTOs.Response;
using InfirmaryVisit_API.DTOs.ServiceResponse;
using InfirmaryVisit_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InfirmaryVisit_API.Controllers
{
    [Route("iGuru/InfirmaryVisit/InfirmaryVisit_DLL")]
    [ApiController]
    public class InfirmaryVisitFetchController : ControllerBase
    {
        private readonly IInfirmaryVisitFetchService _infirmaryVisitFetchService;

        public InfirmaryVisitFetchController(IInfirmaryVisitFetchService infirmaryVisitFetchService)
        {
            _infirmaryVisitFetchService = infirmaryVisitFetchService;
        }

        [HttpPost("GetAllStudentInfo_DDL")]
        public async Task<IActionResult> GetAllStudentInfo_DDL(GetStudentInfoFetchRequest request)
        {
            var response = await _infirmaryVisitFetchService.GetAllStudentInfoFetch(request.InstituteID);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
    }
}
