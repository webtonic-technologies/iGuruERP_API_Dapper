namespace Communication_API.DTOs.Requests.Email
{
    //public class SendEmailEmployeeRequest
    //{
    //    public int GroupID { get; set; }
    //    public int InstituteID { get; set; }
    //    public List<int> EmployeeIDs { get; set; }  // List of employee IDs
    //    public string EmailSubject { get; set; }
    //    public string EmailBody { get; set; }
    //    public string EmailDate { get; set; } // Date in the string format (e.g., "19-11-2024")
    //}

    public class SendEmailEmployeeRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public List<EmployeeEmail> EmployeeEmail { get; set; }  // Renamed to avoid conflict
        public string EmailDate { get; set; } // Changed to string to match request format
    }

    public class EmployeeEmail
    {
        public int EmployeeID { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
    }
}
