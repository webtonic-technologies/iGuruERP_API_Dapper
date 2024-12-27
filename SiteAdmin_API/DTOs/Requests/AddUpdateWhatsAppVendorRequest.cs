namespace SiteAdmin_API.DTOs.Requests
{
    public class AddUpdateWhatsAppVendorRequest
    {
        public int WhatsAppVendorID { get; set; }
        public string VendorName { get; set; }
        public decimal PerWhatsAppCost { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
