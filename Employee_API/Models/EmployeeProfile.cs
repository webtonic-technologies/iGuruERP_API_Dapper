namespace Employee_API.Models
{
    public class EmployeeProfile
    {
        public int Employee_id { get; set; }
        public string First_Name { get; set; } = string.Empty;
        public string Middle_Name { get; set; } = string.Empty;
        public string Last_Name { get; set; } = string.Empty;
        public int Gender_id { get; set; }
        public int Department_id { get; set; }
        public int Designation_id { get; set; }
        public string mobile_number { get; set; } = string.Empty;
        public DateTime? Date_of_Joining { get; set; }
        public int Nationality_id { get; set; }
        public int Religion_id { get; set; }
        public DateTime? Date_of_Birth { get; set; }
        public string EmailID { get; set; } = string.Empty;
        public string Employee_code_id { get; set; } = string.Empty;
        public int marrital_status_id { get; set; }
        public int Blood_Group_id { get; set; }
        public string aadhar_no { get; set; } = string.Empty;
        public string pan_no { get; set; } = string.Empty;
        public string EPF_no { get; set; } = string.Empty;
        public string ESIC_no { get; set; } = string.Empty;
        public int Institute_id { get; set; }
        public string EmpPhoto { get; set; } = string.Empty;
        public string uan_no { get; set; } = string.Empty;
    }
}
