namespace UserRoleManagement_API.DTOs.Response
{
    public class CreateNewRoleResponse
    {
        public int RoleID { get; set; }
        public string UserRoleName { get; set; }
        public int ApplicationTypeID { get; set; }
        public int InstituteID { get; set; }

        public List<ModuleResponse> Modules { get; set; }
    }

    public class ModuleResponse
    {
        public int ModuleID { get; set; }
        public List<SubmoduleResponse> Submodules { get; set; }
    }

    public class SubmoduleResponse
    {
        public int SubModuleID { get; set; }
    }
}
