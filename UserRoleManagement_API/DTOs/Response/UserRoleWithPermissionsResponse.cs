using System.Collections.Generic;

namespace UserRoleManagement_API.DTOs.Response
{
    public class UserRoleWithPermissionsResponse
    {
        public int RoleID { get; set; }
        public string UserRole { get; set; }
        public bool IsActive { get; set; }
        public int InstituteID { get; set; }
        public List<int> EmployeeIDs { get; set; }
    }
}
