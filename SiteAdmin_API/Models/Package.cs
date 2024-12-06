//namespace SiteAdmin_API.Models
//{
//    public class Package
//    {
//        public int PackageID { get; set; }
//        public string PackageName { get; set; }
//        public bool IsActive { get; set; }
//        public List<PackageModuleMapping> PackageModuleMappings { get; set; }
//    }

//    public class PackageModuleMapping
//    {
//        public int PMMID { get; set; }
//        public int PackageID { get; set; }
//        public int ModuleID { get; set; }
//        public int SubModuleID { get; set; }
//    }
//}


using System.Reflection;

namespace SiteAdmin_API.Models
{
    public class Package
    {
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public bool IsActive { get; set; } 
        public List<PackageModuleMapping> PackageModuleMappings { get; set; }
    }

    public class GetPackage
    {
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public bool IsActive { get; set; }
        public List<Modules> Modules { get; set; }  // Updated to contain a list of Modules objects 
    }

    public class PackageModuleMapping
    {
        public int PMMID { get; set; }
        public int PackageID { get; set; }
        public int ModuleID { get; set; }
        public int SubModuleID { get; set; }
    }

    // This class is no longer needed since Module information is now in the Module class
    public class PackageModuleMappingWithModule
    {
        public int PMMID { get; set; }
        public int PackageID { get; set; }
        public int ModuleID { get; set; }
        public int SubModuleID { get; set; }
        public string ModuleName { get; set; }
    }

    public class Modules
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
    }
}
