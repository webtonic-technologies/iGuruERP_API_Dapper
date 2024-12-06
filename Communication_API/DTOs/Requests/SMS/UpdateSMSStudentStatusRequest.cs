namespace Communication_API.DTOs.Requests.SMS
{
    public class UpdateSMSStudentStatusRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public int StudentID { get; set; }
        public int SMSStatusID { get; set; }
    }
}
