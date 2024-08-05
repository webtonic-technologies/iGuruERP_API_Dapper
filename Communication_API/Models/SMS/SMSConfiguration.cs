namespace Communication_API.Models.SMS
{
    public class SMSConfiguration
    {
        public int ConfigID { get; set; }
        public string APIkey { get; set; }
        public int UserID { get; set; }
        public int SenderID { get; set; }
        public bool Status { get; set; }
    }
}
