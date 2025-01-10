namespace SiteAdmin_API.DTOs.Requests
{
    public class UpgradePackageRequest
    {
        public int InstitutePackageID { get; set; }
        public int InstituteOnboardID { get; set; }
        public int PackageID { get; set; }
        public decimal MSG { get; set; }
        public decimal PSPA { get; set; }
        public decimal GST { get; set; }
        public decimal TotalDealValue { get; set; }
        public string SignUpDate { get; set; }
        public string ValidUpto { get; set; }
        public string Comment { get; set; }
    }
}
