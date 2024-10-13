namespace Transport_API.DTOs.Response
{
    public class GetStudentsReportResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public string RollNo { get; set; }
        public string FatherName { get; set; }
        public string MobileNo { get; set; }
        public string Status { get; set; }  // Allocated or Non-Allocated
    }
}
