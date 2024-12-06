namespace Communication_API.DTOs.Requests.SMS
{
    public class UpdateSMSEmployeeStatusRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public int EmployeeID { get; set; }
        public int SMSStatusID { get; set; }
    }
}
