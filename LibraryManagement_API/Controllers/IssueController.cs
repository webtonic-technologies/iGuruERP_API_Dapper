using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibraryManagement_API.Controllers.Operations
{
    [Route("iGuru/Operations/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private readonly IIssueService _issueService;

        public IssueController(IIssueService issueService)
        {
            _issueService = issueService;
        }

        [HttpPost("GetAllIssueBook")]
        public async Task<IActionResult> GetAllIssueBooks(GetAllIssueBooksRequest request)
        {
            var response = await _issueService.GetAllIssueBooks(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("AddUpdateIssue")]
        public async Task<IActionResult> AddUpdateIssue(AddUpdateIssueRequest request)
        {
            var response = await _issueService.AddUpdateIssue(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
