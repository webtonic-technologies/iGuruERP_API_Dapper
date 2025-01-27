using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibraryManagement_API.Controllers.Operations
{
    [Route("iGuru/Operations/[controller]")]
    [ApiController]
    public class ReturnController : ControllerBase
    {
        private readonly IReturnService _returnService;

        public ReturnController(IReturnService returnService)
        {
            _returnService = returnService;
        }

        [HttpPost("GetReturnStudentBook")]
        public async Task<IActionResult> GetReturnStudentBook(GetReturnStudentBookRequest request)
        {
            var response = await _returnService.GetReturnStudentBook(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetReturnEmployeeBook")]
        public async Task<IActionResult> GetReturnEmployeeBook(GetReturnEmployeeBookRequest request)
        {
            var response = await _returnService.GetReturnEmployeeBook(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetReturnGuestBook")]
        public async Task<IActionResult> GetReturnGuestBook(GetReturnGuestBookRequest request)
        {
            var response = await _returnService.GetReturnGuestBook(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("CollectBook")]
        public async Task<IActionResult> CollectBook([FromBody] CollectBookRequest request)
        {
            var response = await _returnService.CollectBook(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
