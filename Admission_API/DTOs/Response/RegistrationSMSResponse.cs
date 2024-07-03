namespace Admission_API.DTOs.Response
{
    public class RegistrationSMSResponse
    {
        public int RegistrationSMSID { get; set; }
        public int RegistrationID { get; set; }
        public int TemplateID { get; set; }
        public string SMSDetails { get; set; }
    }
}
