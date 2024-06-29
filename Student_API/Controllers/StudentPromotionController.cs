using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs;
using Student_API.Services.Implementations;
using Student_API.Services.Interfaces;

namespace Student_API.Controllers
{
    [Route("iGuru/StudentManagement/[controller]")]
    [ApiController]
    public class StudentPromotionController : ControllerBase
    {
        private readonly IStudentPromotionService _studentPromotionService;
        public StudentPromotionController(IStudentPromotionService studentPromotionService)
        {
            _studentPromotionService = studentPromotionService;     
        }

        [HttpGet]
        [Route("GetStudentsForPromotion")]
        public async Task<IActionResult> GetStudentsForPromotion(int classId=0, string sortField = "Student_Name",string sortDirection = "ASC", int? pageSize = null, int? pageNumber = null)
        {
            try
            {
                var data = await _studentPromotionService.GetStudentsForPromotion(classId, sortField, sortDirection, pageSize,pageNumber );
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

        [HttpPost]
        [Route("PromoteStudents")]
        public async Task<IActionResult> PromoteStudents([FromBody]PromoteStudentDTO promoteStudentDTO)
        {
            try
            {
                var data = await _studentPromotionService.PromoteStudents(promoteStudentDTO.studentIds, promoteStudentDTO.nextClassId);
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