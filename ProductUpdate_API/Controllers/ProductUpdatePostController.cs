using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ProductUpdates_API.DTOs.Requests;
using ProductUpdates_API.Services.Interfaces;

namespace ProductUpdates_API.Controllers
{
    [Route("iGuru/ProductUpdates/Post")]
    [ApiController]
    public class ProductUpdatePostController : ControllerBase
    {
        private readonly IProductUpdatePostService _service;

        public ProductUpdatePostController(IProductUpdatePostService service)
        {
            _service = service;
        }

        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            var response = await _service.CreatePost(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllPost/{ModuleID}")]
        public async Task<IActionResult> GetAllPosts(int ModuleID)
        {
            var response = await _service.GetAllPosts(ModuleID);
            return StatusCode(response.StatusCode, response);
        }
    }
}
