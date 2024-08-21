using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibraryManagement_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryService _libraryService;

        public LibraryController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [HttpPost("AddUpdateLibrary")]
        public async Task<IActionResult> AddUpdateLibrary(AddUpdateLibraryRequest request)
        {
            var response = await _libraryService.AddUpdateLibrary(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllLibrary")]
        public async Task<IActionResult> GetAllLibrary(GetAllLibraryRequest request)
        {
            var response = await _libraryService.GetAllLibraries(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetLibrary/{LibraryID}")]
        public async Task<IActionResult> GetLibrary(int LibraryID)
        {
            var response = await _libraryService.GetLibraryById(LibraryID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{LibraryID}")]
        public async Task<IActionResult> DeleteLibrary(int LibraryID)
        {
            var response = await _libraryService.DeleteLibrary(LibraryID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllLibraryIncharge")]
        public async Task<IActionResult> GetAllLibraryIncharge(GetAllLibraryInchargeRequest request)
        {
            var response = await _libraryService.GetAllLibraryIncharge(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllLibrary_Fetch")]
        public async Task<IActionResult> GetAllLibraryFetch(GetAllLibraryFetchRequest request)
        {
            var response = await _libraryService.GetAllLibraryFetch(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
