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
}
