using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.Models;
using LibraryManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement_API.Controllers.Configuration
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpPost("AddUpdatePublisher")]
        public async Task<IActionResult> AddUpdatePublisher(Publisher request)
        {
            var response = await _publisherService.AddUpdatePublisher(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetPublisher/{PublisherID}")]
        public async Task<IActionResult> GetPublisher(int PublisherID)
        {
            var response = await _publisherService.GetPublisherById(PublisherID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllPublishers")]
        public async Task<IActionResult> GetAllPublishers(GetAllPublishersRequest request)
        {
            var response = await _publisherService.GetAllPublishers(request);
            return StatusCode(response.StatusCode, response);
        }

        

        [HttpPut("Delete/{PublisherID}")]
        public async Task<IActionResult> DeletePublisher(int PublisherID)
        {
            var response = await _publisherService.DeletePublisher(PublisherID);
            return StatusCode(response.StatusCode, response);
        }
    }
}
