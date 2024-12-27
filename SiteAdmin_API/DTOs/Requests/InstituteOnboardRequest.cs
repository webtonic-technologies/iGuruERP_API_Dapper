namespace SiteAdmin_API.DTOs.Requests
{
    public class InstituteOnboardRequest
    {
        public int? InstituteOnboardID { get; set; }
        public string InstituteOnboardName { get; set; }
        public string AliasName { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public List<InstituteOnboardContactRequest> InstituteOnboardContacts { get; set; }
        public List<InstituteOnboardCredentialsRequest> InstituteOnboardCredentials { get; set; }
        public List<InstitutePackageRequest> InstitutePackages { get; set; }
    }

    public class InstituteOnboardContactRequest
    {
        public string PrimaryContactName { get; set; }
        public string PrimaryTelephoneNumber { get; set; }
        public string PrimaryMobileNumber { get; set; }
        public string PrimaryEmailID { get; set; }
        public string SecondaryContactName { get; set; }
        public string SecondaryTelephoneNumber { get; set; }
        public string SecondaryMobileNumber { get; set; }
        public string SecondaryEmailID { get; set; }
    }

    public class InstituteOnboardCredentialsRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class InstitutePackageRequest
    {
        public int PackageID { get; set; }
        public decimal MSG { get; set; }
        public decimal PSPA { get; set; }
        public decimal GST { get; set; }
        public decimal TotalDealValue { get; set; }
        public string SignUpDate { get; set; }  // Changed to string
        public string ValidUpto { get; set; }   // Changed to string
    }
}
