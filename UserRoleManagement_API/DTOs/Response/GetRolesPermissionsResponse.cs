namespace UserRoleManagement_API.DTOs.Responses
{
    public class GetRolesPermissionsResponse
    {
        public int RoleID { get; set; }
        public string UserRoleName { get; set; }
        public int ApplicationTypeID { get; set; }
        public string ApplicationType { get; set; }
        public int InstituteID { get; set; }
        public bool IsStudentRole { get; set; }
        public List<ModuleResponse1> Modules { get; set; }
    }

    public class ModuleResponse1
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public List<SubmoduleResponse1> Submodules { get; set; }
    }

    public class SubmoduleResponse1
    {
        public int SubModuleID { get; set; }
        public string SubModuleName { get; set; }
        public List<FunctionalityResponse1> Functionalities { get; set; }
    }

    public class FunctionalityResponse1
    {
        public int FunctionalityID { get; set; }
        public string Functionality { get; set; }
    }
}
