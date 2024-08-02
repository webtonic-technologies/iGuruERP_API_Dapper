namespace SiteAdmin_API.DTOs.Requests
{
    public class CreatePackageRequest
    {
        public string PackageName { get; set; }
        public List<CreatePackageModuleMappingRequest> PackageModuleMappings { get; set; }
    }

    public class CreatePackageModuleMappingRequest
    {
        public int ModuleID { get; set; }
        public int SubModuleID { get; set; }
    }
}
