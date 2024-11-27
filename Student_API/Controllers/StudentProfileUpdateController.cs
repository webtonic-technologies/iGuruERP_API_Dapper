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
        public async Task<IActionResult> AddProfileUpdateRequest([FromBody] AddProfileUpdate obj)
        {
            var response = await _studentProfileUpdateServices.AddProfileUpdateRequest(obj.studentId, obj.status);
            return Ok(response);
        }

        [HttpPut("updateProfileUpdateRequest")]
        public async Task<IActionResult> UpdateProfileUpdateRequest([FromBody] UpdateProfile obj)
        {
            var response = await _studentProfileUpdateServices.UpdateProfileUpdateRequest(obj.requestId, obj.newStatus);
            return Ok(response);
        }

        [HttpPost("getProfileUpdateRequests")]
        public async Task<IActionResult> GetProfileUpdateRequests([FromBody] GetStudentProfileRequestModel requestModel)
        {
            var response = await _studentProfileUpdateServices.GetProfileUpdateRequests(requestModel);
            return Ok(response);
        }

      
    }
}
