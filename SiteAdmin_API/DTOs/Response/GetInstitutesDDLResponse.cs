namespace SiteAdmin_API.DTOs.Responses
{
    public class GetInstitutesDDLResponse
    {
        public int InstituteOnboardID { get; set; }
        public string InstituteOnboardName { get; set; }
    }
    public class CreateUserResponse
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
