namespace StudentManagement_API.DTOs.Responses
{
    public class GetStudentImportHistoryResponse
    {
        public int StudentCount { get; set; }
        public string AcademicYearCode { get; set; }
        // DateTime formatted as "dd-MM-yyyy at hh:mm tt"
        public string DateTime { get; set; }
        public string IPAddress { get; set; }
        public string UserName { get; set; }
    }
}
