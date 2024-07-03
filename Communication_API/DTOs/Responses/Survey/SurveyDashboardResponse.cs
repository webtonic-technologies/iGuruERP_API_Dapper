namespace Communication_API.DTOs.Responses.Survey
{
    public class SurveyDashboardResponse
    {
        public int SurveyID { get; set; }
        public string SurveyName { get; set; }
        public string Description { get; set; }
        public int ResponseCount { get; set; }
        public int InProgressCount { get; set; }
        public int PastCount { get; set; }
    }
}
