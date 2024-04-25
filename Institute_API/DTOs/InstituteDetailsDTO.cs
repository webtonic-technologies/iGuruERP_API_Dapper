using Institute_API.Models;

namespace Institute_API.DTOs
{
    public class InstituteDetailsDTO
    {
        public int Institute_id { get; set; }
        public string Institute_name { get; set; } = string.Empty;
        public string Institute_Alias { get; set; } = string.Empty;
        public DateTime? en_date { get; set; }
        public List<InstituteAddress>? InstituteAddresses { get; set; }
        public List<SchoolContact>? SchoolContacts { get; set; }
        public List<InstituteSMMapping>? InstituteSMMappings { get; set; }
        public InstituteDescription? InstituteDescription { get; set; }
    }
    public class InstLogoDTO
    {
        public int Institute_id { get; set; }
        public IFormFile? InstLogo {  get; set; }
    }
    public class InstDigiStampDTO
    {
        public int Institute_id { get; set; }
        public IFormFile? InstDigStamp { get; set; }
    }
    public class InstDigSignDTO
    {
        public int Institute_id { get; set; }
        public IFormFile? InstDigSign { get; set; }
    }
    public class InstPriSignDTO
    {
        public int Institute_id { get; set; }
        public IFormFile? InstPrinSign { get; set; }
    }
}
