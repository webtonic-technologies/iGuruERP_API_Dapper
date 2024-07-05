using Institute_API.DTOs;
using Institute_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Institute_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class GalleryController : ControllerBase
    {
        private readonly IGalleryService _galleryService;

        public GalleryController(IGalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        [HttpPost("AddGalleryImage")]
        public async Task<IActionResult> AddGalleryImage([FromBody] GalleryDTO galleryDTO)
        {
            try
            {
                var response = await _galleryService.AddGalleryImage(galleryDTO);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetApprovedImagesByEvent/{Institute_id}")]
        public async Task<IActionResult> GetApprovedImagesByEvent(int Institute_id)
        {
            try
            {
                var response = await _galleryService.GetApprovedImagesByEvent(Institute_id);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UpdateGalleryImageApprovalStatus")]
        public async Task<IActionResult> UpdateGalleryImageApprovalStatus([FromBody] ToggleGalleryActiveStatusRequest obj)
        {
            try
            {
                var response = await _galleryService.UpdateGalleryImageApprovalStatus(obj.GalleryId, obj.isApproved, obj.UserId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllGalleryImagesByEvent/{Institute_id}")]
        public async Task<IActionResult> GetAllGalleryImagesByEvent(int Institute_id)
        {
            try
            {
                var response = await _galleryService.GetAllGalleryImagesByEvent(Institute_id);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
