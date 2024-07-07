using Communication_API.DTOs.Requests.Survey;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Survey;
using Communication_API.Repository.Interfaces.Survey;
using Communication_API.Services.Interfaces.Survey;

namespace Communication_API.Services.Implementations.Survey
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;

        public SurveyService(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public async Task<ServiceResponse<string>> CreateSurvey(CreateSurveyRequest request)
        {
            return await _surveyRepository.CreateSurvey(request);
        }

        public async Task<ServiceResponse<int>> GetTotalResponseCount()
        {
            return await _surveyRepository.GetTotalResponseCount();
        }

        public async Task<ServiceResponse<int>> GetInProgressSurveysCount()
        {
            return await _surveyRepository.GetInProgressSurveysCount();
        }

        public async Task<ServiceResponse<int>> GetPastSurveysCount()
        {
            return await _surveyRepository.GetPastSurveysCount();
        }

        public async Task<ServiceResponse<List<Communication_API.Models.Survey.Survey>>> GetAllActiveSurveys(GetAllSurveysRequest request)
        {
            return await _surveyRepository.GetAllActiveSurveys(request);
        }

        public async Task<ServiceResponse<List<Communication_API.Models.Survey.Survey>>> GetAllScheduledSurveys(GetAllSurveysRequest request)
        {
            return await _surveyRepository.GetAllScheduledSurveys(request);
        }

        public async Task<ServiceResponse<List<Communication_API.Models.Survey.Survey>>> GetAllPastSurveys(GetAllSurveysRequest request)
        {
            return await _surveyRepository.GetAllPastSurveys(request);
        }

        public async Task<ServiceResponse<List<SurveyResponse>>> GetSurveysResponse(int SurveyID)
        {
            return await _surveyRepository.GetSurveysResponse(SurveyID);
        }
    }
}
