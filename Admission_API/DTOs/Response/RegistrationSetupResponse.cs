namespace Admission_API.DTOs.Response
{
    public class RegistrationSetupResponse
    {
        public int RegistrationSetupID { get; set; }
        public string FieldName { get; set; }
        public int FieldTypeID { get; set; }
        public bool IsDefault { get; set; }
    }
}
