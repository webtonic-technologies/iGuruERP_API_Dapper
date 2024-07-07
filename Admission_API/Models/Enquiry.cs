namespace Admission_API.Models
{
    public class Enquiry
    {
        public int EnquiryID { get; set; }
        public int EnquiryGroupID { get; set; }
        public int FieldTypeID { get; set; }
        public string FieldTypeValue { get; set; }
        public int LeadStageID { get; set; }
        public DateTime FollowupDate { get; set; }
        public string Comments { get; set; }
    }

    public class EnquirySMS
    {
        public int EnquirySMSID { get; set; }
        public int EnquiryID { get; set; }
        public int TemplateID { get; set; }
        public string SMSDetails { get; set; }
    }
}
