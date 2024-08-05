using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.Models;
using LibraryManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpPost("AddUpdateLanguage")]
        public async Task<IActionResult> AddUpdateLanguage(Language request)
        {
            var response = await _languageService.AddUpdateLanguage(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllLanguage")]
        public async Task<IActionResult> GetAllLanguage(GetAllLanguageRequest request)
        {
            var response = await _languageService.GetAllLanguages(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetLanguage/{LanguageID}")]
        public async Task<IActionResult> GetLanguage(int LanguageID)
        {
            var response = await _languageService.GetLanguageById(LanguageID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{LanguageID}")]
        public async Task<IActionResult> DeleteLanguage(int LanguageID)
        {
            var response = await _languageService.DeleteLanguage(LanguageID);
            return StatusCode(response.StatusCode, response);
        }
    }
}
