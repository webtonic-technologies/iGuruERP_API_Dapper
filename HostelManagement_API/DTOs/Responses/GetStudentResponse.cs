namespace HostelManagement_API.DTOs.Responses
{
    public class GetStudentResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string ClassSection { get; set; }
        public string AdmissionNumber { get; set; }
        public string RollNumber { get; set; }
        public string StudentType { get; set; }
    }
}
