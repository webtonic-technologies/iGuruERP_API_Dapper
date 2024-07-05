namespace Communication_API.DTOs.Requests.Email
{
    public class ConfigureEmailRequest
    {
        public string SMTPUserName { get; set; }
        public string SMTPPassword { get; set; }
        public string SMTPServer { get; set; }
        public string SMTPPort { get; set; }
        public string Security { get; set; }
    }
}
