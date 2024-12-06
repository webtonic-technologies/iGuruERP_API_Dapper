namespace Communication_API.DTOs.Requests.SMS
{
    public class SendSMSEmployeeRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public List<int> EmployeeIDs { get; set; }
        public string SMSMessage { get; set; }
        public string SMSDate { get; set; } // Date in string format (DD-MM-YYYY)
    }
}
