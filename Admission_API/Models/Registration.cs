namespace Admission_API.Models
{
    public class Registration
    {
        public int RegistrationID { get; set; }
        public int RegistrationGroupID { get; set; }
        public int FieldTypeID { get; set; }
        public string FieldTypeValue { get; set; }
    }

    public class RegistrationSMS
    {
        public int RegistrationSMSID { get; set; }
        public int RegistrationID { get; set; }
        public int TemplateID { get; set; }
        public string SMSDetails { get; set; }
    }
}
