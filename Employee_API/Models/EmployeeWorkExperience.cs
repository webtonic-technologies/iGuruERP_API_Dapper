namespace Employee_API.Models
{
    public class EmployeeWorkExperience
    {
        public int work_experience_id { get; set; }
        public string Year { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public string Previous_Organisation { get; set; } = string.Empty;
        public string Previous_Designation { get; set; } = string.Empty;
        public int Employee_id { get; set; }
    }
}
