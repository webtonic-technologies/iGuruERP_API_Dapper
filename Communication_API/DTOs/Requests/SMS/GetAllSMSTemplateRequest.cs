namespace Communication_API.DTOs.Requests.SMS
{
    public class GetAllSMSTemplateRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
