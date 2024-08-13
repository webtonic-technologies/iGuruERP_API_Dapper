namespace Employee_API.Models
{
    public class Designation
    {
        public int Designation_id { get; set; }
        public int Institute_id { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public int Department_id { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class Department
    {
        public int Department_id { get; set; }
        public int Institute_id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
