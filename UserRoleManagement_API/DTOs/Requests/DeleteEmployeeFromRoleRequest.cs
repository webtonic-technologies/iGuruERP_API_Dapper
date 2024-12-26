namespace UserRoleManagement_API.DTOs.Requests
{
    public class DeleteEmployeeFromRoleRequest
    {
        public int RoleID { get; set; }
        public int EmployeeID { get; set; }
    }
}
