namespace Employee_API.Models
{
    public class EmployeeDocument
    {
        public int Document_id { get; set; }
        public int employee_id { get; set; }
        public string Document_Name { get; set; } = string.Empty;
        public string file_name { get; set; } = string.Empty;
        public string file_path { get; set; } = string.Empty;
    }
}
