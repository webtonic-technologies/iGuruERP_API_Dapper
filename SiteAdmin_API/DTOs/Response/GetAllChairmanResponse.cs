namespace SiteAdmin_API.DTOs.Responses
{
    public class GetAllChairmanResponse
    {
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string EmailID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<InstituteResponse> Institutes { get; set; }
    }

    public class InstituteResponse
    {
        public int InstituteID { get; set; }
        public string InstituteName { get; set; }
    }
}
