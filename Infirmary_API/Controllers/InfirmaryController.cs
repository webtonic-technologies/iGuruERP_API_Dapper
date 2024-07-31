using Infirmary_API.DTOs.Requests;
using Infirmary_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Infirmary_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class InfirmaryController : ControllerBase
    {
        private readonly IInfirmaryService _infirmaryService;

        public InfirmaryController(IInfirmaryService infirmaryService)
        {
            _infirmaryService = infirmaryService;
        }

        [HttpPost("AddUpdateInfirmary")]
        public async Task<IActionResult> AddUpdateInfirmary(AddUpdateInfirmaryRequest request)
        {
            try
            {
                var data = await _infirmaryService.AddUpdateInfirmary(request);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPost("GetAllInfirmary")]
        public async Task<IActionResult> GetAllInfirmary(GetAllInfirmaryRequest request)
        {
            try
            {
                var data = await _infirmaryService.GetAllInfirmary(request);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpGet("GetInfirmary/{InfirmaryID}")]
        public async Task<IActionResult> GetInfirmary(int InfirmaryID)
        {
            try
            {
                var data = await _infirmaryService.GetInfirmaryById(InfirmaryID);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPut("DeleteInfirmary/{InfirmaryID}")]
        public async Task<IActionResult> DeleteInfirmary(int InfirmaryID)
        {
            try
            {
                var data = await _infirmaryService.DeleteInfirmary(InfirmaryID);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
    }
}
