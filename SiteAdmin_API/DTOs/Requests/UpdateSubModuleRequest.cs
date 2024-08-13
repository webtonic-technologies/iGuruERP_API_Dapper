namespace SiteAdmin_API.DTOs.Requests
{
    public class UpdateSubModuleRequest
    {
        public int SubModuleID { get; set; }
        public string SubModuleName { get; set; }
        public int ModuleID { get; set; }
        public bool IsActive { get; set; }
    }
}
