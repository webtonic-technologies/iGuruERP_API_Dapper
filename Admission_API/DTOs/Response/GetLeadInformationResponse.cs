namespace Admission_API.DTOs.Response
{
    public class GetLeadInformationResponse
    {
        public int LeadID { get; set; }
        public string LeadCode { get; set; }
        public int LeadStageID { get; set; }
        public string LeadStage { get; set; }
        public List<GetLeadEnquiry> EnquiryInfo { get; set; } // List of LeadInfo for each lead
        public List<GetLeadComments> LeadComments { get; set; } // List of Lead comments for each lead

    }

    public class GetLeadEnquiry
    {
        public int EnquirySetupID { get; set; }
        public string FieldName { get; set; }
        public string FieldTypeValue { get; set; }
        public string Options { get; set; } // Assuming Options is a string
    }

    public class GetLeadComments
    {
        public int LeadStageID { get; set; }
        public string LeadStage { get; set; }
        public string Comments { get; set; }
        public string FollowupDate { get; set; } // Format: DD-MM-YYYY
    }
}

