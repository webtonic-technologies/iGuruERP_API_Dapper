namespace Communication_API.DTOs.Responses.SMS
{
    public class SMSConfigurationResponse
    {
        public int ConfigID { get; set; }
        public string APIkey { get; set; }
        public int UserID { get; set; }
        public int SenderID { get; set; }
        public bool Status { get; set; }
    }
}
