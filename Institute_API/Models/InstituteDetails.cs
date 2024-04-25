namespace Institute_API.Models
{
    public class InstituteDetails
    {
        public int Institute_id { get; set; }
        public string Institute_name { get; set; } = string.Empty;
        public string Institute_Alias { get; set; } = string.Empty;
        public string Institute_Logo { get; set; } = string.Empty;
        public string Institute_DigitalStamp { get; set; } = string.Empty;
        public string Institute_DigitalSignatory { get; set; } = string.Empty;
        public string Institute_PrincipalSignatory { get; set; } = string.Empty;
        public DateTime? en_date { get; set; }
    }
}
