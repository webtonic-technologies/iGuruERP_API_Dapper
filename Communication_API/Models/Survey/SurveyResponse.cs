namespace Communication_API.Models.Survey
{
    public class SurveyResponse
    {
        public int SurveyResponseID { get; set; }
        public int SurveyID { get; set; }
        public string Response { get; set; }
        public DateTime ResponseDate { get; set; }
    }
}
