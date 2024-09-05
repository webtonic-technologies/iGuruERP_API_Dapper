namespace SiteAdmin_API.Models
{
    public class Package
    {
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public bool IsActive { get; set; }
        public List<PackageModuleMapping> PackageModuleMappings { get; set; }
    }

    public class PackageModuleMapping
    {
        public int PMMID { get; set; }
        public int PackageID { get; set; }
        public int ModuleID { get; set; }
        public int SubModuleID { get; set; }
    }
}
