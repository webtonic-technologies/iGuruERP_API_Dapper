using Institute_API.Models;

namespace Institute_API.DTOs
{
    public class InstituteDetailsDTO
    {
        public int Institute_id { get; set; }
        public string Institute_name { get; set; } = string.Empty;
        public string Institute_Alias { get; set; } = string.Empty;
        public IFormFile? Institute_Logo { get; set; }
        public IFormFile? Institute_DigitalStamp { get; set; }
        public IFormFile? Institute_DigitalSignatory { get; set; }
        public IFormFile? Institute_PrincipalSignatory { get; set; }
        public DateTime? en_date { get; set; }
        public List<InstituteAddress>? InstituteAddresses { get; set; }
        public List<SchoolContact>? SchoolContacts { get; set; }
        public List<InstituteSMMapping>? InstituteSMMappings { get; set; }
        public InstituteDescription? InstituteDescription { get; set; }
    }
}
