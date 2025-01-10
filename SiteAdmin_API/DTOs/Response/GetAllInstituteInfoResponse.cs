namespace SiteAdmin_API.DTOs.Responses
{
    public class GetAllInstituteInfoResponse
    {
        public string InstituteOnboardName { get; set; }
        public string AliasName { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
    }
}
