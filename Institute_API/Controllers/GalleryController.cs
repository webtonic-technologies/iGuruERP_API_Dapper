using Institute_API.DTOs;
using Institute_API.Services.Implementations;
using Institute_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

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

        [HttpPost("GetApprovedImagesByEvent")]
        public async Task<IActionResult> GetApprovedImagesByEvent(GetGalleryRequestModel model)
        {
            try
            {
                var response = await _galleryService.GetApprovedImagesByEvent(model);
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
                var response = await _galleryService.UpdateGalleryImageApprovalStatus(obj.GalleryId, obj.Status, obj.UserId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetAllGalleryImagesByEvent")]
        public async Task<IActionResult> GetAllGalleryImagesByEvent(GetGalleryRequestModel model)
        {
            try
            {
                var response = await _galleryService.GetAllGalleryImagesByEvent(model);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteGalleryImage/{Gallery_id}")]
        public async Task<IActionResult> DeleteGalleryImage(int Gallery_id)
        {
            try
            {
                var data = await _galleryService.DeleteGalleryImage(Gallery_id);
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
