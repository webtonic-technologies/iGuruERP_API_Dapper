using Institute_API.Models;

namespace Institute_API.DTOs
{
    public class InstituteDetailsDTO
    {
        public int Institute_id { get; set; }
        public string Institute_name { get; set; } = string.Empty;
        public string Institute_Alias { get; set; } = string.Empty;
        public DateTime? en_date { get; set; }
        public string Institute_Logo { get; set; } = string.Empty;
        public string Institute_DigitalStamp { get; set; } = string.Empty;
        public string Institute_DigitalSignatory { get; set; } = string.Empty;
        public string Institute_PrincipalSignatory { get; set; } = string.Empty;
        public List<InstituteAddress>? InstituteAddresses { get; set; }
        public List<SchoolContact>? SchoolContacts { get; set; }
        public List<InstituteSMMapping>? InstituteSMMappings { get; set; }
        public InstituteDescription? InstituteDescription { get; set; }
        public List<AcademicInfo>? AcademicInfos { get; set; }
        public SemesterInfo? SemesterInfo { get; set; }
    }
}
