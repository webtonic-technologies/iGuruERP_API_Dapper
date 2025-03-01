namespace StudentManagement_API.DTOs.Responses
{
    public class GetStudentsResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }   
        public string Class { get; set; }
        public string Section { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; } // Format: DD-MM-YYYY
        public string Status { get; set; } // "Generated" if certificate exists, else "Pending"
    }
}
