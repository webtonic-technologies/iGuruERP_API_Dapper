namespace StudentManagement_API.DTOs.Responses
{
    public class GetCertificateReportResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string DateOfGeneration { get; set; } // Format: DD-MM-YYYY
    }
}
