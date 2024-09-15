using Employee_API.DTOs;
using Employee_API.Models;
using Employee_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Employee_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class EmployeeDashboardController : ControllerBase
    {
        private readonly IEmployeeDashboardServices _employeeDashboardServices;
        public EmployeeDashboardController(IEmployeeDashboardServices employeeDashboardServices)
        {
            _employeeDashboardServices = employeeDashboardServices;
        }
        
        [HttpGet("GetEmployeeGenderStats/{instituteId}")]
        public async Task<IActionResult> GetEmployeeGenderStats(int instituteId)
        {
            try
            {
                var data = await _employeeDashboardServices.GetEmployeeGenderStats(instituteId);
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
        [HttpGet("GetEmployeeAgeGroupStats/{instituteId}")]
        public async Task<IActionResult> GetEmployeeAgeGroupStats(int instituteId)
        {
            try
            {
                var data = await _employeeDashboardServices.GetEmployeeAgeGroupStats(instituteId);
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
        [HttpGet("GetEmployeeExperienceStats/{instituteId}")]
        public async Task<IActionResult> GetEmployeeExperienceStats(int instituteId)
        {
            try
            {
                var data = await _employeeDashboardServices.GetEmployeeExperienceStats(instituteId);
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
        [HttpGet("GetEmployeeDepartmentCounts/{instituteId}")]
        public async Task<IActionResult> GetEmployeeDepartmentCounts(int instituteId)
        {
            try
            {
                var data = await _employeeDashboardServices.GetEmployeeDepartmentCounts(instituteId);
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
        [HttpGet("GetEmployeeEvents/{instituteId}")]
        public async Task<IActionResult> GetEmployeeEvents(DateTime date, int instituteId)
        {
            try
            {
                var data = await _employeeDashboardServices.GetEmployeeEvents(date, instituteId);
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
        [HttpGet("GetGenderCountByDesignation/{instituteId}")]
        public async Task<IActionResult> GetGenderCountByDesignation(int instituteId)
        {
            try
            {
                var data = await _employeeDashboardServices.GetGenderCountByDesignation(instituteId);
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
        [HttpGet("GetActiveInactiveEmployees/{instituteId}")]
        public async Task<IActionResult> GetActiveInactiveEmployees(int instituteId)
        {
            try
            {
                var data = await _employeeDashboardServices.GetActiveInactiveEmployees(instituteId);
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
        [HttpGet("GetAppUsersNonAppUsersEmployees/{instituteId}")]
        public async Task<IActionResult> GetAppUsersNonAppUsersEmployees(int instituteId)
        {
            try
            {
                var data = await _employeeDashboardServices.GetAppUsersNonAppUsersEmployees(instituteId);
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
