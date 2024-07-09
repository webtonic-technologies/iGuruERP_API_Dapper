using Microsoft.AspNetCore.Mvc;
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
        [Route("Gender/GetAllGenders")]
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
        [Route("Sections/GetAllSections/{Class_Id}")]
        public async Task<IActionResult> GetAllSections(int Class_Id)
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllSections(Class_Id);
				return Ok(data);
			}
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("Sections/GetAllClass/{institute_id}")]
        public async Task<IActionResult> GetAllClass(int institute_id)
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllClass(institute_id);
                return Ok(data);
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("Religions/GetAllReligions")]
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
        [Route("Nationalities/GetAllNationalities")]
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
        [Route("MotherTongues/GetAllMotherTongues")]
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
        [Route("BloodGroups/GetAllBloodGroups")]
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
        [Route("StudentTypes/GetAllStudentTypes")]
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
        [Route("StudentGroups/GetAllStudentGroups")]
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
        [Route("Occupations/GetAllOccupations")]
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
        [Route("ParentTypes/GetAllParentTypes")]
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

        [HttpGet]
        [Route("States/GetAllStates")]
        public async Task<IActionResult> GetAllStates()
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllStates();
                return Ok(data);
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("Cities/GetAllCities/{StateId}")]
        public async Task<IActionResult> GetAllCities(int StateId)
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllCities(StateId);
                return Ok(data);
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("AcademicYear/GetAllAcademicYear")]
        public async Task<IActionResult> GetAllAcademic()
        {
            try
            {
                var data = await _studentInfoDropdownService.GetAllAcademic();
                return Ok(data);
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
    }
}
