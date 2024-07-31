namespace SiteAdmin_API.Models
{
    public class Package
    {
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public bool IsActive { get; set; }
        public List<PackageModuleMapping> PackageModuleMappings { get; set; }
    }
}
