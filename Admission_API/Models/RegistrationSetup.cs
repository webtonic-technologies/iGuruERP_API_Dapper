namespace Admission_API.Models
{
    public class RegistrationSetup
    {
        public int RegistrationSetupID { get; set; }
        public string FieldName { get; set; }
        public int FieldTypeID { get; set; }
        public bool IsDefault { get; set; }
    }

    //public class FieldType
    //{
    //    public int FieldTypeID { get; set; }
    //    public string FieldTypeName { get; set; }
    //}
}
