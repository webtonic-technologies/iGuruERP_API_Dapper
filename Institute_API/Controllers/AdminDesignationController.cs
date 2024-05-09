using Institute_API.DTOs;
using Institute_API.Models;
using Institute_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Institute_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class AdminDesignationController : ControllerBase
    {
        private readonly IAdminDesignationServices _adminDesignationServices;

        public AdminDesignationController(IAdminDesignationServices adminDesignationServices)
        {
            _adminDesignationServices = adminDesignationServices;
        }

        [HttpPost("AddAdminDesignation")]
        public async Task<IActionResult> AddUpdateAdminDesignation([FromBody]AdminDesignation request)
        {
            try
            {
                var data = await _adminDesignationServices.AddUpdateAdminDesignation(request);
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

        [HttpGet("GetAdminDesignationById/{id}")]
        public async Task<IActionResult> GetAdminDesignationById(int id)
        {
            try
            {
                var data = await _adminDesignationServices.GetAdminDesignationById(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllAdminDesignationList")]
        public async Task<IActionResult> GetAdminDesignationList(int Instituteid)
        {
            try
            {
                var data = await _adminDesignationServices.GetAdminDesignationList(Instituteid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteAdminDesignation")]
        public async Task<IActionResult> DeleteAdminDesignation(int id)
        {
            try
            {
                var data = await _adminDesignationServices.DeleteAdminDesignation(id);
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
