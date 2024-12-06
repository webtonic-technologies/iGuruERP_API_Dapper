namespace UserRoleManagement_API.DTOs.Requests
{
    public class CreateNewRoleRequest
    {
        public string UserRoleName { get; set; }
        public int ApplicationTypeID { get; set; }
        public int InstituteID { get; set; }  

        public List<ModuleSetting> Modules { get; set; }
    }

    public class ModuleSetting
    {
        public int ModuleID { get; set; }
        public List<SubmoduleSetting> Submodules { get; set; }
    }

    public class SubmoduleSetting
    {
        public int SubModuleID { get; set; }
    }
}
