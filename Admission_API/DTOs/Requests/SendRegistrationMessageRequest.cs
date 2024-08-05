namespace Admission_API.DTOs.Requests
{
    public class SendRegistrationMessageRequest
    {
        public int RegistrationID { get; set; }
        public int TemplateID { get; set; }
        public string SMSDetails { get; set; }
    }
}
