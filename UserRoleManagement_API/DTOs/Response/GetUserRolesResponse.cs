namespace UserRoleManagement_API.DTOs.Response
{
    public class GetUserRolesResponse
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public List<EmployeeResponse> Employees { get; set; }
    }

    public class EmployeeResponse
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
    }
}
