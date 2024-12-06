namespace UserRoleManagement_API.DTOs.Requests
{
    public class AssignRoleRequest
    {
        public int RoleID { get; set; }
        public List<int> EmployeeIDs { get; set; }  // Changed to a List<int> to allow multiple EmployeeIDs
    }
}
