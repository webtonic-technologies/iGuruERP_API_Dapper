namespace SiteAdmin_API.Models
{
    public class InstitutePackage
    {
        public int InstitutePackageID { get; set; }
        public int InstituteOnboardID { get; set; }
        public int PackageID { get; set; }
        public decimal MSG { get; set; }
        public decimal PSPA { get; set; }
        public decimal GST { get; set; }
        public decimal TotalDealValue { get; set; }
        public DateTime SignUpDate { get; set; }
        public DateTime ValidUpto { get; set; }
    }
}
