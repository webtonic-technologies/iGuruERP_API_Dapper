namespace Institute_API.Models
{
    public class SchoolContact
    {
        public int School_Contact_id { get; set; }
        public int ContactType_id { get; set; }
        public int Institute_id { get; set; }
        public string Contact_Person_name { get; set; } = string.Empty;
        public string Telephone_number { get; set; } = string.Empty;
        public string Email_ID { get; set; } = string.Empty;
        public string Mobile_number { get; set; } = string.Empty;
        public bool? isPrimary { get; set; }
        public DateTime? en_date { get; set; }
    }
}
