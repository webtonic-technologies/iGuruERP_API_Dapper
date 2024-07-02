namespace Communication_API.Models.Email
{
    public class EmailConfiguration
    {
        public int ConfigurationID { get; set; }
        public string SMTPUserName { get; set; }
        public string SMTPPassword { get; set; }
        public string SMTPServer { get; set; }
        public string SMTPPort { get; set; }
        public string Security { get; set; }
    }
}
