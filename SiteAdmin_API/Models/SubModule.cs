namespace SiteAdmin_API.Models
{
    public class SubModule
    {
        public int SubModuleId { get; set; }
        public int ModuleId { get; set; }
        public string SubModuleName { get; set; }
        public bool IsActive { get; set; }
    }
}
