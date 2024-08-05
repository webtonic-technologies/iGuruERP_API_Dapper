namespace SiteAdmin_API.DTOs.Response
{
    public class CreatePackageResponse
    {
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public bool IsActive { get; set; }
        public List<PackageModuleMappingResponse> PackageModuleMappings { get; set; }
    }

    public class PackageModuleMappingResponse
    {
        public int PMMID { get; set; }
        public int ModuleID { get; set; }
        public int SubModuleID { get; set; }
    }
}
