using Institute_API.DTOs;
using Institute_API.Models;
using Institute_API.Services.Implementations;
using Institute_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Institute_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventServices _eventService;

        public EventController(IEventServices eventService)
        {
            _eventService = eventService;
        }
        [HttpGet("GetEventById/{eventId}")]
        public async Task<IActionResult> GetEventById(int eventId)
        {

            try
            {
                var data = await _eventService.GetEventById(eventId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetApprovedEvents")]
        public async Task<IActionResult> GetApprovedEvents()
        {
            try
            {
                var data = await _eventService.GetApprovedEvents();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllEvents")]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var data = await _eventService.GetAllEvents();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("AddUpdateEvent")]
        public async Task<IActionResult> AddUpdateEvent([FromBody] EventDTO eventDto)
        {
            try
            {
                var data = await _eventService.AddUpdateEvent(eventDto);
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

        [HttpDelete("DeleteEvent")]
        public async Task<IActionResult> DeleteEvent(int eventId)
        {
            try
            {
                var data = await _eventService.DeleteEvent(eventId);
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

        [HttpPost("ToggleEventActiveStatus")]
        public async Task<IActionResult> ToggleEventActiveStatus([FromBody]ToggleEventActiveStatusRequest toggleEvent)
        {
            try
            {
                var data = await _eventService.ToggleEventActiveStatus(toggleEvent.EventId, toggleEvent.IsActive, toggleEvent.UserId);
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
