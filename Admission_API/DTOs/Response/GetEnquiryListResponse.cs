namespace Admission_API.DTOs.Response
{

    public class GetEnquiryListResponse
    {
        public string LeadStage { get; set; }  // Grouping leads by their stage
        public List<GetLeadDetails> Leads { get; set; }  // List of leads under each LeadStage
    }

    public class GetLeadDetails
    {
        public int LeadID { get; set; }
        public string LeadCode { get; set; }
        public int LeadStageID { get; set; }
        public string LeadStage { get; set; }
        public List<GetEnquiryList> EnquiryInfo { get; set; } // List of enquiry info for each lead
    }

    public class GetEnquiryList
    {
        public int EnquirySetupID { get; set; }
        public string FieldName { get; set; }
        public string FieldTypeValue { get; set; }
        public string Options { get; set; } // Assuming Options is a string
    }

    //public class GetEnquiryListResponse
    //{
    //    public int LeadID { get; set; }
    //    public string LeadCode { get; set; }
    //    public int LeadStageID { get; set; }
    //    public string LeadStage { get; set; }
    //    public List<GetEnquiryList> EnquiryInfo { get; set; } // List of LeadInfo for each lead
    //}

    //public class GetEnquiryList
    //{
    //    public int EnquirySetupID { get; set; }
    //    public string FieldName { get; set; }
    //    public string FieldTypeValue { get; set; }
    //    public string Options { get; set; } // Assuming Options is a string
    //}
}

 
