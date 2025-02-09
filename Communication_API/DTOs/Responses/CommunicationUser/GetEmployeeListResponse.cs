namespace Communication_API.DTOs.Responses
{
    public class GetEmployeeListResponse
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
    }
}
