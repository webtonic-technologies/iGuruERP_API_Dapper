using Communication_API.DTOs.Requests.Survey;
using Communication_API.Services.Interfaces.Survey;
using Microsoft.AspNetCore.Mvc;

namespace Communication_API.Controllers
{
    [Route("iGuru/Survey/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly ISurveyService _surveyService;

        public SurveyController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        [HttpPost("CreateSurvey")]
        public async Task<IActionResult> CreateSurvey([FromBody] CreateSurveyRequest request)
        {
            var response = await _surveyService.CreateSurvey(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetTotalResponseCount")]
        public async Task<IActionResult> GetTotalResponseCount()
        {
            var response = await _surveyService.GetTotalResponseCount();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetInProgressSurveysCount")]
        public async Task<IActionResult> GetInProgressSurveysCount()
        {
            var response = await _surveyService.GetInProgressSurveysCount();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetPastSurveysCount")]
        public async Task<IActionResult> GetPastSurveysCount()
        {
            var response = await _surveyService.GetPastSurveysCount();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllActiveSurveys")]
        public async Task<IActionResult> GetAllActiveSurveys([FromBody] GetAllSurveysRequest request)
        {
            var response = await _surveyService.GetAllActiveSurveys(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllScheduledSurveys")]
        public async Task<IActionResult> GetAllScheduledSurveys([FromBody] GetAllSurveysRequest request)
        {
            var response = await _surveyService.GetAllScheduledSurveys(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllPastSurveys")]
        public async Task<IActionResult> GetAllPastSurveys([FromBody] GetAllSurveysRequest request)
        {
            var response = await _surveyService.GetAllPastSurveys(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetSurveysResponse/{SurveyID}")]
        public async Task<IActionResult> GetSurveysResponse(int SurveyID)
        {
            var response = await _surveyService.GetSurveysResponse(SurveyID);
            return StatusCode(response.StatusCode, response);
        }
    }
}
