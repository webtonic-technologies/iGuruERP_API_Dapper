namespace Employee_API.Models
{
    public class EmployeeBankDetails
    {
        public int bank_id { get; set; }
        public int employee_id { get; set; }
        public string bank_name { get; set; } = string.Empty;
        public string account_name { get; set; } = string.Empty;
        public string account_number { get; set; } = string.Empty;
        public string IFSC_code { get; set; } = string.Empty;
        public string Bank_address { get; set; } = string.Empty;
    }
    public class EmployeeAddressDetails
    {
        public int Employee_Present_Address_id { get; set; }
        public string Address { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string StateName { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string Pin_code { get; set; } = string.Empty;
        public int AddressTypeId { get; set; }
        public int Employee_id {  get; set; }
    }
}
