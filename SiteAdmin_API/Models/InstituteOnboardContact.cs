namespace SiteAdmin_API.Models
{
    public class InstituteOnboardContact
    {
        public int ContactID { get; set; }
        public int InstituteOnboardID { get; set; }
        public string PrimaryContactName { get; set; }
        public string PrimaryTelephoneNumber { get; set; }
        public string PrimaryMobileNumber { get; set; }
        public string PrimaryEmailID { get; set; }
        public string SecondaryContactName { get; set; }
        public string SecondaryTelephoneNumber { get; set; }
        public string SecondaryMobileNumber { get; set; }
        public string SecondaryEmailID { get; set; }
    }
}
