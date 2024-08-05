using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.Models;
using LibraryManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpPost("AddUpdateGenre")]
        public async Task<IActionResult> AddUpdateGenre(Genre request)
        {
            var response = await _genreService.AddUpdateGenre(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllGenre")]
        public async Task<IActionResult> GetAllGenre(GetAllGenreRequest request)
        {
            var response = await _genreService.GetAllGenres(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetGenre/{GenreID}")]
        public async Task<IActionResult> GetGenre(int GenreID)
        {
            var response = await _genreService.GetGenreById(GenreID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{GenreID}")]
        public async Task<IActionResult> DeleteGenre(int GenreID)
        {
            var response = await _genreService.DeleteGenre(GenreID);
            return StatusCode(response.StatusCode, response);
        }
    }
}
