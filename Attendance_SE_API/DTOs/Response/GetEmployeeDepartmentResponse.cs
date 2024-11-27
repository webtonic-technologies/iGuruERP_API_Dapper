namespace Attendance_SE_API.DTOs.Responses
{
    public class GetEmployeeDepartmentResponse
    {
        public List<Department> Departments { get; set; }
    }

    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } 
    }
}
