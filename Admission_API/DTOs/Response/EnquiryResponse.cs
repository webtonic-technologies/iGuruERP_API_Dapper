namespace Admission_API.DTOs.Response
{
    public class EnquiryResponse
    {
        public int EnquiryID { get; set; }
        public int EnquiryGroupID { get; set; }
        public int FieldTypeID { get; set; }
        public string FieldTypeValue { get; set; }
        public int LeadStageID { get; set; }
        public DateTime FollowupDate { get; set; }
        public string Comments { get; set; }
    }
}
