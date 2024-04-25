﻿using Microsoft.AspNetCore.Mvc;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;

namespace Student_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class StudentInfoDropdownController : ControllerBase
    {
        private readonly IStudentInfoDropdownService _studentInfoDropdownService ;
       public StudentInfoDropdownController(IStudentInfoDropdownService studentInfoDropdownService)
        {
            _studentInfoDropdownService = studentInfoDropdownService;
        }
        [HttpGet]
        [Route("GetAllGenders")]
        public async Task<IActionResult> GetAllGenders()
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllGenders();
				return Ok(data);
			}
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("GetAllSections")]
        public async Task<IActionResult> GetAllSections()
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllSections();
				return Ok(data);
			}
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("GetAllReligions")]
        public async Task<IActionResult> GetAllReligions()
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllReligions();
				return Ok(data);
			}
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("GetAllNationalities")]
        public async Task<IActionResult> GetAllNationalities()
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllNationalities();
				return Ok(data);
			}
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("GetAllMotherTongues")]
        public async Task<IActionResult> GetAllMotherTongues()
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllMotherTongues();
				return Ok(data);
			}
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("GetAllBloodGroups")]
        public async Task<IActionResult> GetAllBloodGroups()
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllBloodGroups();
				return Ok(data);
			}
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("GetAllStudentTypes")]
        public async Task<IActionResult> GetAllStudentTypes()
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllStudentTypes();
				return Ok(data);
			}
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("GetAllStudentGroups")]
        public async Task<IActionResult> GetAllStudentGroups()
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllStudentGroups();
				return Ok(data);
			}
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("GetAllOccupations")]
        public async Task<IActionResult> GetAllOccupations()
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllOccupations();
				return Ok(data);
			}
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("GetAllParentTypes")]
        public async Task<IActionResult> GetAllParentTypes()
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllParentTypes();
				return Ok(data);
			}
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
    }
}