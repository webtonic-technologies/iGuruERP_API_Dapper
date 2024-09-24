using Institute_API.Models;

namespace Institute_API.DTOs
{
    public class InstituteDetailsResponseDTO
    {

        public int Institute_id { get; set; }
        public string Institute_name { get; set; } = string.Empty;
        public string Institute_Alias { get; set; } = string.Empty;
        public DateTime? en_date { get; set; }
        public List<InstituteLogosResponse>? InstituteLogos { get; set; }
        public List<InstituteDigitalStampsResponse>? InstituteDigitalStamps { get; set; }
        public List<InstituteDigitalSignsResponse>? InstituteDigitalSigns { get; set; }
        public List<InstitutePrinSignsResponse>? InstitutePrinSigns { get; set; }
        public InstituteAddressResponse? AddressResponse { get; set; }
        public List<SchoolContactResponse>? SchoolContacts { get; set; }
        public List<InstituteSMMappingResponse>? InstituteSMMappings { get; set; }
        public InstituteDescription? InstituteDescription { get; set; }
        public List<AcademicInfo>? AcademicInfos { get; set; }
        public SemesterInfo? SemesterInfo { get; set; }
    }
    public class InstituteSMMappingResponse
    {
        public int SM_Mapping_Id { get; set; }
        public int Institute_id { get; set; }
        public int SM_Id { get; set; }
        public string SM_Name { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
    }
    public class InstituteLogosResponse
    {
        public int InstituteLogoId { get; set; }
        public int InstituteId { get; set; }
        public string InstituteLogo { get; set; } = string.Empty;
    }
    public class InstituteDigitalStampsResponse
    {
        public int InstituteDigitalStampId { get; set; }
        public int InstituteId { get; set; }
        public string DigitalStamp { get; set; } = string.Empty;
    }
    public class InstituteDigitalSignsResponse
    {
        public int InstituteDigitalSignId { get; set; }
        public int InstituteId { get; set; }
        public string DigitalSign { get; set; } = string.Empty;
    }
    public class InstitutePrinSignsResponse
    {
        public int InstitutePrinSignId { get; set; }
        public int InstituteId { get; set; }
        public string InstitutePrinSign { get; set; } = string.Empty;
    }
    public class AddressResponse
    {
        public int Institute_address_id { get; set; }
        public int Institute_id { get; set; }
        public int country_id { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public int state_id { get; set; }
        public string StateName { get; set; } = string.Empty;
        public int city_id { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string house { get; set; } = string.Empty;
        public string pincode { get; set; } = string.Empty;
        public int district_id { get; set; }
        public string DistrictName {  get; set; } = string.Empty;
        public string Locality { get; set; } = string.Empty;
        public string Landmark { get; set; } = string.Empty;
        public string Mobile_number { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? AddressType_id { get; set; }
        public string AddressTypeName { get; set; } = string.Empty;
        public DateTime? en_date { get; set; }
    }
    public class InstituteAddressResponse
    {
        public List<AddressResponse>? BillingAddress { get; set; }
        public List<AddressResponse>? MailingAddress {  get; set; }
    }
    public class SchoolContactResponse
    {
        public int School_Contact_id { get; set; }
        public int ContactType_id { get; set; }
        public string ContactType_Name { get; set; } = string.Empty;
        public int Institute_id { get; set; }
        public string Contact_Person_name { get; set; } = string.Empty;
        public string Telephone_number { get; set; } = string.Empty;
        public string Email_ID { get; set; } = string.Empty;
        public string Mobile_number { get; set; } = string.Empty;
        public bool? isPrimary { get; set; }
        public DateTime? en_date { get; set; }
    }
    public class AcademicYearMaster
    {
        public int yearId { get; set; }
        public string YearName { get; set; } = string.Empty;
    }
}
