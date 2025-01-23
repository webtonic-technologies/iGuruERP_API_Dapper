namespace Admission_API.DTOs.Response
{
    public class GetAllEnquiryResponse
    {  
        public int LeadID { get; set; }
        public string LeadCode { get; set; }
        public int LeadStageID { get; set; }
        public string LeadStage { get; set; }
        public List<GetEnquiry> EnquiryInfo { get; set; } // List of LeadInfo for each lead
    }

    public class GetEnquiry
    {
        public int EnquirySetupID { get; set; }
        public string FieldName { get; set; }
        public string FieldTypeValue { get; set; }
        public string Options { get; set; } // Assuming Options is a string
    }
}
