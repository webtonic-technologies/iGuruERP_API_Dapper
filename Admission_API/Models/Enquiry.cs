namespace Admission_API.Models
{
    public class EnquiryRequest
    {
        public int EnquirySetupID { get; set; }
        public int FieldTypeID { get; set; }
        public string FieldTypeValue { get; set; }
        public bool IsMultiChoice { get; set; }
        public int OptionID { get; set; }

        //public int LeadStageID { get; set; }
    }


    public class Enquiry
    {
        //public int EnquiryID { get; set; }
        //public int EnquiryGroupID { get; set; }
        //public int FieldTypeID { get; set; }
        //public string FieldTypeValue { get; set; }
        //public int LeadStageID { get; set; }
        //public DateTime FollowupDate { get; set; }
        //public string Comments { get; set; }


        public int EnquirySetupID { get; set; }
        public string FieldName { get; set; }
        public string FieldTypeValue { get; set; }
        public string Options { get; set; }
    }

    public class EnquirySMS
    {
        public int EnquirySMSID { get; set; }
        public int EnquiryID { get; set; }
        public int TemplateID { get; set; }
        public string SMSDetails { get; set; }
    }
}
