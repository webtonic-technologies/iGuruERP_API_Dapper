using Employee_API.DTOs;
using Employee_API.Models;
using Employee_API.Services.Implementations;
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
        [HttpPost("AddUpdateEmployeeProfile")]
        public async Task<IActionResult> AddUpdateEmployeeProfile([FromBody] EmployeeProfile request)
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
        [HttpPost("GetAllEmployeeList")]
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
        //[HttpPost("EmployeeDoc")]
        //public async Task<IActionResult> AddUpdateEmployeeDocuments([FromBody]List<EmployeeDocument> request, int employeeId)
        //{
        //    try
        //    {
        //        var data = await _employeeProfileServices.AddUpdateEmployeeDocuments(request, employeeId);
        //        if (data != null)
        //        {
        //            return Ok(data);
        //        }
        //        else
        //        {
        //            return BadRequest("Bad Request");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return this.BadRequest(e.Message);
        //    }

        //}
        //[HttpGet("EmployeeDoc/{id}")]
        //public async Task<IActionResult> GetEmployeeDocumentsById(int id)
        //{
        //    try
        //    {
        //        var data = await _employeeProfileServices.GetEmployeeDocuments(id);
        //        if (data != null)
        //        {
        //            return Ok(data);
        //        }
        //        else
        //        {
        //            return BadRequest("Bad Request");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        //[HttpPost("BankDetails/{employeeId}")]
        //public async Task<IActionResult> AddUpdateEmployeeBankDetails([FromBody] List<EmployeeBankDetails>? request, int employeeId)
        //{
        //    try
        //    {
        //        var data = await _employeeProfileServices.AddUpdateEmployeeBankDetails(request, employeeId);
        //        if (data != null)
        //        {
        //            return Ok(data);
        //        }
        //        else
        //        {
        //            return BadRequest("Bad Request");
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        return this.BadRequest(e.Message);
        //    }

        //}
        //[HttpPost("EmployeeFamily")]
        //public async Task<IActionResult> AddUpdateEmployeeFamily([FromBody] EmployeeFamily request)
        //{
        //    try
        //    {
        //        var data = await _employeeProfileServices.AddUpdateEmployeeFamily(request);
        //        if (data != null)
        //        {
        //            return Ok(data);
        //        }
        //        else
        //        {
        //            return BadRequest("Bad Request");
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        return this.BadRequest(e.Message);
        //    }

        //}
        //[HttpPost("EmpWorkExperience/{employeeId}")]
        //public async Task<IActionResult> AddUpdateEmployeeWorkExp([FromBody] List<EmployeeWorkExperience>? request, int employeeId)
        //{
        //    try
        //    {
        //        var data = await _employeeProfileServices.AddUpdateEmployeeWorkExp(request, employeeId);
        //        if (data != null)
        //        {
        //            return Ok(data);
        //        }
        //        else
        //        {
        //            return BadRequest("Bad Request");
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        return this.BadRequest(e.Message);
        //    }

        //}
        //[HttpPost("EmpQualification/{employeeId}")]
        //public async Task<IActionResult> AddUpdateEmployeeQualification([FromBody] List<EmployeeQualification>? request, int employeeId)
        //{
        //    try
        //    {
        //        var data = await _employeeProfileServices.AddUpdateEmployeeQualification(request, employeeId);
        //        if (data != null)
        //        {
        //            return Ok(data);
        //        }
        //        else
        //        {
        //            return BadRequest("Bad Request");
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        return this.BadRequest(e.Message);
        //    }

        //}
        //[HttpGet("GetEmpQualification/{employeeId}")]
        //public async Task<IActionResult> GetEmployeeQualificationById(int employeeId)
        //{
        //    try
        //    {
        //        var data = await _employeeProfileServices.GetEmployeeQualificationById(employeeId);
        //        if (data != null)
        //        {
        //            return Ok(data);
        //        }
        //        else
        //        {
        //            return BadRequest("Bad Request");
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        return this.BadRequest(e.Message);
        //    }

        //}
        //[HttpGet("GetEmpWorkExp/{employeeId}")]
        //public async Task<IActionResult> GetEmployeeWorkExperienceById(int employeeId)
        //{
        //    try
        //    {
        //        var data = await _employeeProfileServices.GetEmployeeWorkExperienceById(employeeId);
        //        if (data != null)
        //        {
        //            return Ok(data);
        //        }
        //        else
        //        {
        //            return BadRequest("Bad Request");
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        return this.BadRequest(e.Message);
        //    }

        //}
        //[HttpGet("GetEmpBankDetails/{employeeId}")]
        //public async Task<IActionResult> GetEmployeeBankDetailsById(int employeeId)
        //{
        //    try
        //    {
        //        var data = await _employeeProfileServices.GetEmployeeBankDetailsById(employeeId);
        //        if (data != null)
        //        {
        //            return Ok(data);
        //        }
        //        else
        //        {
        //            return BadRequest("Bad Request");
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        return this.BadRequest(e.Message);
        //    }

        //}
        //[HttpGet("GetEmpFamilyDetails/{employeeId}")]
        //public async Task<IActionResult> GetEmployeeFamilyDetailsById(int employeeId)
        //{
        //    try
        //    {
        //        var data = await _employeeProfileServices.GetEmployeeFamilyDetailsById(employeeId);
        //        if (data != null)
        //        {
        //            return Ok(data);
        //        }
        //        else
        //        {
        //            return BadRequest("Bad Request");
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        return this.BadRequest(e.Message);
        //    }

        //}
        [HttpPut("Status")]
        public async Task<IActionResult> StatusActiveInactive(EmployeeStatusRequest request)
        {
            try
            {
                var data = await _employeeProfileServices.StatusActiveInactive(request);
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
        [HttpGet("GetMaritalStatusList")]
        public async Task<IActionResult> GetMaritalStatusList()
        {
            try
            {
                var data = await _employeeProfileServices.GetMaritalStatusList();
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
        [HttpGet("GetBloodGroupList")]
        public async Task<IActionResult> GetBloodGroupList()
        {
            try
            {
                var data = await _employeeProfileServices.GetBloodGroupList();
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
        [HttpGet("GetDepartmentList/{InstituteId}")]
        public async Task<IActionResult> GetDepartmentList(int InstituteId)
        {
            try
            {
                var data = await _employeeProfileServices.GetDepartmentList(InstituteId);
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
        [HttpGet("GetDesignationList/{DepartmentId}")]
        public async Task<IActionResult> GetDesignationList(int DepartmentId)
        {
            try
            {
                var data = await _employeeProfileServices.GetDesignationList(DepartmentId);
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
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> UpdatePassword(ChangePasswordRequest request)
        {
            try
            {
                var data = await _employeeProfileServices.UpdatePassword(request);
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
        //[HttpPost("DownloadExcel")]
        //public async Task<IActionResult> DownloadExcelSheet(ExcelDownloadRequest request)
        //{
        //    var response = await _employeeProfileServices.ExcelDownload(request);
        //    if (response.Success)
        //    {
        //        return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee.xlsx");
        //    }
        //    return StatusCode(response.StatusCode, response.Message);
        //}
        [HttpGet("GetClassSectionList/{instituteId}")]
        public async Task<IActionResult> GetClassSectionList(int instituteId)
        {
            try
            {
                var data = await _employeeProfileServices.GetClassSectionList(instituteId);
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
        [HttpGet("ClassSectionSubjectsList/{classId}/{sectionId}")]
        public async Task<IActionResult> ClassSectionSubjectsList(int classId, int sectionId)
        {
            try
            {
                var data = await _employeeProfileServices.ClassSectionSubjectsList(classId, sectionId);
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
        [HttpGet("ClassSectionSubjectsMappings/{InstituteId}")]
        public async Task<IActionResult> ClassSectionSubjectsMappings(int InstituteId)
        {
            try
            {
                var data = await _employeeProfileServices.ClassSectionSubjectsMappings(InstituteId);
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
        [HttpGet("DownloadEmployeeData")]
        public async Task<IActionResult> DownloadEmployeeData([FromQuery] ExcelDownloadRequest request, [FromQuery] string format)
        {
            var response = await _employeeProfileServices.ExcelDownload(request, format);

            if (response.Success)
            {
                if (format.ToLower() == "csv")
                {
                    return File(response.Data, "text/csv", "Employee Data.csv");
                }
                else
                {
                    return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee Data.xlsx");
                }
            }

            return StatusCode(response.StatusCode, response.Message);
        }
        [HttpPost("BulkUpdate")]
        public async Task<IActionResult> BulkUpdate([FromBody] GetListRequest request)
        {
            // Validate request
            if (request == null || request.InstituteId == 0)
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                // Call service method
                var response = await _employeeProfileServices.BulkUpdate(request);

                if (response.Success)
                {
                    // Return the Excel file as a downloadable response
                    var fileName = $"Employee_Data_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                    return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
                else
                {
                    return StatusCode(response.StatusCode, response.Message);
                }
            }
            catch (Exception ex)
            {
                // Log exception and return error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPut("bulk-update")]
        public async Task<IActionResult> BulkUpdateEmployeeProfiles([FromBody] List<EmployeeProfile> employeeProfiles)
        {
            // Validate input
            if (employeeProfiles == null || !employeeProfiles.Any())
            {
                return BadRequest("Employee profile list cannot be empty.");
            }

            // Call the service method to perform the bulk update
            var response = await _employeeProfileServices.BulkUpdateEmployee(employeeProfiles);

            // Handle the response
            if (response.Success)
            {
                return Ok(response);  // Return a 200 status with the response message
            }
            else
            {
                return StatusCode(response.StatusCode, response);  // Return the error code and message
            }
        }
        [HttpGet("EmployeeColumn")]
        public async Task<IActionResult> GetEmployeeColumns()
        {
            var result = await _employeeProfileServices.GetEmployeeColumnsAsync();

            if (result == null)
            {
                return NotFound("No employee columns found.");
            }

            return Ok(result);
        }
        [HttpPost("UploadEmployeeData")]
        public async Task<IActionResult> UploadEmployeeData(IFormFile file, [FromQuery] int instituteId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is not provided");

            try
            {
                // Parse Excel file and read its data
                var data = await _employeeProfileServices.ParseExcelFile(file, instituteId);
                if (data != null && data.Any())
                {
                    return Ok(data);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("BulkHistory/{InstituteId}")]
        public async Task<IActionResult> GetBulkHistoryByInstituteId(int InstituteId)
        {
            var result = await _employeeProfileServices.GetBulkHistoryByInstituteId(InstituteId);

            if (result == null)
            {
                return NotFound("No data found.");
            }

            return Ok(result);
        }
        [HttpGet("EmployeeExport/{InstituteId}")]
        public async Task<IActionResult> GetExportHistoryByInstituteId(int InstituteId)
        {
            var result = await _employeeProfileServices.GetExportHistoryByInstituteId(InstituteId);

            if (result == null)
            {
                return NotFound("No employee columns found.");
            }

            return Ok(result);
        }
        [HttpGet("DownloadSheet")]
        public async Task<IActionResult> DownloadSheetImport(int instituteId)
        {
            var result = await _employeeProfileServices.DownloadSheetImport(instituteId);

            if (result.Success)
            {
                return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeData.xlsx");
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadEmployeedata(IFormFile file, int instituteId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var response = await _employeeProfileServices.UploadEmployeedata(file, instituteId);
            if (response.Success)
            {
                return Ok(response.Message);
            }
            return StatusCode(response.StatusCode, response.Message);
        }
    }
}
