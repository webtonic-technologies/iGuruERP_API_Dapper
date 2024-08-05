namespace SiteAdmin_API.Models
{
    public class InstituteOnboard
    {
        public int InstituteOnboardID { get; set; }
        public string InstituteOnboardName { get; set; }
        public string AliasName { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public List<InstituteOnboardContact> InstituteOnboardContacts { get; set; }
        public List<InstituteOnboardCredentials> InstituteOnboardCredentials { get; set; }
        public List<InstitutePackage> InstitutePackages { get; set; }
    }
}
