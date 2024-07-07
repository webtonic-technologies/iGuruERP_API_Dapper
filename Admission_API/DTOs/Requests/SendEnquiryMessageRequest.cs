namespace Admission_API.DTOs.Requests
{
    public class SendEnquiryMessageRequest
    {
        public int EnquiryID { get; set; }
        public int TemplateID { get; set; }
        public string SMSDetails { get; set; }
    }
}
