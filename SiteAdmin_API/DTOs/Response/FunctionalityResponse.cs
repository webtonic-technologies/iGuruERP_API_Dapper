namespace SiteAdmin_API.DTOs.Response
{
    public class FunctionalityResponse
    {
        public int FunctionalityID { get; set; }
        public int SubModuleID { get; set; }
        public string? Functionality { get; set; }
        public bool IsActive { get; set; }
    }
}
