namespace SiteAdmin_API.Models
{
    public class Module
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int ModuleOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
