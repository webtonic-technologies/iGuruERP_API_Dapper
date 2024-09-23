namespace Institute_API.Models
{
    public class InstituteAddress
    {
        public int Institute_address_id { get; set; }
        public int Institute_id { get; set; }
        public int country_id { get; set; }
        public int state_id { get; set; }
        public int city_id { get; set; }
        public string house { get; set; } = string.Empty;
        public string pincode { get; set; } = string.Empty;
        public int district_id { get; set; }
       // public string Locality { get; set; } = string.Empty;
      //  public string Landmark { get; set; } = string.Empty;
        public string Mobile_number { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? AddressType_id { get; set; }
        public DateTime? en_date { get; set; }
    }
}
