namespace Admission_API.Models
{
    public class EnquirySetup
    {
        public int EnquirySetupID { get; set; }
        public string FieldName { get; set; }
        public int FieldTypeID { get; set; }
        public bool IsMultiChoice { get; set; }
        public int InstituteID { get; set; }
        public List<EnquiryOption> Options { get; set; } // Options field added for multi-choice options
    }

    public class EnquiryOption
    {
        public int EnquirySetupID { get; set; }
        public int FieldTypeID { get; set; }
        public string Options { get; set; }
    }
}
