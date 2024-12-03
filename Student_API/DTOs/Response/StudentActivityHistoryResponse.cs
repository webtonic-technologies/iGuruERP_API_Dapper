namespace Student_API.DTOs.Responses
{
    public class StudentActivityHistoryResponse
    {
        public int StatusID { get; set; }
        public string Status { get; set; }
        public string DateTime { get; set; }
        public int EmployeeID { get; set; }
        public string UserName { get; set; }
        public string Reason { get; set; }
    }
}
