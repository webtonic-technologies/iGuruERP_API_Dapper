using Institute_API.Models;

namespace Institute_API.DTOs
{
    public class InstituteDetailsDTO
    {
        public int Institute_id { get; set; }
        public string Institute_name { get; set; } = string.Empty;
        public string Institute_Alias { get; set; } = string.Empty;
        public DateTime? en_date { get; set; }
        public List<InstituteLogos>? InstituteLogos { get; set; }
        public List<InstituteDigitalStamps>? InstituteDigitalStamps { get; set; }
        public List<InstituteDigitalSigns>? InstituteDigitalSigns { get; set; }
        public List<InstitutePrinSigns>? InstitutePrinSigns { get; set; }
        public List<InstituteAddress>? InstituteAddresses { get; set; }
        public List<SchoolContact>? SchoolContacts { get; set; }
        public List<InstituteSMMapping>? InstituteSMMappings { get; set; }
        public InstituteDescription? InstituteDescription { get; set; }
        public List<AcademicInfo>? AcademicInfos { get; set; }
        public SemesterInfo? SemesterInfo { get; set; }
    }
    public class InstituteLogos
    {
        public int InstituteLogoId { get; set; }
        public int InstituteId { get; set; }
        public string InstituteLogo { get; set; } = string.Empty;
    }
    public class InstituteDigitalStamps
    {
        public int InstituteDigitalStampId { get; set; }
        public int InstituteId { get; set; }
        public string DigitalStamp { get; set; } = string.Empty;
    }
    public class InstituteDigitalSigns
    {
        public int InstituteDigitalSignId {  get; set; }
        public int InstituteId { get; set; }
        public string DigitalSign {  get; set; } = string.Empty;
    }
    public class InstitutePrinSigns
    {
        public int InstitutePrinSignId {  get; set; }
        public int InstituteId { get; set; }
        public string InstitutePrinSign { get; set; } = string.Empty;
    }
}