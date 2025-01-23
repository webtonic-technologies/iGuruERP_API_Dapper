namespace Admission_API.DTOs.Response
{
    public class EnquirySetupResponse
    {
        public int EnquirySetupID { get; set; }
        public string FieldName { get; set; }
        public int FieldTypeID { get; set; }
        public string FieldType { get; set; }
        public bool IsMultiChoice { get; set; }
        public bool IsInForm { get; set; } 
        public bool IsMandatory { get; set; }  
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
        public List<EnquiryOptionResponse> Options { get; set; }
    }

    public class EnquiryOptionResponse
    {
        public int OptionID { get; set; }  
        public string Options { get; set; }
        public object EnquirySetupID { get; internal set; }
    }
}
