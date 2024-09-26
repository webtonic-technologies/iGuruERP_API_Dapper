using EventGallery_API.DTOs.Requests;
using EventGallery_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventGallery_API.Controllers
{
    [Route("iGuru/Event")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpPost("AddUpdateEvent")]
        public async Task<IActionResult> AddUpdateEvent([FromBody] EventRequest request)
        {
            var response = await _eventService.AddUpdateEvent(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetAllEvents")]
        public async Task<IActionResult> GetAllEvents([FromBody] GetAllEventsRequest request)
        {
            var result = await _eventService.GetAllEvents(request);
            return Ok(result);
        }


        [HttpGet("GetEventById/{eventId}")]
        public async Task<IActionResult> GetEventById(int eventId)
        {
            var response = await _eventService.GetEventById(eventId);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpDelete("DeleteEvent/{eventId}")]
        public async Task<IActionResult> DeleteEvent(int eventId)
        {
            var response = await _eventService.DeleteEvent(eventId);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPost("ExportAllEvents")]
        public async Task<IActionResult> ExportAllEvents()
        {
            var response = await _eventService.ExportAllEvents();
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
