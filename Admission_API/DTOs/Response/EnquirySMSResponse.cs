namespace Admission_API.DTOs.Response
{
    public class EnquirySMSResponse
    {
        public int EnquirySMSID { get; set; }
        public int EnquiryID { get; set; }
        public int TemplateID { get; set; }
        public string SMSDetails { get; set; }
    }
}
