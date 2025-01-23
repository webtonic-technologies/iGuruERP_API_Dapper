namespace Admission_API.DTOs.Response
{

    public class RegistrationSetupResponse
    {
        public int RegistrationSetupID { get; set; }
        public string FieldName { get; set; }
        public int FieldTypeID { get; set; }
        public string FieldType { get; set; }
        public bool IsMultiChoice { get; set; }
        public bool IsInForm { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
        public List<RegistrationOptionResponse> Options { get; set; }
    }

    public class RegistrationOptionResponse
    {
        public int OptionID { get; set; }
        public string Options { get; set; }
        public object RegistrationSetupID { get; internal set; }
    } 
}
