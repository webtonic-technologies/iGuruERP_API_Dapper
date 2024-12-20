namespace VisitorManagement_API.DTOs.Responses
{
    public class GetEmployeeResponse
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
    }
}
