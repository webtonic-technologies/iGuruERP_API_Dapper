using Communication_API.DTOs.Requests;
using Communication_API.DTOs.Responses;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Communication_API.Controllers
{
    [Route("iGuru/CommunicationUser/User")]
    [ApiController]
    public class CommunicationUserController : ControllerBase
    {
        private readonly ICommunicationUserService _communicationUserService;

        public CommunicationUserController(ICommunicationUserService communicationUserService)
        {
            _communicationUserService = communicationUserService;
        }

        [HttpPost("GetStudentList")]
        public async Task<IActionResult> GetStudentList([FromBody] GetStudentListRequest request)
        {
            var response = await _communicationUserService.GetStudentList(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetEmployeeList")]
        public async Task<IActionResult> GetEmployeeList([FromBody] GetEmployeeListRequest request)
        {
            var response = await _communicationUserService.GetEmployeeList(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
