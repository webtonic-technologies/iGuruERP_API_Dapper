namespace Communication_API.DTOs.Requests.SMS
{
    //public class SendSMSEmployeeRequest
    //{
    //    public int GroupID { get; set; }
    //    public int InstituteID { get; set; }
    //    public List<int> EmployeeIDs { get; set; }
    //    public string SMSMessage { get; set; }
    //    public string SMSDate { get; set; } // Date in string format (DD-MM-YYYY)
    //}

    public class SendSMSEmployeeRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public List<EmployeeMessage> EmployeeMessages { get; set; }  // Renamed property to EmployeeMessages
        public string SMSDate { get; set; } // Changed to string to match request format
    }

    public class EmployeeMessage
    {
        public int EmployeeID { get; set; }
        public string Message { get; set; }  // Renamed property to Message
    }
}
