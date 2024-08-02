namespace SiteAdmin_API.DTOs.Response
{
    public class SubModuleResponse
    {
        public int SubModuleId { get; set; }
        public int ModuleId { get; set; }
        public string SubModuleName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
