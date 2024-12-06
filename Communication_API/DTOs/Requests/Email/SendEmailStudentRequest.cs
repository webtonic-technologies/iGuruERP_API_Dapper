namespace Communication_API.DTOs.Requests.Email
{
    public class SendEmailStudentRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public List<int> StudentIDs { get; set; }  // List of student IDs
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailDate { get; set; } // Date in the string format (e.g., "19-11-2024")
    }
}
