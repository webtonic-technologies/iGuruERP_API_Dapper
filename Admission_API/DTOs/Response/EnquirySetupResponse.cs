namespace Admission_API.DTOs.Response
{
    public class EnquirySetupResponse
    {
        public int EnquirySetupID { get; set; }
        public string FieldName { get; set; }
        public int FieldTypeID { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
    }
}
