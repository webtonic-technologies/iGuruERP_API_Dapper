namespace Communication_API.DTOs.Responses.Email
{
    public class EmailStudentReportsResponse
    {
        public int? StudentID { get; set; } // Nullable for Employee reports
        public string AdmissionNumber { get; set; } // Added for Admission Number
        public string StudentName { get; set; } // Nullable for Employee reports
        public string ClassSection { get; set; } // Nullable for Employee reports   
        public string DateTime { get; set; }  // Format '15 Dec 2024, 05:00 PM'
        public string EmailSubject { get; set; }
        public string EmailID { get; set; } 
        public string Status { get; set; }
    }
}
