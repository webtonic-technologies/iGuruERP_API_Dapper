namespace Communication_API.DTOs.Requests.SMS
{
    public class SendSMSStudentRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public List<int> StudentIDs { get; set; }
        public string SMSMessage { get; set; }
        public string SMSDate { get; set; } // Changed to string to match request format
    }
}
