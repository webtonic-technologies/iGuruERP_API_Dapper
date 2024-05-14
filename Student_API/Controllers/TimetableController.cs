using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs;
using Student_API.Models;
using Student_API.Services.Implementations;
using Student_API.Services.Interfaces;

namespace Student_API.Controllers
{

    [Route("iGuru/[controller]")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        private readonly ITimetableServices _timetableServices;
        public TimetableController(ITimetableServices timetableServices)
        {
            _timetableServices = timetableServices;
        }

        [HttpPost]
        [Route("AddUpdateTimeTableGroup")]
        public async Task<IActionResult> AddUpdateTimeTableGroup(TimeTableGroupDTO timeTableGroupDTO)
        {
            try
            {
                var data = await _timetableServices.AddUpdateTimeTableGroup(timeTableGroupDTO);
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

        [HttpGet]
        [Route("GetTimeTableGroup")]
        public async Task<IActionResult> GetTimeTableGroup()
        {
            try
            {
                var data = await _timetableServices.GetTimeTableGroup();
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
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("GetTimeTableGroupById/{timetableGroupId}")]
        public async Task<IActionResult> GetTimeTableGroupById(int timetableGroupId)
        {
            try
            {
                var data = await _timetableServices.GetTimeTableGroupById(timetableGroupId);
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
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("AddTimeTableDaysPlan")]
        public async Task<IActionResult> AddTimeTableDaysPlan(DaysSetupDTO daysSetupDTO)
        {
            try
            {
                var data = await _timetableServices.AddTimeTableDaysPlan(daysSetupDTO);
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

        [HttpGet]
        [Route("GetTimeTableDaysPlan")]
        public async Task<IActionResult> GetTimeTableDaysPlan()
        {
            try
            {
                var data = await _timetableServices.GetTimeTableDaysPlan();
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
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("AddUpdateTimeTableMaker")]
        public async Task<IActionResult> AddOrUpdateTimetable(Timetable timetable)
        {
            try
            {
                var data = await _timetableServices.AddOrUpdateTimetable( timetable);
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

        [HttpGet]
        [Route("GetTimeTableMakerList")]
        public async Task<IActionResult> GetTimeTableMakerList(TimetableParam timetableParam)
        {
            try
            {
                var data = await _timetableServices.GetTimetablesByCriteria(timetableParam);
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
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteTimetableGroup")]
        public async Task<IActionResult> DeleteTimetableGroup(int timetableGroupId)
        {
            try
            {
                var data = await _timetableServices.DeleteTimetableGroup(timetableGroupId);
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
                return BadRequest(e.Message);
            }
        }
    }
}
