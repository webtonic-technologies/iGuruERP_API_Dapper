using Microsoft.AspNetCore.Mvc;
using Student_API.Services.Interfaces;
using Student_API.DTOs.RequestDTO;
using System.Threading.Tasks;

namespace Student_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentProfileUpdateController : ControllerBase
    {
        private readonly IStudentProfileUpdateServices _studentProfileUpdateServices;

        public StudentProfileUpdateController(IStudentProfileUpdateServices studentProfileUpdateServices)
        {
            _studentProfileUpdateServices = studentProfileUpdateServices;
        }

        [HttpPost("addProfileUpdateRequest")]
        public async Task<IActionResult> AddProfileUpdateRequest(int studentId, int status)
        {
            var response = await _studentProfileUpdateServices.AddProfileUpdateRequest(studentId, status);
            return Ok(response);
        }

        [HttpPut("updateProfileUpdateRequest")]
        public async Task<IActionResult> UpdateProfileUpdateRequest(int requestId, int newStatus)
        {
            var response = await _studentProfileUpdateServices.UpdateProfileUpdateRequest(requestId, newStatus);
            return Ok(response);
        }

        [HttpGet("getProfileUpdateRequests")]
        public async Task<IActionResult> GetProfileUpdateRequests([FromQuery] GetStudentProfileRequestModel requestModel)
        {
            var response = await _studentProfileUpdateServices.GetProfileUpdateRequests(requestModel);
            return Ok(response);
        }

      
    }
}
