namespace SiteAdmin_API.DTOs.Response
{
    public class ModuleResponse
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
