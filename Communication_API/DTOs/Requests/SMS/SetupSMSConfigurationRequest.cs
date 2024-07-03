namespace Communication_API.DTOs.Requests.SMS
{
    public class SetupSMSConfigurationRequest
    {
        public string APIkey { get; set; }
        public int UserID { get; set; }
        public int SenderID { get; set; }
        public bool Status { get; set; }
    }
}
