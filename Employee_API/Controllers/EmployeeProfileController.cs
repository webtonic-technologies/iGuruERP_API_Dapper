using Employee_API.DTOs;
using Employee_API.Models;
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
        [HttpPost("AddEmployee")]
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
        [HttpGet("GetAllEmployeeList")]
        public async Task<IActionResult> GetEmployeeProfileList(GetAllEmployeeListRequest request)
        {
            try
            {
                var data = await _employeeProfileServices.GetEmployeeProfileList(request);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("EmployeeDoc")]
        public async Task<IActionResult> AddUpdateEmployeeDocuments([FromForm]EmployeeDocumentDTO request, int employeeId)
        {
            try
            {
                var data = await _employeeProfileServices.AddUpdateEmployeeDecuments(request, employeeId);
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
        [HttpGet("EmployeeDoc/{id}")]
        public async Task<IActionResult> GetEmployeeDocumentsById(int id)
        {
            try
            {
                var data = await _employeeProfileServices.GetEmployeeDocuments(id);
                 var response = new List<object>();
                foreach(var item in data.Data)
                {
                    var file = File(item, "image/*");
                    response.Add(file);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("BankDetails/{employeeId}")]
        public async Task<IActionResult> AddUpdateEmployeeBankDetails([FromBody] List<EmployeeBankDetails>? request, int employeeId)
        {
            try
            {
                var data = await _employeeProfileServices.AddUpdateEmployeeBankDetails(request, employeeId);
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
        [HttpPost("EmployeeFamily")]
        public async Task<IActionResult> AddUpdateEmployeeFamily([FromBody] EmployeeFamily request)
        {
            try
            {
                var data = await _employeeProfileServices.AddUpdateEmployeeFamily(request);
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
        [HttpPost("EmpWorkExperience/{employeeId}")]
        public async Task<IActionResult> AddUpdateEmployeeWorkExp([FromBody] List<EmployeeWorkExperience>? request, int employeeId)
        {
            try
            {
                var data = await _employeeProfileServices.AddUpdateEmployeeWorkExp(request, employeeId);
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
        [HttpPost("EmpQualification/{employeeId}")]
        public async Task<IActionResult> AddUpdateEmployeeQualification([FromBody] List<EmployeeQualification>? request, int employeeId)
        {
            try
            {
                var data = await _employeeProfileServices.AddUpdateEmployeeQualification(request, employeeId);
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
        [HttpGet("GetEmpQualification/{employeeId}")]
        public async Task<IActionResult> GetEmployeeQualificationById(int employeeId)
        {
            try
            {
                var data = await _employeeProfileServices.GetEmployeeQualificationById(employeeId);
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
        [HttpGet("GetEmpWorkExp/{employeeId}")]
        public async Task<IActionResult> GetEmployeeWorkExperienceById(int employeeId)
        {
            try
            {
                var data = await _employeeProfileServices.GetEmployeeWorkExperienceById(employeeId);
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
        [HttpGet("GetEmpBankDetails/{employeeId}")]
        public async Task<IActionResult> GetEmployeeBankDetailsById(int employeeId)
        {
            try
            {
                var data = await _employeeProfileServices.GetEmployeeBankDetailsById(employeeId);
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
        [HttpGet("GetEmpFamilyDetails/{employeeId}")]
        public async Task<IActionResult> GetEmployeeFamilyDetailsById(int employeeId)
        {
            try
            {
                var data = await _employeeProfileServices.GetEmployeeFamilyDetailsById(employeeId);
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
