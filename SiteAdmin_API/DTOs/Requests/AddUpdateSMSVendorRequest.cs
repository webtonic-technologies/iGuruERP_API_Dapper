namespace SiteAdmin_API.DTOs.Requests
{
    public class AddUpdateSMSVendorRequest
    {
        public int SMSVendorID { get; set; }
        public string VendorName { get; set; }
        public decimal PerSMSCost { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
