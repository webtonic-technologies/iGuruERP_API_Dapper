using Employee_API.Models;

namespace Employee_API.DTOs
{
    public class EmployeeProfileResponseDTO
    {
        public int? Employee_id { get; set; }
        public string First_Name { get; set; } = string.Empty;
        public string Middle_Name { get; set; } = string.Empty;
        public string Last_Name { get; set; } = string.Empty;
        public int? Gender_id { get; set; }
        public string GenderName { get; set; } = string.Empty;
        public int? Department_id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int? Designation_id { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public string mobile_number { get; set; } = string.Empty;
        public string Date_of_Joining { get; set; } = string.Empty;
        public int? Nationality_id { get; set; }
        public string NationalityName { get; set; } = string.Empty;
        public int? Religion_id { get; set; }
        public string ReligionName { get; set; } = string.Empty;
        public string Date_of_Birth { get; set; } = string.Empty;
        public string EmailID { get; set; } = string.Empty;
        public string Employee_code_id { get; set; } = string.Empty;
        public int? marrital_status_id { get; set; }
        public string MaritalStatusName { get; set; } = string.Empty;
        public int? Blood_Group_id { get; set; }
        public string BloodGroupName { get; set; } = string.Empty;
        public string aadhar_no { get; set; } = string.Empty;
        public string pan_no { get; set; } = string.Empty;
        public string EPF_no { get; set; } = string.Empty;
        public string ESIC_no { get; set; } = string.Empty;
        public int? Institute_id { get; set; }
        public string EmpPhoto { get; set; } = string.Empty;
        public string uan_no { get; set; } = string.Empty;
        public bool Status { get; set; }
        public List<EmployeeDocument>? EmployeeDocuments { get; set; }
        public List<EmployeeQualification>? EmployeeQualifications { get; set; }
        public List<EmployeeWorkExperience>? EmployeeWorkExperiences { get; set; }
        public List<EmployeeBankDetails>? EmployeeBankDetails { get; set; }
        public EmployeeFamily? Family { get; set; }
        public EmployeeAddressResponse? EmployeeAddressDetails { get; set; }
        public EmployeeStaffMappingResponse EmployeeStaffMappingResponse {  get; set; }
    }
    public class EmployeeAddressResponse
    {
        public List<EmployeeAddressDetailsResponse>? PresentAddress { get; set; }
        public List<EmployeeAddressDetailsResponse>? PermanentAddress { get; set; }
    }
    public class EmployeeExportHistoryDto
    {
        public int HistoryId { get; set; }
        public int EmployeeCount { get; set; }
        public string DownloadDate { get; set; }
        public string IPAddress { get; set; }
        public string Username { get; set; }
        public int InstituteId { get; set; }
    }

    public class EmployeeAddressDetailsResponse
    {
        public int? Employee_Present_Address_id { get; set; }
        public string Address { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string StateName { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string Pin_code { get; set; } = string.Empty;
        public int? AddressTypeId { get; set; }
        public int? Employee_id { get; set; }
    }
    public class EmployeeCredentialsResponse
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string LoginId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? LastActivity { get; set; }
    }
    public class EmployeeNonAppUsersResponse
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Departemnt { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
    }
    public class EmployeeActivityResponse
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Departemnt { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string LastActionTaken { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }
    public class EmployeeLoginResposne
    {
        public string Username { get; set; } = string.Empty;
        public string InstituteLogo { get; set; } = string.Empty;
        public int InstituteId { get; set; }
    }
    public class LoginResposne
    {
        public string Username { get; set; }
        public int UserId { get; set; }
        public int InstituteId { get; set; }
        public string UserType { get; set; }
        public List<ModuleResponse> ModulesAndSubmodules { get; set; } = new List<ModuleResponse>();
    }

    public class ModuleResponse
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public bool IsActive { get; set; }
       // public int IsApp { get; set; }
        public List<SubModuleResponse> Submodules { get; set; } = new List<SubModuleResponse>();
    }

    public class SubModuleResponse
    {
        public int SubModuleID { get; set; }
        public string SubModuleName { get; set; }
        public bool IsActive { get; set; }
        public List<FunctionalityResponse> Functionalities { get; set; } = new List<FunctionalityResponse>();
    }

    public class FunctionalityResponse
    {
        public int FunctionalityId { get; set; }
        public string Functionality { get; set; }
        public bool IsActive { get; set; }
    }

    public class EmployeeStaffMappingResponse
    {
        public int EmployeeId { get; set; }
        public List<EmployeeStaffMapClassTeacherResponse>? EmployeeStaffMappingsClassTeacher { get; set; }
        public List<EmployeeStappMapClassSectionResponse>? EmployeeStappMappingsClassSection { get; set; }
    }
    public class EmployeeStaffMapClassTeacherResponse
    {
        public int MappingId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public List<Subjects>? subjects { get; set; }
    }
    public class Subjects
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
    }
    public class EmployeeStappMapClassSectionResponse
    {
        public int ClassSectionMapId { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int? ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public List<Sections>? sections { get; set; }
    }
    public class Sections
    {
        public int? SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
    }
    public class ClassSectionList
    {
        public int classId { get; set; }
        public int sectionId {  get; set; }
        public string ClassSection {  get; set; } = string.Empty;//8 - A
    }
    public class ClassSectionSubjectResponse
    {
        public int classId { set; get; }
        public string ClassName {  set; get; } = string.Empty;
        public int SectionId {  get; set; }
        public string SectionName {  set; get; } = string.Empty;
        public List<Subjects>? subjects { get; set; }
    }
    public class UserSwitchOverResponse
    {
        public List<StudentResponse> Students { get; set; }
        public List<EmployeeResponse> Employees { get; set; }
    }

    public class StudentResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }

    public class EmployeeResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
    }
    public class ForgetPasswordResponse
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Usertype { get; set; } = string.Empty;
        public string UserName { set; get; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class EmployeeColumn
    {
        public int ECMId { get; set; }
        public string ColumnDisplayName { get; set; }
        public string ColumnDatabaseName { get; set; }
        public int CategoryId { get; set; }
        public bool Status { get; set; }
    }
    public class CategoryWiseEmployeeColumns
    {
        public int CategoryId { get; set; }
        public List<EmployeeColumn> EmployeeColumns { get; set; }
    }
}