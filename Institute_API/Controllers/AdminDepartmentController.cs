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

        [HttpPost]
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

        [HttpGet("AdminDepartment/{id}")]
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

        [HttpGet]
        public async Task<IActionResult> GetAdminDepartmentList(int Instituteid)
        {
            try
            {
                var data = await _adminDepartmentServices.GetAdminDepartmentList(Instituteid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
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
    }
}
