using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs;
using Student_API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Student_API.Controllers
{
    [Route("iGuru/Certificate/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplateController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpPost]
        [Route("AddUpdateTemplate")]
        public async Task<IActionResult> AddUpdateTemplate([FromBody] TemplateDTO templateDto)
        {
            try
            {
                var response = await _templateService.AddUpdateTemplate(templateDto);
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("GetByIdStudentTemplateSelection/{templateId}")]
        public async Task<IActionResult> GetTemplateById(int templateId)
        {
            try
            {
                var response = await _templateService.GetTemplateById(templateId);
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteByIDStudentTemplate/{templateId}")]
        public async Task<IActionResult> DeleteTemplate(int templateId)
        {
            try
            {
                var response = await _templateService.DeleteTemplate(templateId);
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("GetAllStudentTemplateSelectionList")]
        public async Task<IActionResult> GetAllTemplates([FromQuery] int? pageSize = null, [FromQuery] int? pageNumber = null)
        {
            try
            {
                var response = await _templateService.GetAllTemplates(pageSize, pageNumber);
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
    }
}
