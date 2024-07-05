namespace Admission_API.Models
{
    public class EnquirySetup
    {
        public int EnquirySetupID { get; set; }
        public string FieldName { get; set; }
        public int FieldTypeID { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
    }

    //public class FieldType
    //{
    //    public int FieldTypeID { get; set; }
    //    public string FieldTypeName { get; set; }
    //}
}
