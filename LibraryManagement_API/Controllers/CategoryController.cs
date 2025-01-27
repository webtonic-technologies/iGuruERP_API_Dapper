using LibraryManagement_API.DTOs.Requests; // Ensure this using directive is present
using LibraryManagement_API.Models;
using LibraryManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("AddUpdateCategory")]
        public async Task<IActionResult> AddUpdateCategories(List<Category> requests)
        {
            var response = await _categoryService.AddUpdateCategories(requests);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories(GetAllCategoriesRequest request)
        {
            var response = await _categoryService.GetAllCategories(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetCategory/{CategoryID}")]
        public async Task<IActionResult> GetCategory(int CategoryID)
        {
            var response = await _categoryService.GetCategoryById(CategoryID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{CategoryID}")]
        public async Task<IActionResult> DeleteCategory(int CategoryID)
        {
            var response = await _categoryService.DeleteCategory(CategoryID);
            return StatusCode(response.StatusCode, response);
        }
    }
}
