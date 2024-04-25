namespace Employee_API.Models
{
    public class EmployeeQualification
    {
        public int Qualification_Info_id { get; set; }
        public int employee_id { get; set; }
        public string Educational_Qualification { get; set; } = string.Empty;
        public string Year_of_Completion { get; set; } = string.Empty;
    }
}
