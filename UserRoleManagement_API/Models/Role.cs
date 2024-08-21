namespace UserRoleManagement_API.Models
{
    public class Role
    {
        public int RoleID { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int InstituteID { get; set; }
    }
}
