namespace Communication_API.DTOs.Requests.Email
{
    //public class SendEmailStudentRequest
    //{
    //    public int GroupID { get; set; }
    //    public int InstituteID { get; set; }
    //    public List<int> StudentIDs { get; set; }  // List of student IDs
    //    public string EmailSubject { get; set; }
    //    public string EmailBody { get; set; }
    //    public string EmailDate { get; set; } // Date in the string format (e.g., "19-11-2024")
    //}



    public class SendEmailStudentRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public List<StudentEmail> StudentEmail { get; set; }  // Renamed to avoid conflict
        public string EmailDate { get; set; } // Changed to string to match request format
    }

    public class StudentEmail
    {
        public int StudentID { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
    }
}
