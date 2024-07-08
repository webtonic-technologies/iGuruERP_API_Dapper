namespace Institute_API.Models
{
    public class AdminDepartment
    {
        public int Department_id { get; set; }
        public int Institute_id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public bool IsDeleted {  get; set; }
    }
}
