namespace SiteAdmin_API.DTOs.Response
{
    public class InstituteOnboardResponse
    {
        public int InstituteOnboardID { get; set; }
        public string InstituteOnboardName { get; set; }
        public string AliasName { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public List<InstituteOnboardContactResponse> InstituteOnboardContacts { get; set; }
        public List<InstituteOnboardCredentialsResponse> InstituteOnboardCredentials { get; set; }
        public List<InstitutePackageResponse> InstitutePackages { get; set; }
    }

    public class InstituteOnboardContactResponse
    {
        public int ContactID { get; set; }
        public string PrimaryContactName { get; set; }
        public string PrimaryTelephoneNumber { get; set; }
        public string PrimaryMobileNumber { get; set; }
        public string PrimaryEmailID { get; set; }
        public string SecondaryContactName { get; set; }
        public string SecondaryTelephoneNumber { get; set; }
        public string SecondaryMobileNumber { get; set; }
        public string SecondaryEmailID { get; set; }
    }

    public class InstituteOnboardCredentialsResponse
    {
        public int CredentialID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class InstitutePackageResponse
    {
        public int InstitutePackageID { get; set; }
        public int PackageID { get; set; }
        public decimal MSG { get; set; }
        public decimal PSPA { get; set; }
        public decimal GST { get; set; }
        public decimal TotalDealValue { get; set; }
        public DateTime SignUpDate { get; set; }
        public DateTime ValidUpto { get; set; }
    }
}
