using UserRoleManagement_API.Models; // Add this using directive
using System.Collections.Generic;

namespace UserRoleManagement_API.DTOs.Requests
{
    public class SetRolePermissionRequest
    {
        public Role Role { get; set; }
        public List<UserRoleMapping> UserRoleMappings { get; set; }
    }
}
