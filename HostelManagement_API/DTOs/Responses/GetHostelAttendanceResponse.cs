namespace HostelManagement_API.DTOs.Responses
{
    public class GetHostelAttendanceResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string ClassSection { get; set; }
        public int StatusID { get; set; }
        public string Remarks { get; set; }
    }
}
