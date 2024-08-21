using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.Models;
using LibraryManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpPost("AddUpdateAuthor")]
        public async Task<IActionResult> AddUpdateAuthors(AddUpdateAuthorsRequest request)
        {
            var response = await _authorService.AddUpdateAuthors(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllAuthors")]
        public async Task<IActionResult> GetAllAuthors(GetAllAuthorsRequest request)
        {
            var response = await _authorService.GetAllAuthors(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllAuthors_Fetch")]
        public async Task<IActionResult> GetAllAuthorsFetch(GetAllAuthorsFetchRequest request)
        {
            var response = await _authorService.GetAllAuthorsFetch(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetAuthor/{AuthorID}")]
        public async Task<IActionResult> GetAuthor(int AuthorID)
        {
            var response = await _authorService.GetAuthorById(AuthorID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{AuthorID}")]
        public async Task<IActionResult> DeleteAuthor(int AuthorID)
        {
            var response = await _authorService.DeleteAuthor(AuthorID);
            return StatusCode(response.StatusCode, response);
        }
    }
}
