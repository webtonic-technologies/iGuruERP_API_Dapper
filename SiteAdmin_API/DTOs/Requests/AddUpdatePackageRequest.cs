namespace SiteAdmin_API.DTOs.Requests
{
    public class AddUpdatePackageRequest
    {
        public int? PackageID { get; set; }
        public string PackageName { get; set; }
        public bool IsActive { get; set; }
        public List<PackageModuleMappingRequest> PackageModuleMappings { get; set; }
    }

    public class PackageModuleMappingRequest
    {
        public int ModuleID { get; set; }
        public int SubModuleID { get; set; }
    }
}
