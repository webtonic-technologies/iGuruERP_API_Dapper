using Employee_API.Models;

namespace Employee_API.DTOs
{
    public class EmployeeProfileResponseDTO
    {
        public int Employee_id { get; set; }
        public string First_Name { get; set; } = string.Empty;
        public string Middle_Name { get; set; } = string.Empty;
        public string Last_Name { get; set; } = string.Empty;
        public int Gender_id { get; set; }
        public string GenderName {  get; set; } = string.Empty;
        public int Department_id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int Designation_id { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public string mobile_number { get; set; } = string.Empty;
        public string Date_of_Joining { get; set; } = string.Empty;
        public int Nationality_id { get; set; }
        public string NationalityName { get; set; } = string.Empty;
        public int Religion_id { get; set; }
        public string ReligionName { get; set; } = string.Empty;
        public string Date_of_Birth { get; set; } = string.Empty;
        public string EmailID { get; set; } = string.Empty;
        public string Employee_code_id { get; set; } = string.Empty;
        public int marrital_status_id { get; set; }
        public string MaritalStatusName { get; set; } = string.Empty;
        public int Blood_Group_id { get; set; }
        public string BloodGroupName { get; set; } = string.Empty;
        public string aadhar_no { get; set; } = string.Empty;
        public string pan_no { get; set; } = string.Empty;
        public string EPF_no { get; set; } = string.Empty;
        public string ESIC_no { get; set; } = string.Empty;
        public int Institute_id { get; set; }
        public string EmpPhoto { get; set; } = string.Empty;
        public string uan_no { get; set; } = string.Empty;
        public bool Status {  get; set; }
        public List<EmployeeDocument>? EmployeeDocuments { get; set; }
        public List<EmployeeQualification>? EmployeeQualifications { get; set; }
        public List<EmployeeWorkExperience>? EmployeeWorkExperiences { get; set; }
        public List<EmployeeBankDetails>? EmployeeBankDetails { get; set; }
        public EmployeeFamily? Family { get; set; }
        public EmployeeAddressResponse? EmployeeAddressDetails { get; set; }
    }
    public class EmployeeAddressResponse
    {
        public List<EmployeeAddressDetailsResponse>? PresentAddress { get; set; }
        public List<EmployeeAddressDetailsResponse>? PermanentAddress { get; set; }
    }
    public class EmployeeAddressDetailsResponse
    {
        public int Employee_Present_Address_id { get; set; }
        public string Address { get; set; } = string.Empty;
        public int Country_id { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public int State_id { get; set; }
        public string StateName {  get; set; } = string.Empty;
        public int City_id { get; set; }
        public string CityName { get; set; } = string.Empty;
        public int District_id { get; set; }
        public string DistrictName {  get; set; } = string.Empty;
        public string Pin_code { get; set; } = string.Empty;
        public int AddressTypeId { get; set; }
        public int Employee_id { get; set; }
    }       
}
