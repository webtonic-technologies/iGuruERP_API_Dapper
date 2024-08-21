using Infirmary_API.DTOs.Requests;
using Infirmary_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Infirmary_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class InfirmaryVisitController : ControllerBase
    {
        // Declare both services as readonly fields
        private readonly IInfirmaryVisitService _infirmaryVisitService;
        private readonly IInfirmaryVisitorTypeService _infirmaryVisitorTypeService;

        // Single constructor to inject both services
        public InfirmaryVisitController(IInfirmaryVisitService infirmaryVisitService, IInfirmaryVisitorTypeService infirmaryVisitorTypeService)
        {
            _infirmaryVisitService = infirmaryVisitService;
            _infirmaryVisitorTypeService = infirmaryVisitorTypeService;
        }

        [HttpPost("AddUpdateInfirmaryVisit")]
        public async Task<IActionResult> AddUpdateInfirmaryVisit(AddUpdateInfirmaryVisitRequest request)
        {
            try
            {
                var data = await _infirmaryVisitService.AddUpdateInfirmaryVisit(request);
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

        [HttpPost("GetAllInfirmaryVisits")]
        public async Task<IActionResult> GetAllInfirmaryVisits(GetAllInfirmaryVisitsRequest request)
        {
            try
            {
                var data = await _infirmaryVisitService.GetAllInfirmaryVisits(request);
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

        [HttpGet("GetInfirmaryVisitById/{VisitID}")]
        public async Task<IActionResult> GetInfirmaryVisitById(int VisitID)
        {
            try
            {
                var data = await _infirmaryVisitService.GetInfirmaryVisitById(VisitID);
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

        [HttpPut("DeleteInfirmaryVisit/{VisitID}")]
        public async Task<IActionResult> DeleteInfirmaryVisit(int VisitID)
        {
            try
            {
                var data = await _infirmaryVisitService.DeleteInfirmaryVisit(VisitID);
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

        [HttpGet("GetAllInfirmaryVisitorType")]
        public async Task<IActionResult> GetAllInfirmaryVisitorType()
        {
            try
            {
                var data = await _infirmaryVisitorTypeService.GetAllInfirmaryVisitorTypes();
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return NotFound(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
    }
}
