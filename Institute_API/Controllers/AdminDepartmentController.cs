using Institute_API.DTOs;
using Institute_API.Models;
using Institute_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Institute_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class AdminDepartmentController : ControllerBase
    {
        private readonly IAdminDepartmentServices _adminDepartmentServices;

        public AdminDepartmentController(IAdminDepartmentServices adminDepartmentServices)
        {
            _adminDepartmentServices = adminDepartmentServices;
        }

        [HttpPost("AddAdminDepartment")]
        public async Task<IActionResult> AddUpdateAdminDepartment([FromBody] AdminDepartment request)
        {
            try
            {
                var data = await _adminDepartmentServices.AddUpdateAdminDept(request);
                if (data != null)
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

        [HttpGet("GetAdminDepartmentById/{id}")]
        public async Task<IActionResult> GetAdminDepartmentById(int id)
        {
            try
            {
                var data = await _adminDepartmentServices.GetAdminDepartmentById(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetAllAdminDepartmentList")]
        public async Task<IActionResult> GetAdminDepartmentList(GetListRequest request)
        {
            try
            {
                var data = await _adminDepartmentServices.GetAdminDepartmentList(request);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteAdminDepartment")]
        public async Task<IActionResult> DeleteAdminDepartment(int id)
        {
            try
            {
                var data = await _adminDepartmentServices.DeleteAdminDepartment(id);
                if (data != null)
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
        [HttpGet("DownloadExcel/{instituteId}")]
        public async Task<IActionResult> DownloadExcelSheet(int instituteId)
        {
            var response = await _adminDepartmentServices.DownloadExcelSheet(instituteId);
            if (response.Success)
            {
                return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Department.xlsx");
            }
            return StatusCode(response.StatusCode, response.Message);
        }
    }
}
