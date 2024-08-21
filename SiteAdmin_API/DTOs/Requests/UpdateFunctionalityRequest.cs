namespace SiteAdmin_API.DTOs.Requests
{
    public class UpdateFunctionalityRequest
    {
        public int FunctionalityID { get; set; }
        public string Functionality { get; set; }
        public int SubModuleID { get; set; }
        public bool IsActive { get; set; }
    }
}
