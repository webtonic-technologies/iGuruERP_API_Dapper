using Communication_API.DTOs.Requests.Survey;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Survey;

namespace Communication_API.Services.Interfaces.Survey
{
    public interface ISurveyService
    {
        Task<ServiceResponse<string>> CreateSurvey(CreateSurveyRequest request);
        Task<ServiceResponse<int>> GetTotalResponseCount();
        Task<ServiceResponse<int>> GetInProgressSurveysCount();
        Task<ServiceResponse<int>> GetPastSurveysCount();
        Task<ServiceResponse<List<Communication_API.Models.Survey.Survey>>> GetAllActiveSurveys(GetAllSurveysRequest request);
        Task<ServiceResponse<List<Communication_API.Models.Survey.Survey>>> GetAllScheduledSurveys(GetAllSurveysRequest request);
        Task<ServiceResponse<List<Communication_API.Models.Survey.Survey>>> GetAllPastSurveys(GetAllSurveysRequest request);
        Task<ServiceResponse<List<SurveyResponse>>> GetSurveysResponse(int SurveyID);
    }
}
