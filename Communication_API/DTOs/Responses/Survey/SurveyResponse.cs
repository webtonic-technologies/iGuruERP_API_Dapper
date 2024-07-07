namespace Communication_API.DTOs.Responses.Survey
{
    public class SurveyResponse
    {
        public int SurveyID { get; set; }
        public string SurveyName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsStudent { get; set; }
        public bool IsEmployee { get; set; }
    }
}
