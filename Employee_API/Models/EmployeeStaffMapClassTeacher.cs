namespace Employee_API.Models
{
    public class EmployeeStaffMapClassTeacher
    {
        public int MappingId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public string SubjectId { get; set; } = string.Empty;
    }
    public class EmployeeStappMapClassSection
    {
        public int ClassSectionMapId { get; set; }
        public int SubjectId { get; set; }
        public int ClassId { get; set; }
        public string SectionId { get; set; } = string.Empty;
    }
    public class EmployeeStaffMappingRequest
    {
        public int EmployeeId { get; set; }
        public EmployeeStaffMapClassTeacher? EmployeeStaffMappingsClassTeacher { get; set; }
        public EmployeeStappMapClassSection? EmployeeStappMappingsClassSection { get; set; }
    }
}
