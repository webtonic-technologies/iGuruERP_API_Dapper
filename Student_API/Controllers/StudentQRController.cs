using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.Services.Implementations;
using Student_API.Services.Interfaces;

namespace Student_API.Controllers
{
    [Route("iGuru/StudentManagement/[controller]")]
    [ApiController]
    public class StudentQRController : ControllerBase
    {
        private readonly IStudentQRService _studentQRService;
        public StudentQRController(IStudentQRService studentQRService)
        {
            _studentQRService = studentQRService;
        }

        [HttpPost]
        [Route("GetAllStudentQRcode")]
        public async Task<IActionResult> GetAllStudentQR(GetQrcodeRequestModel obj)
        {
            try
            {
                obj.sortField = obj.sortField ?? "Student_Name";
                obj.sortDirection = obj.sortDirection ?? "ASC";
                var data = await _studentQRService.GetAllStudentQR(obj.section_id, obj.class_id, obj.sortField, obj.sortDirection,obj.searchQuery, obj.pageNumber, obj.pageSize);
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
