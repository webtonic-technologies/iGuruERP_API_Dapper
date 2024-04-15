using Employee_API.DTOs;
using Employee_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Employee_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class EmployeeProfileController : ControllerBase
    {
        private readonly IEmployeeProfileServices _employeeProfileServices;
        public EmployeeProfileController(IEmployeeProfileServices employeeProfileServices)
        {
            _employeeProfileServices = employeeProfileServices;
        }

        [HttpPost]
        public async Task<IActionResult> AddUpdateEmployeeProfile([FromBody] EmployeeProfileDTO request)
        {
            try
            {
                var data = await _employeeProfileServices.AddUpdateEmployeeProfile(request);
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
        [HttpGet("EmployeeProfile/{id}")]
        public async Task<IActionResult> GetEmployeeProfileById(int id)
        {
            try
            {
                var data = await _employeeProfileServices.GetEmployeeProfileById(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeeProfileList(int Instituteid)
        {
            try
            {
                var data = await _employeeProfileServices.GetEmployeeProfileList(Instituteid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
