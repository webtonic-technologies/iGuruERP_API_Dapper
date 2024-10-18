using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
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

        [HttpPost]
        [Route("GetStudentsForPromotion")]
        public async Task<IActionResult> GetStudentsForPromotion(GetStudentsForPromotionParam obj)
        {
            try
            {
                obj.sortField = obj.sortField ?? "Student_Name";
                obj.sortDirection = obj.sortDirection ?? "ASC";
                var data = await _studentPromotionService.GetStudentsForPromotion(obj);
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
        public async Task<IActionResult> PromoteStudents([FromBody] PromoteStudentDTO promoteStudentDTO)
        {
            try
            {
                var data = await _studentPromotionService.PromoteStudents(promoteStudentDTO.studentIds, promoteStudentDTO.nextClassId, promoteStudentDTO.sectionId);
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
        [Route("PromoteClasses")]
        public async Task<IActionResult> PromoteClasses([FromBody] ClassPromotionDTO classPromotionDTO)
        {
            try
            {
                var response = await _studentPromotionService.PromoteClasses(classPromotionDTO);
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost]
        [Route("GetClassPromotionLog")]
        public async Task<IActionResult> GetClassPromotionLog([FromBody] GetClassPromotionLogParam param)
        {
            try
            {
                var response = await _studentPromotionService.GetClassPromotionLog(param);
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("GetToClassIdAsync")]
        public async Task<IActionResult> GetToClassIdAsync([FromBody] ClassPromotionParams param)
        {
            try
            {
                var response = await _studentPromotionService.GetToClassIdAsync(param);
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("ExportClassPromotionLogToExcel")]
        public async Task<IActionResult> ExportClassPromotionLogToExcel(GetClassPromotionLogParam obj)
        {
            var response = await _studentPromotionService.ExportClassPromotionLogToExcel(obj);

            if (response.Success)
            {
                var filePath = response.Data;
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                // Return the Excel file for download
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(filePath));
            }
            else
            {
                return BadRequest(response.Message);
            }
        }


    }
}