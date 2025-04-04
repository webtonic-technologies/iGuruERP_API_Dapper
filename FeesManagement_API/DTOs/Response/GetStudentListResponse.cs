namespace FeesManagement_API.DTOs.Responses
{
    public class GetStudentListResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string RollNumber { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
    }
}
