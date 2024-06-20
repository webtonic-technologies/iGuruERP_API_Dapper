using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs;
using Student_API.Services.Implementations;
using Student_API.Services.Interfaces;

namespace Student_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class StudentQRController : ControllerBase
    {
        private readonly IStudentQRService _studentQRService;
        public StudentQRController(IStudentQRService studentQRService)
        {
            _studentQRService = studentQRService;
        }

        [HttpGet]
        [Route("GetAllStudentQR")]
        public async Task<IActionResult> GetAllStudentQR( int section_id, int class_id, string sortField = "Student_Name", string sortDirection = "ASC",  int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                var data = await _studentQRService.GetAllStudentQR(section_id, class_id, sortField, sortDirection, pageNumber, pageSize);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest("Bad Request");
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
    }
}
