using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Model;
using Lesson_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Controllers
{
    [Route("iGuru/LessonPlanning/[controller]")]
    [ApiController]
    public class LessonPlanTypeController : ControllerBase
    {
        private readonly IPlanTypeService _planTypeService;

        public LessonPlanTypeController(IPlanTypeService planTypeService)
        {
            _planTypeService = planTypeService;
        }

        [HttpGet("GetLessonPlanType")]
        public async Task<ActionResult<ServiceResponse<List<PlanType>>>> GetLessonPlanType()
        {
            var response = await _planTypeService.GetAllPlanTypesAsync();
            return Ok(response);
        }
    }
}
